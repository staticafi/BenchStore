using BenchStoreBL.Models;
using BenchStoreBL.Models.Mappers;
using BenchStoreBL.Options;
using BenchStoreBL.Services.Labels;
using BenchStoreBL.Services.ResultEntries;
using BenchStoreBL.Services.Results;
using BenchStoreBL.Services.ResultStoring;
using BenchStoreBL.Services.ScriptExecution;
using BenchStoreBL.Services.XMLElementParsing;
using BenchStoreBL.XMLData;
using BenchStoreMVC.ViewModels;
using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace BenchStoreMVC.Controllers
{
    public class ResultEntriesController : Controller
    {
        private readonly IResultEntriesService _resultEntriesService;
        private readonly IResultsService _resultsService;
        private readonly ILabelsService _labelsService;
        private readonly IFileStorage _fileStorage;
        private readonly ITableGeneratorExecutor _tableGeneratorExecutor;
        private readonly IXMLElementParser _xmlElementParser;
        private readonly StorageOptions _storageOptions;

        public ResultEntriesController(
            IResultEntriesService resultEntriesService,
            IResultsService resultsService,
            ILabelsService labelsService,
            IFileStorage fileStorage,
            ITableGeneratorExecutor tableGeneratorExecutor,
            IXMLElementParser xmlElementParser,
            IOptions<StorageOptions> options)
        {
            _resultEntriesService = resultEntriesService;
            _resultsService = resultsService;
            _labelsService = labelsService;
            _fileStorage = fileStorage;
            _tableGeneratorExecutor = tableGeneratorExecutor;
            _xmlElementParser = xmlElementParser;
            _storageOptions = options.Value;
        }

        // GET: ResultEntries
        public async Task<IActionResult> Index(FilterResultEntriesViewModel viewModel)
        {
            List<ResultEntry> storedResults = await _resultEntriesService
                .GetResultEntries(OrderResultEntryBy.Date);

            IEnumerable<string?> tools = storedResults
                .Select(sr => sr.Result.Tool)
                .Distinct()
                .Order();

            List<Label> labels = await _labelsService
                .GetLabels();

            viewModel.Labels = labels;
            viewModel.Tools = new SelectList(tools);

            return View(viewModel);
        }

        // GET: ResultEntries/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: ResultEntries/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadResultFileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Result result;
            string tempFilePath;

            try
            {
                using (Stream resultFileStream = viewModel.ResultFile.OpenReadStream())
                {
                    switch (viewModel.ResultFile.ContentType)
                    {
                        case "application/octet-stream" or "application/x-bzip" or "application/x-bzip2" or "application/x-compressed":
                            using (MemoryStream decompressedStream = new MemoryStream())
                            {
                                using (BZip2InputStream bzipInputStream = new BZip2InputStream(resultFileStream))
                                {
                                    bzipInputStream.IsStreamOwner = false;
                                    await bzipInputStream.CopyToAsync(decompressedStream);

                                }
                                decompressedStream.Position = 0;
                                result = ParseResult(decompressedStream);

                                decompressedStream.Position = 0;
                                tempFilePath = await _fileStorage.StoreTemporaryFile(decompressedStream);
                            }
                            break;
                        case "text/xml":
                            result = ParseResult(resultFileStream);

                            resultFileStream.Position = 0;
                            tempFilePath = await _fileStorage.StoreTemporaryFile(resultFileStream);
                            break;
                        default:
                            throw new ArgumentException($"File content type '{viewModel.ResultFile.ContentType}' not recognized. The Result File needs to be of content type 'text/xml' (.xml) or 'application/octet-stream', 'application/x-bzip' or 'application/x-bzip2' (.xml.bz2)");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError<UploadResultFileViewModel>(
                    m => m.ResultFile,
                    ex.Message
                );

                return View(viewModel);
            }
            catch (FormatException ex)
            {
                ModelState.AddModelError<UploadResultFileViewModel>(
                    m => m.ResultFile,
                    ex.Message
                );

                return View(viewModel);
            }

            string resultName = _resultsService.GetResultName(result);
            if (!viewModel.ResultFile.FileName.Contains(resultName))
            {
                ModelState.AddModelError<UploadResultFileViewModel>(
                    m => m.ResultFile,
                    "The file name does not match the result metadata."
                );

                return View(viewModel);
            }

            int namePosition = viewModel.ResultFile.FileName.IndexOf(resultName);
            string filePrefix = viewModel.ResultFile.FileName.Substring(0, namePosition);

            string? resultLogsPath = null;
            if (viewModel.LogFiles != null)
            {
                string logFilesName = _resultsService.GetLogFilesName(result);
                if (!viewModel.LogFiles.FileName.Contains(logFilesName))
                {
                    ModelState.AddModelError<UploadResultFileViewModel>(
                        m => m.LogFiles,
                        "The file name does not match the result metadata."
                    );

                    return View(viewModel);
                }

                using (Stream resultLogsStream = viewModel.LogFiles.OpenReadStream())
                {
                    switch (viewModel.LogFiles.ContentType)
                    {
                        case "application/zip" or "application/x-zip-compressed":
                            resultLogsPath = await _fileStorage.StoreTemporaryFile(resultLogsStream);
                            break;
                        default:
                            ModelState.AddModelError<UploadResultFileViewModel>(
                                m => m.LogFiles,
                                $"File content type '{viewModel.LogFiles.ContentType}' not recognized. The Log Files need to be of content type 'application/zip' or 'application/x-zip-compressed' (.zip)"
                            );

                            return View(viewModel);
                    }
                }
            }

            IEnumerable<Label> labels = await _labelsService
                .GetLabels();

            CreateResultEntryViewModel createResultEntryViewModel = new CreateResultEntryViewModel
            {
                ResultEntry = new ResultEntry(),
                Result = result,
                ResultFileTempPath = tempFilePath,
                ResultLogsTempPath = resultLogsPath,
                Labels = labels,
                FileNamePrefix = filePrefix,
            };

            return View(nameof(Create), createResultEntryViewModel);
        }

        private Result ParseResult(Stream resultStream)
        {
            XMLResultElement xmlResultElement = _xmlElementParser.ParseXMLElement<XMLResultElement>(resultStream);
            return xmlResultElement.MapToModel();
        }

        // POST: ResultEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResultEntryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Labels = await _labelsService.GetLabels();
                return View(viewModel);
            }

            viewModel.ResultEntry.ResultSubdirectoryName = _fileStorage.CreateNewRandomStorageDirectory(_storageOptions.ResultStoragePath);

            string resultName = _resultsService.GetResultName(viewModel.Result);
            viewModel.ResultEntry.ResultFileName = await _fileStorage.MoveTemporaryFileToStore(
                viewModel.ResultFileTempPath,
                viewModel.ResultEntry.ResultSubdirectoryName,
                $"{viewModel.FileNamePrefix}{resultName}"
            );

            if (viewModel.ResultLogsTempPath != null)
            {
                string logFilesName = _resultsService.GetLogFilesName(viewModel.Result);
                viewModel.ResultEntry.LogFilesName = await _fileStorage.MoveTemporaryLogFilesToStore(
                    viewModel.ResultLogsTempPath,
                    viewModel.ResultEntry.ResultSubdirectoryName,
                    $"{viewModel.FileNamePrefix}{logFilesName}"
                );
            }

            int storedResultId = await _resultEntriesService.CreateResultEntry(viewModel.ResultEntry, viewModel.Result);

            List<Label> labels = await GetLabelsFromLabelsInput(viewModel.LabelsInput);

            try
            {
                await _resultEntriesService.EditResultEntryLabels(storedResultId, labels);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ResultEntries/Details/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? resultEntry = await _resultEntriesService.GetResultEntryByID(id.Value);

            if (resultEntry == null)
            {
                return NotFound();
            }

            return View(resultEntry);
        }

        // GET: ResultEntries/Edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? resultEntry = await _resultEntriesService.GetResultEntryByID(id.Value);

            if (resultEntry == null)
            {
                return NotFound();
            }

            IEnumerable<Label> resultEntryLabels = await _labelsService.GetResultEntryLabels(resultEntry.ID);

            IEnumerable<Label> labels = await _labelsService.GetLabels();

            EditResultEntryViewModel viewModel = new EditResultEntryViewModel
            {
                ResultEntry = resultEntry,
                Result = resultEntry.Result,
                LabelsInput = string.Join(",",
                    resultEntryLabels.Select(l => l.Name)
                ),
                Labels = labels,
            };

            return View(viewModel);
        }

        // POST: ResultEntries/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditResultEntryViewModel viewModel)
        {
            if (id != viewModel.ResultEntry.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            List<Label> labels = await GetLabelsFromLabelsInput(viewModel.LabelsInput);

            try
            {
                await _resultEntriesService.EditResultEntry(viewModel.ResultEntry);
                await _resultEntriesService.EditResultEntryLabels(viewModel.ResultEntry.ID, labels);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: ResultEntries/Delete/id
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(new ResultEntry { ID = id.Value });
        }

        // POST: ResultEntries/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            ResultEntry? resultEntry = await _resultEntriesService.GetResultEntryByID(id);

            if (resultEntry == null)
            {
                return NotFound();
            }

            await _resultEntriesService.DeleteResultEntry(id);

            _fileStorage.DeleteStore(resultEntry.ResultSubdirectoryName);

            return RedirectToAction(nameof(Index));
        }

        // GET: ResultEntries/Download/id
        public async Task<IActionResult> Download(int? id, bool decompress = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? storedResult = await _resultEntriesService.GetResultEntryByID(id.Value);

            if (storedResult == null)
            {
                return NotFound();
            }

            return DownloadFiles(storedResult.ResultSubdirectoryName, storedResult.ResultFileName, decompress, storedResult.ResultFileName, "application/octet-stream");
        }

        // GET: ResultEntries/DownloadLogFiles/id
        public async Task<IActionResult> DownloadLogFiles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? storedResult = await _resultEntriesService.GetResultEntryByID(id.Value);

            if (storedResult == null || storedResult.LogFilesName == null)
            {
                return NotFound();
            }

            return DownloadFiles(storedResult.ResultSubdirectoryName, storedResult.LogFilesName, false, storedResult.LogFilesName, "application/zip");
        }

        private IActionResult DownloadFiles(string subdirectoryName, string fileName, bool decompress, string downloadFileTitle, string contentType)
        {
            try
            {
                Stream resultStream = _fileStorage.OpenFileReader(subdirectoryName, fileName, decompress);

                if (decompress)
                {
                    downloadFileTitle = Path.ChangeExtension(downloadFileTitle, null);
                    contentType = "text/xml";
                }

                return File(resultStream, contentType, downloadFileTitle);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: ResultEntries/GenerateTable/id
        public async Task<IActionResult> GenerateTable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? resultEntry = await _resultEntriesService.GetResultEntryByID(id.Value);
            if (resultEntry == null)
            {
                return NotFound();
            }

            return await GenerateTableFromIDs(new List<int>() { resultEntry.ID });
        }

        // POST: ResultEntries/GenerateTableMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateTableMultiple(IEnumerable<ListResultEntryViewModel> resultEntries)
        {
            IEnumerable<int> IDs = resultEntries
                .Where(re => re.SelectResult.IsChecked)
                .Select(re => re.SelectResult.ID);

            return await GenerateTableFromIDs(IDs);
        }

        private async Task<IActionResult> GenerateTableFromIDs(IEnumerable<int> resultEntryIDs)
        {
            List<string> resultFilePaths = new List<string>();

            foreach (int id in resultEntryIDs)
            {
                ResultEntry? storedResult = await _resultEntriesService.GetResultEntryByID(id);

                if (storedResult == null || !_fileStorage.FileExists(storedResult.ResultSubdirectoryName, storedResult.ResultFileName))
                {
                    return NotFound();
                }

                resultFilePaths.Add($"{storedResult.ResultSubdirectoryName}/{storedResult.ResultFileName}");

                if (storedResult.LogFilesName != null && !_fileStorage.FileExists(storedResult.ResultSubdirectoryName, storedResult.LogFilesName))
                {
                    return NotFound();
                }
            }

            string hostUrl = $"{(HttpContext.Request.IsHttps ? "https" : "http")}://{HttpContext.Request.Host.Value}/Results";
            string content = await _tableGeneratorExecutor.ExecuteTableGenerator(hostUrl, resultFilePaths);

            return base.Content(content, "text/html");
        }

        private async Task<List<Label>> GetLabelsFromLabelsInput(string? labelsInput)
        {
            List<Label> labels = new List<Label>();

            if (labelsInput == null)
            {
                return labels;
            }

            string[] stringLabels = labelsInput.Split(",");

            foreach (string stringLabel in stringLabels)
            {
                Label? label = await _labelsService.GetLabelByName(stringLabel);

                if (label == null)
                {
                    label = new Label
                    {
                        Name = stringLabel,
                    };
                    label.ID = await _labelsService.CreateLabel(label);
                }

                labels.Add(label);
            }

            return labels;
        }
    }
}

