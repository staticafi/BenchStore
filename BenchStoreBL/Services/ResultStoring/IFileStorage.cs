namespace BenchStoreBL.Services.ResultStoring
{
    public interface IFileStorage
    {
        public Task<string> StoreTemporaryFile(Stream inputStream);

        public Task<string> MoveTemporaryFileToStore(string temporaryFilePath, string subdirectoryName, string outputFileName);

        public Task<string> MoveTemporaryLogFilesToStore(string temporaryFilePath, string subdirectoryName, string outputFileName);

        public void DeleteStore(string subdirectoryName);

        public string CreateNewRandomStorageDirectory(string storagePath);

        public Stream OpenFileReader(string subdirectoryName, string fileName, bool decompress);

        public bool FileExists(string subdirectoryName, string fileName);
    }
}
