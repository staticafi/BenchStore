namespace BenchStoreBL.Services.ResultStoring
{
    public interface IFileStoring
    {
        public Task<string> StoreTemporaryFile(Stream inputStream);
        public Task<string> StoreFile(Stream inputStream, string storagePath, string fileName);
        public Task<string> StoreLogFiles(Stream inputStream, string storagePath, string fileName);
        public void DeleteFile(string fileName, string storagePath);
    }
}
