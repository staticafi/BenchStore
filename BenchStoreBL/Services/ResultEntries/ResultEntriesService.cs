using BenchStoreBL.Exceptions;
using BenchStoreBL.Models;
using BenchStoreBL.Models.Mappers;

using BenchStoreDAL.Data;

using Microsoft.EntityFrameworkCore;

using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Services.ResultEntries
{
    internal class ResultEntriesService : IResultEntriesService
    {
        private readonly BenchStoreContext _context;

        public ResultEntriesService(BenchStoreContext context)
        {
            _context = context;

            if (_context.ResultEntry == null)
            {
                throw ExceptionThrowerHelper.ThrowWhenDbSetIsNull<Entities.ResultEntry>();
            }
        }

        public async Task<List<ResultEntry>> GetResultEntries(OrderResultEntryBy orderResultEntryBy, ResultEntriesFilter? filter = null)
        {
            IQueryable<Entities.ResultEntry> query = _context
                .ResultEntry
                .AsNoTracking()
                .Include(re => re.Result)
                .Include(re => re.Labels.OrderBy(l => l.Name));

            if (filter != null )
            {
                query = FilterResultEntries(query, filter);
            }

            query = OrderResultEntries(query, orderResultEntryBy);

            return await query
                .Select(re => re.MapToModel(true))
                .ToListAsync();
        }

        private IQueryable<Entities.ResultEntry> FilterResultEntries(IQueryable<Entities.ResultEntry> query, ResultEntriesFilter filter)
        {
            query = FilterByBenchmarkName(query, filter);

            query = FilterByName(query, filter);

            query = FilterByTool(query, filter);

            query = FilterByOwnerName(query, filter);

            query = FilterByDescription(query, filter);

            query = FilterByLabelNames(query, filter);

            return query;
        }

        private static IQueryable<Entities.ResultEntry> FilterByLabelNames(IQueryable<Entities.ResultEntry> query, ResultEntriesFilter filter)
        {
            if (filter.LabelNames != null)
            {
                query = query
                    .Where(re => re
                        .Labels!
                        .Count(l => filter.LabelNames.Contains(l.Name))
                    .Equals(filter.LabelNames.Count()));
            }

            return query;
        }
        private IQueryable<Entities.ResultEntry> FilterByDescription(IQueryable<Entities.ResultEntry> query, ResultEntriesFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Description))
            {
                query = query
                    .Where(re => re.Description != null && re.Description.Contains(filter.Description));
            }

            return query;
        }

        private static IQueryable<Entities.ResultEntry> FilterByOwnerName(IQueryable<Entities.ResultEntry> query, ResultEntriesFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.OwnerName))
            {
                query = query
                    .Where(re => re.OwnerName != null && re.OwnerName.Contains(filter.OwnerName));
            }

            return query;
        }

        private IQueryable<Entities.ResultEntry> FilterByTool(IQueryable<Entities.ResultEntry> query, ResultEntriesFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Tool))
            {
                query = query
                    .Where(re => re.Result.Tool == filter.Tool);
            }

            return query;
        }

        private IQueryable<Entities.ResultEntry> FilterByName(IQueryable<Entities.ResultEntry> query, ResultEntriesFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query
                    .Where(re => re.Result.Name != null && re.Result.Name.Contains(filter.Name));
            }

            return query;
        }

        private IQueryable<Entities.ResultEntry> FilterByBenchmarkName(IQueryable<Entities.ResultEntry> query, ResultEntriesFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.BenchmarkName))
            {
                query = query
                    .Where(re => re.Result.BenchmarkName != null && re.Result.BenchmarkName.Contains(filter.BenchmarkName));
            }

            return query;
        }

        private IQueryable<Entities.ResultEntry> OrderResultEntries(IQueryable<Entities.ResultEntry> query, OrderResultEntryBy orderResultEntryBy)
        {
            switch (orderResultEntryBy)
            {
                case OrderResultEntryBy.Name:
                    return query
                        .OrderBy(re => re.Result.Name);
                case OrderResultEntryBy.OwnerName:
                    return query
                        .OrderBy(re => re.OwnerName);
                case OrderResultEntryBy.BenchmarkName:
                    return query
                        .OrderBy(re => re.Result.BenchmarkName);
                case OrderResultEntryBy.Tool:
                    return query
                        .OrderBy(re => re.Result.Tool);
                case OrderResultEntryBy.Date:
                    return query
                        .OrderBy(re => re.Result.Date);
                default:
                    throw new ArgumentException($"Unknown OrderResultEntryBy option: '{orderResultEntryBy}'");
            }
        }

        public async Task<ResultEntry?> GetResultEntryByID(int id)
        {
            Entities.ResultEntry? resultEntryEntity = await _context
                .ResultEntry
                .AsNoTracking()
                .Include(sr => sr.Result)
                .FirstOrDefaultAsync(sr => sr.ID == id);

            return resultEntryEntity?.MapToModel(true);
        }

        public async Task<int> CreateResultEntry(ResultEntry resultEntry, Result result)
        {
            Entities.ResultEntry resultEntryEntity = resultEntry.MapToEntity(false);
            resultEntryEntity.LastAccessTime = DateTime.UtcNow;

            Entities.Result resultEntity = result.MapToEntity(false);

            resultEntryEntity.Result = resultEntity;
            _context.ResultEntry.Add(resultEntryEntity);
            await _context.SaveChangesAsync();

            return resultEntryEntity.ID;
        }

        public async Task EditResultEntry(ResultEntry resultEntry)
        {
            Entities.ResultEntry resultEntryEntity = resultEntry.MapToEntity(true);
            
            try
            {
                _context.ResultEntry.Update(resultEntryEntity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ResultEntryExists(resultEntry.ID))
                {
                    throw ExceptionThrowerHelper.ThrowWhenEntityWithIDDoesNotExist<Entities.ResultEntry>(resultEntry.ID);
                }
                throw;
            }
        }

        public async Task DeleteResultEntry(int id)
        {
            Entities.ResultEntry? resultEntryEntity = await _context.ResultEntry.FindAsync(id);

            if (resultEntryEntity != null)
            {
                _context.ResultEntry.Remove(resultEntryEntity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task EditResultEntryLabels(int resultEntryID, IEnumerable<Label> labels)
        {
            Entities.ResultEntry? resultEntryEntity = await _context
                .ResultEntry
                .Include(re => re.Labels)
                .FirstOrDefaultAsync(re => re.ID == resultEntryID);

            if (resultEntryEntity == null)
            {
                throw ExceptionThrowerHelper.ThrowWhenEntityWithIDDoesNotExist<Entities.ResultEntry>(resultEntryID);
            }

            if (resultEntryEntity.Labels == null)
            {
                resultEntryEntity.Labels = new List<Entities.Label>();
            }

            List<Entities.Label> labelEntities = new List<Entities.Label>();
            foreach (var label in labels)
            {
                Entities.Label? labelEntity = await _context.Label.FindAsync(label.ID);
                if (labelEntity == null)
                {
                    throw ExceptionThrowerHelper.ThrowWhenEntityWithIDDoesNotExist<Entities.Label>(label.ID);
                }

                labelEntities.Add(labelEntity);
            }

            resultEntryEntity.Labels = labelEntities;

            try
            {
                _context.ResultEntry.Update(resultEntryEntity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ResultEntryExists(resultEntryEntity.ID))
                {
                    ExceptionThrowerHelper.ThrowWhenEntityWithIDDoesNotExist<Entities.ResultEntry>(resultEntryEntity.ID);
                }
                throw;
            }
        }

        private async Task<bool> ResultEntryExists(int id)
        {
            if (_context.ResultEntry == null)
            {
                return false;
            }

            return await _context.ResultEntry.AnyAsync(re => re.ID == id);
        }
    }
}

