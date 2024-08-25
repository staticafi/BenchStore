using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Models.Mappers
{
    public static class ResultEntryMapper
    {
        public static ResultEntry MapToModel(this Entities.ResultEntry storedResultEntity, bool mapId)
        {
            return mapId
                ? new ResultEntry
                {
                    ID = storedResultEntity.ID,
                    OwnerName = storedResultEntity.OwnerName,
                    Description = storedResultEntity.Description,
                    LastAccessTime = storedResultEntity.LastAccessTime,
                    ResultSubdirectoryName = storedResultEntity.ResultSubdirectoryName,
                    ResultFileName = storedResultEntity.ResultFileName,
                    LogFilesName = storedResultEntity.LogFilesName,
                    Result = storedResultEntity.Result?.MapToModel(mapId),
                    Labels = storedResultEntity.Labels?.Select(l => l.MapToModel(mapId)),
                }
                : new ResultEntry
                {
                    OwnerName = storedResultEntity.OwnerName,
                    Description = storedResultEntity.Description,
                    LastAccessTime = storedResultEntity.LastAccessTime,
                    ResultSubdirectoryName = storedResultEntity.ResultSubdirectoryName,
                    ResultFileName = storedResultEntity.ResultFileName,
                    LogFilesName = storedResultEntity.LogFilesName,
                    Result = storedResultEntity.Result?.MapToModel(mapId),
                    Labels = storedResultEntity.Labels?.Select(l => l.MapToModel(mapId)),
                };
        }

        public static Entities.ResultEntry MapToEntity(this ResultEntry storedResult, bool mapId)
        {
            return mapId
                ? new Entities.ResultEntry
                {
                    ID = storedResult.ID,
                    OwnerName = storedResult.OwnerName,
                    Description = storedResult.Description,
                    LastAccessTime = storedResult.LastAccessTime,
                    ResultSubdirectoryName = storedResult.ResultSubdirectoryName,
                    ResultFileName = storedResult.ResultFileName,
                    LogFilesName = storedResult.LogFilesName,
                }
                : new Entities.ResultEntry
                {
                    OwnerName = storedResult.OwnerName,
                    Description = storedResult.Description,
                    LastAccessTime = storedResult.LastAccessTime,
                    ResultSubdirectoryName = storedResult.ResultSubdirectoryName,
                    ResultFileName = storedResult.ResultFileName,
                    LogFilesName = storedResult.LogFilesName,
                };
        }
    }
}

