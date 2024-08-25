using ICSharpCode.SharpZipLib.BZip2;

namespace BenchStoreBL.Services.ResultStoring
{
    internal class XMLFileStoring : IFileStoring
    {
        public async Task<string> StoreTemporaryFile(Stream inputStream)
        {
            string tempFilePath = Path.GetTempFileName();
            using (var tempFileStream = File.Create(tempFilePath))
            {
                await inputStream.CopyToAsync(tempFileStream);
            }

            return tempFilePath;
        }

        public async Task<string> StoreFile(Stream inputStream, string storagePath, string fileName)
        {
            string fullFileName = $"{fileName}.xml.bz2";
            string xmlFilePath = Path.Combine(storagePath, fullFileName);

            using (FileStream xmlFileStream = File.OpenWrite(xmlFilePath))
            {
                using (BZip2OutputStream bzipOutputStream = new BZip2OutputStream(xmlFileStream))
                {
                    await inputStream.CopyToAsync(bzipOutputStream);
                }
            }

            return fullFileName;
        }

        public async Task<string> StoreLogFiles(Stream inputStream, string storagePath, string fileName)
        {
            string fullFileName = $"{fileName}.zip";
            string logFilesPath = Path.Combine(storagePath, fullFileName);

            using (FileStream logFilesStream = File.OpenWrite(logFilesPath))
            {
                await inputStream.CopyToAsync(logFilesStream);
            }

            return fullFileName;
        }

        public void DeleteFile(string fileName, string storagePath)
        {
            string filePath = Path.Combine(storagePath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                throw new ArgumentException($"Cannot delete file '{filePath}' because it does not exist!");
            }
        }
    }
}
