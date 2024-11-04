using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Models.Mappers
{
    public static class ResultEntryMapper
    {
        public static ResultEntry MapToModel(this Entities.ResultEntry resultEntryEntity, bool mapId)
        {
            return mapId
                ? new ResultEntry
                {
                    ID = resultEntryEntity.ID,
                    OwnerName = resultEntryEntity.OwnerName,
                    Description = resultEntryEntity.Description,
                    LastAccessTime = resultEntryEntity.LastAccessTime,
                    ResultSubdirectoryName = resultEntryEntity.ResultSubdirectoryName,
                    ResultFileName = resultEntryEntity.ResultFileName,
                    LogFilesName = resultEntryEntity.LogFilesName,
                    Result = resultEntryEntity.Result?.MapToModel(mapId),
                    Labels = resultEntryEntity.Labels?.Select(l => l.MapToModel(mapId)),
                }
                : new ResultEntry
                {
                    OwnerName = resultEntryEntity.OwnerName,
                    Description = resultEntryEntity.Description,
                    LastAccessTime = resultEntryEntity.LastAccessTime,
                    ResultSubdirectoryName = resultEntryEntity.ResultSubdirectoryName,
                    ResultFileName = resultEntryEntity.ResultFileName,
                    LogFilesName = resultEntryEntity.LogFilesName,
                    Result = resultEntryEntity.Result?.MapToModel(mapId),
                    Labels = resultEntryEntity.Labels?.Select(l => l.MapToModel(mapId)),
                };
        }

        public static Entities.ResultEntry MapToEntity(this ResultEntry resultEntry, bool mapId)
        {
            return mapId
                ? new Entities.ResultEntry
                {
                    ID = resultEntry.ID,
                    OwnerName = resultEntry.OwnerName,
                    Description = resultEntry.Description,
                    LastAccessTime = resultEntry.LastAccessTime,
                    ResultSubdirectoryName = resultEntry.ResultSubdirectoryName,
                    ResultFileName = resultEntry.ResultFileName,
                    LogFilesName = resultEntry.LogFilesName,
                }
                : new Entities.ResultEntry
                {
                    OwnerName = resultEntry.OwnerName,
                    Description = resultEntry.Description,
                    LastAccessTime = resultEntry.LastAccessTime,
                    ResultSubdirectoryName = resultEntry.ResultSubdirectoryName,
                    ResultFileName = resultEntry.ResultFileName,
                    LogFilesName = resultEntry.LogFilesName,
                };
        }
    }
}

