using BenchStoreBL.Exceptions;
using BenchStoreBL.Models;
using BenchStoreBL.Models.Mappers;

using BenchStoreDAL.Data;

using Microsoft.EntityFrameworkCore;

using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Services.Results
{
    internal class ResultsService : IResultsService
    {
        private readonly BenchStoreContext _context;

        public ResultsService(BenchStoreContext context)
        {
            _context = context;
        }

        public async Task<Result?> GetResultByID(int id)
        {
            if (_context.Result == null)
            {
                throw ExceptionThrowerHelper.ThrowWhenDbSetIsNull<Entities.Result>();
            }

            Entities.Result? resultEntity = await _context.Result.FindAsync(id);
            return resultEntity?.MapToModel(true);
        }

        public async Task<Result?> GetResultByResultEntryID(int resultEntryID)
        {
            if (_context.ResultEntry == null)
            {
                throw ExceptionThrowerHelper.ThrowWhenDbSetIsNull<Entities.Result>();
            }

            Entities.ResultEntry? resultEntryEntity = await _context
                .ResultEntry
                .Include(re => re.Result)
                .FirstOrDefaultAsync(re => re.ID == resultEntryID);

            return resultEntryEntity?.Result.MapToModel(true);
        }

        public string GetResultName(Result result)
        {
            return $"{result.BenchmarkName}.{result.Date:yyyy-MM-dd_HH-mm-ss}.results.{result.Name}";
        }

        public string GetLogFilesName(Result result)
        {
            return $"{result.BenchmarkName}.{result.Date:yyyy-MM-dd_HH-mm-ss}.logfiles";
        }

    }
}

