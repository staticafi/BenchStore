using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Models.Mappers
{
    public static class ResultMapper
    {
        public static Result MapToModel(this Entities.Result resultEntity, bool mapId)
        {
            return mapId
                ? new Result
                {
                    ID = resultEntity.ID,
                    Name = resultEntity.Name,
                    BenchmarkName = resultEntity.BenchmarkName,
                    Block = resultEntity.Block,
                    CPUCores = resultEntity.CPUCores,
                    DisplayName = resultEntity.DisplayName,
                    Date = resultEntity.Date,
                    StartTime = resultEntity.StartTime,
                    EndTime = resultEntity.EndTime,
                    Error = resultEntity.Error,
                    Generator = resultEntity.Generator,
                    MemLimit = resultEntity.MemLimit,
                    Options = resultEntity.Options,
                    TimeLimit = resultEntity.TimeLimit,
                    Tool = resultEntity.Tool,
                    ToolModule = resultEntity.ToolModule,
                    Version = resultEntity.Version,
                }
                : new Result
                {
                    Name = resultEntity.Name,
                    BenchmarkName = resultEntity.BenchmarkName,
                    Block = resultEntity.Block,
                    CPUCores = resultEntity.CPUCores,
                    DisplayName = resultEntity.DisplayName,
                    Date = resultEntity.Date,
                    StartTime = resultEntity.StartTime,
                    EndTime = resultEntity.EndTime,
                    Error = resultEntity.Error,
                    Generator = resultEntity.Generator,
                    MemLimit = resultEntity.MemLimit,
                    Options = resultEntity.Options,
                    TimeLimit = resultEntity.TimeLimit,
                    Tool = resultEntity.Tool,
                    ToolModule = resultEntity.ToolModule,
                    Version = resultEntity.Version,
                };
        }

        public static Entities.Result MapToEntity(this Result result, bool mapId)
        {
            DateTime startTime = DateTime.SpecifyKind(result.StartTime, DateTimeKind.Utc);
            DateTime endTime = DateTime.SpecifyKind(result.EndTime, DateTimeKind.Utc);
            DateTime date = DateTime.SpecifyKind(result.Date.UtcDateTime, DateTimeKind.Utc);

            return mapId
                ? new Entities.Result
                {
                    ID = result.ID,
                    Name = result.Name,
                    BenchmarkName = result.BenchmarkName,
                    Block = result.Block,
                    CPUCores = result.CPUCores,
                    DisplayName = result.DisplayName,
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    Error = result.Error,
                    Generator = result.Generator,
                    MemLimit = result.MemLimit,
                    Options = result.Options,
                    TimeLimit = result.TimeLimit,
                    Tool = result.Tool,
                    ToolModule = result.ToolModule,
                    Version = result.Version,
                }
                : new Entities.Result
                {
                    Name = result.Name,
                    BenchmarkName = result.BenchmarkName,
                    Block = result.Block,
                    CPUCores = result.CPUCores,
                    DisplayName = result.DisplayName,
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    Error = result.Error,
                    Generator = result.Generator,
                    MemLimit = result.MemLimit,
                    Options = result.Options,
                    TimeLimit = result.TimeLimit,
                    Tool = result.Tool,
                    ToolModule = result.ToolModule,
                    Version = result.Version,
                };
        }
    }
}

