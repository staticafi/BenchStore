namespace BenchStoreBL.Services.ResultStoring
{
    public interface IFileStoring
    {
        public Task<string> StoreTemporaryFile(Stream inputStream);
        public Task<string> StoreFile(Stream inputStream, string storagePath);
        public Task<string> StoreLogFiles(Stream inputStream, string storagePath);
        public void DeleteFile(string fileName, string storagePath);
    }
}
