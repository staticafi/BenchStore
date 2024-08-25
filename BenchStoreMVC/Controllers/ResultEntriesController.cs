using BenchStoreBL.Models;
using BenchStoreBL.Options;
using BenchStoreBL.Services.Labels;
using BenchStoreBL.Services.ResultEntries;
using BenchStoreBL.Services.ResultParsing;
using BenchStoreBL.Services.ResultStoring;
using BenchStoreBL.Services.ScriptExecution;

using BenchStoreMVC.ViewModels;

using ICSharpCode.SharpZipLib.BZip2;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace BenchStoreMVC.Controllers
{
    public class ResultEntriesController : Controller
    {
        private readonly IResultEntriesService _resultEntriesService;
        private readonly ILabelsService _labelsService;
        private readonly IResultParser _resultParser;
        private readonly IFileStoring _fileStoring;
        private readonly ITableGeneratorExecutor _tableGeneratorExecutor;
        private readonly StorageOptions _storageOptions;

        public ResultEntriesController(
            IResultEntriesService resultEntriesService,
            ILabelsService labelsService,
            IResultParser resultParser,
            IFileStoring fileStoreService,
            ITableGeneratorExecutor tableGeneratorExecutor,
            IOptions<StorageOptions> options)
        {
            _resultEntriesService = resultEntriesService;
            _labelsService = labelsService;
            _resultParser = resultParser;
            _fileStoring = fileStoreService;
            _tableGeneratorExecutor = tableGeneratorExecutor;
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

            ParsedResult parsedResult;
            try
            {
                parsedResult = await ParseResultFile(viewModel.ResultFile);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError<UploadResultFileViewModel>(
                    m => m.ResultFile,
                    ex.Message
                );
                return View(viewModel);
            }

            string? resultLogsPath = null;
            if (viewModel.LogFiles != null)
            {
                try
                {
                    resultLogsPath = await StoreLogFiles(viewModel.LogFiles);
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError<UploadResultFileViewModel>(
                        m => m.LogFiles,
                        ex.Message
                    );
                    return View(viewModel);
                }
            }

            IEnumerable<Label> labels = await _labelsService
                .GetLabels();

            CreateResultEntryViewModel createResultEntryViewModel = new CreateResultEntryViewModel
            {
                ResultEntry = new ResultEntry(),
                Result = parsedResult.Result,
                ResultFileTempPath = parsedResult.ResultFilePath,
                ResultLogsTempPath = resultLogsPath,
                Labels = labels,
            };

            return View(nameof(Create), createResultEntryViewModel);
        }

        private async Task<ParsedResult> ParseResultFile(IFormFile resultFile)
        {
            string contentType = resultFile.ContentType;
            using (Stream resultFileStream = resultFile.OpenReadStream())
            {
                switch (contentType)
                {
                    case "application/octet-stream" or "application/x-bzip" or "application/x-bzip2":
                        return await _resultParser
                            .ParseCompressedResult(resultFileStream);
                    case "text/xml":
                        return await _resultParser
                            .ParseResult(resultFileStream);
                    default:
                        throw new ArgumentException($"File content type '{contentType}' not recognized. " +
                            $"The Result File needs to be of content type 'text/xml' (.xml) or 'application/octet-stream', " +
                            $"'application/x-bzip' or 'application/x-bzip2' (.xml.bz2)");
                }
            }
        }

        private async Task<string> StoreLogFiles(IFormFile logFiles)
        {
            string logsContentType = logFiles.ContentType;
            switch (logsContentType)
            {
                case "application/zip" or "application/x-zip-compressed":
                    using (Stream resultLogsStream = logFiles.OpenReadStream())
                    {
                        return await _fileStoring
                            .StoreTemporaryFile(resultLogsStream);
                    }
                default:
                    throw new ArgumentException($"File content type '{logsContentType}' not recognized. The Log Files need to be of content type application/zip or application/x-zip-compressed (.zip)");
            }
        }

        // POST: ResultEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResultEntryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string resultSubdirectoryName = Guid.NewGuid().ToString();
            string resultStoragePath = Path.Combine(_storageOptions.ResultStoragePath, resultSubdirectoryName);

            Directory.CreateDirectory(resultStoragePath);
            viewModel.ResultEntry.ResultSubdirectoryName = resultSubdirectoryName;

            string resultFileName = $"{viewModel.Result.BenchmarkName}.{viewModel.Result.Date:yyyy-MM-dd_HH-mm-ss}.results.{viewModel.Result.Name}";
            // TODO: different date!!!!
            string logFilesName = $"{viewModel.Result.BenchmarkName}.{viewModel.Result.Date:yyyy-MM-dd_HH-mm-ss}.logfiles";

            List<Label> labels = await GetLabelsFromLabelsInput(viewModel.LabelsInput);

            using (FileStream tempFileStream = System.IO.File.OpenRead(viewModel.ResultFileTempPath))
            {
                viewModel.ResultEntry.ResultFileName = await _fileStoring
                    .StoreFile(tempFileStream, resultStoragePath, resultFileName);
            }

            if (viewModel.ResultLogsTempPath != null)
            {
                using (FileStream logsFileStream = System.IO.File.OpenRead(viewModel.ResultLogsTempPath))
                {
                    viewModel.ResultEntry.LogFilesName = await _fileStoring
                        .StoreLogFiles(logsFileStream, resultStoragePath, logFilesName);
                }
            }

            int storedResultId = await _resultEntriesService
                .CreateResultEntry(viewModel.ResultEntry, viewModel.Result);

            try
            {
                await _resultEntriesService
                    .EditResultEntryLabels(storedResultId, labels);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return RedirectToAction(nameof(Index));
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
                Label? label = await _labelsService
                    .GetLabelByName(stringLabel);

                if (label == null)
                {
                    label = new Label
                    {
                        Name = stringLabel,
                    };
                    label.ID = await _labelsService
                        .CreateLabel(label);
                }

                labels.Add(label);
            }

            return labels;
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

            ResultEntry? resultEntry = await _resultEntriesService
                .GetResultEntryByID(id.Value);

            if (resultEntry == null)
            {
                return NotFound();
            }

            IEnumerable<Label> resultEntryLabels = await _labelsService
                .GetResultEntryLabels(resultEntry.ID);

            IEnumerable<Label> labels = await _labelsService
                .GetLabels();

            EditResultEntryViewModel viewModel = new EditResultEntryViewModel
            {
                ResultEntry = resultEntry,
                Result = resultEntry.Result,
                LabelsInput = string.Join(",", resultEntryLabels
                    .Select(l => l.Name)
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

            if (ModelState.IsValid)
            {
                List<Label> labels = await GetLabelsFromLabelsInput(viewModel.LabelsInput);

                try
                {
                    await _resultEntriesService
                        .EditResultEntry(viewModel.ResultEntry);
                    await _resultEntriesService
                        .EditResultEntryLabels(viewModel.ResultEntry.ID, labels);
                }
                catch (ArgumentException e)
                {
                    return NotFound(e.Message);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
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

            await _resultEntriesService
                .DeleteResultEntry(id);

            if (resultEntry != null && resultEntry.ResultFileName != null && resultEntry.LogFilesName != null)
            {
                _fileStoring.DeleteFile(Path.Combine(resultEntry.ResultSubdirectoryName, resultEntry.ResultFileName), _storageOptions.ResultStoragePath);
                _fileStoring.DeleteFile(Path.Combine(resultEntry.ResultSubdirectoryName, resultEntry.LogFilesName), _storageOptions.ResultStoragePath);
                Directory.Delete(Path.Combine(_storageOptions.ResultStoragePath, resultEntry.ResultSubdirectoryName));
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ResultEntries/Download/id
        public async Task<IActionResult> Download(int? id, bool decompress = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? storedResult = await _resultEntriesService
                .GetResultEntryByID(id.Value);

            if (storedResult == null || storedResult.ResultFileName == null)
            {
                return NotFound();
            }

            string downloadFileName = storedResult.ResultFileName;

            return DownloadFiles(storedResult.ResultSubdirectoryName, storedResult.ResultFileName, decompress, storedResult.ResultFileName, "application/octet-stream");
        }

        // GET: ResultEntries/Download/id
        public async Task<IActionResult> DownloadLogFiles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? storedResult = await _resultEntriesService
                .GetResultEntryByID(id.Value);

            if (storedResult == null || storedResult.LogFilesName == null)
            {
                return NotFound();
            }

            string downloadFileName = storedResult.LogFilesName;

            return DownloadFiles(storedResult.ResultSubdirectoryName, storedResult.LogFilesName, false, storedResult.LogFilesName, "application/zip");
        }

        private IActionResult DownloadFiles(string subdirectoryName, string fileName, bool isCompressedXML, string downloadFileName, string contentType)
        {
            string resultStoragePath = _storageOptions.ResultStoragePath;

            string resultFilePath = Path.Combine(resultStoragePath, subdirectoryName, fileName);
            if (!System.IO.File.Exists(resultFilePath))
            {
                return NotFound();
            }

            FileStream resultStream = System.IO.File.OpenRead(resultFilePath);

            if (isCompressedXML)
            {
                BZip2InputStream bzipInputStream = new BZip2InputStream(resultStream);
                bzipInputStream.IsStreamOwner = true;

                downloadFileName = Path.ChangeExtension(downloadFileName, null);
                return File(bzipInputStream, "text/xml", downloadFileName);
            }

            return File(resultStream, contentType, downloadFileName);
        }

        // GET: ResultEntries/GenerateTable/id
        public async Task<IActionResult> GenerateTable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResultEntry? resultEntry = await _resultEntriesService
                .GetResultEntryByID(id.Value);
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
            List<string> logFilePaths = new List<string>();

            //string resultStoragePath = _storageOptions.ResultStoragePath;
            // TODO: try specifying with URL
            string resultStoragePath = HttpContext.Request.Host.Value;

            foreach (int id in resultEntryIDs)
            {
                ResultEntry? storedResult = await _resultEntriesService
                    .GetResultEntryByID(id);

                if (storedResult == null || storedResult.ResultFileName == null)
                {
                    return NotFound();
                }

                //string resultFilePath = Path.Combine(resultStoragePath, storedResult.ResultSubdirectoryName, storedResult.ResultFileName);
                string resultFilePath = $"{(HttpContext.Request.IsHttps ? "https" : "http")}://{resultStoragePath}/Results/{storedResult.ResultSubdirectoryName}/{storedResult.ResultFileName}";

                //if (!System.IO.File.Exists(resultFilePath))
                //{
                //    return NotFound();
                //}

                resultFilePaths.Add(resultFilePath);

                if (storedResult.LogFilesName != null)
                {
                    string logFilePath = Path.Combine(resultStoragePath, storedResult.ResultSubdirectoryName, storedResult.LogFilesName);

                    //if (!System.IO.File.Exists(logFilePath))
                    //{
                    //    return NotFound();
                    //}

                    logFilePaths.Add(logFilePath);
                }
            }

            //string htmlPath = await _tableGeneratorExecutor
            //    .ExecuteTableGenerator(resultFilePaths, logFilePaths);
            string htmlPath = await _tableGeneratorExecutor
                .ExecuteTableGenerator(resultFilePaths);

            FileStreamOptions fileStreamOptions = new FileStreamOptions
            {
                Options = FileOptions.DeleteOnClose
            };

            using (StreamReader reader = new StreamReader(htmlPath, fileStreamOptions))
            {
                return base.Content(await reader.ReadToEndAsync(), "text/html");
            }
        }
    }
}

