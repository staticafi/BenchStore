using BenchStoreBL.Options;
using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.Extensions.Options;

namespace BenchStoreBL.Services.ResultStoring
{
    internal class ResultFileStorage : IFileStorage
    {
        private readonly StorageOptions _storageOptions;

        public ResultFileStorage(IOptions<StorageOptions> storageOptions)
        {
            _storageOptions = storageOptions.Value;
        }

        public async Task<string> StoreTemporaryFile(Stream inputStream)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var tempFileStream = File.Create(tempFilePath))
            {
                await inputStream.CopyToAsync(tempFileStream);
            }

            return tempFilePath;
        }

        public async Task<string> MoveTemporaryFileToStore(string temporaryFilePath, string subdirectoryName, string outputFileName)
        {
            string fullFileName = $"{outputFileName}.xml.bz2";
            string resultFilePath = Path.Combine(_storageOptions.ResultStoragePath, subdirectoryName, fullFileName);

            using (FileStream tempFileStream = File.OpenRead(temporaryFilePath))
            {
                using (FileStream xmlFileStream = File.OpenWrite(resultFilePath))
                {
                    using (BZip2OutputStream bzipOutputStream = new BZip2OutputStream(xmlFileStream))
                    {
                        await tempFileStream.CopyToAsync(bzipOutputStream);
                    }
                }
            }

            File.Delete(temporaryFilePath);
            return fullFileName;
        }

        public async Task<string> MoveTemporaryLogFilesToStore(string temporaryFilePath, string subdirectoryName, string outputFileName)
        {
            string fullFileName = $"{outputFileName}.zip";
            string logFilesPath = Path.Combine(_storageOptions.ResultStoragePath, subdirectoryName, fullFileName);

            using (FileStream tempFileStream = File.OpenRead(temporaryFilePath))
            {
                using (FileStream logFilesStream = File.OpenWrite(logFilesPath))
                {
                    await tempFileStream.CopyToAsync(logFilesStream);
                }
            }

            File.Delete(temporaryFilePath);
            return fullFileName;
        }

        public void DeleteStore(string subdirectoryName)
        {
            string fullStorePath = Path.Combine(_storageOptions.ResultStoragePath, subdirectoryName);

            if (Directory.Exists(fullStorePath))
            {
                Directory.Delete(fullStorePath, true);
            }
            else
            {
                throw new ArgumentException($"Cannot delete directory '{fullStorePath}' because it does not exist!");
            }
        }

        public string CreateNewRandomStorageDirectory(string storagePath)
        {
            string name = Path.GetRandomFileName();
            Directory.CreateDirectory(Path.Combine(storagePath, name));
            return name;
        }

        public Stream OpenFileReader(string subdirectoryName, string fileName, bool decompress)
        {
            string fullFilePath = Path.Combine(_storageOptions.ResultStoragePath, subdirectoryName, fileName);

            if (!File.Exists(fullFilePath))
            {
                throw new ArgumentException($"Cannot open file '{subdirectoryName}/{fileName}' for reading because it does not exist.");
            }

            FileStream resultStream = File.OpenRead(fullFilePath);

            if (decompress)
            {
                BZip2InputStream bzipInputStream = new BZip2InputStream(resultStream);
                bzipInputStream.IsStreamOwner = true;
                return bzipInputStream;
            }

            return resultStream;
        }

        public bool FileExists(string subdirectoryName, string fileName)
        {
            string fullFilePath = Path.Combine(_storageOptions.ResultStoragePath, subdirectoryName, fileName);
            return File.Exists(fullFilePath);
        }
    }
}
