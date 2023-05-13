using System.Drawing;

using BenchStoreBL.Exceptions;
using BenchStoreBL.Models;
using BenchStoreBL.Models.Mappers;

using BenchStoreDAL.Data;

using Microsoft.EntityFrameworkCore;

using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Services.Labels
{
    internal class LabelsService : ILabelsService
    {
        private readonly BenchStoreContext _context;

        public LabelsService(BenchStoreContext context)
        {
            _context = context;

            if (_context.Label == null)
            {
                throw ExceptionThrowerHelper.ThrowWhenDbSetIsNull<Entities.Label>();
            }

            if (_context.ResultEntry == null)
            {
                throw ExceptionThrowerHelper.ThrowWhenDbSetIsNull<Entities.ResultEntry>();
            }
        }

        public async Task<List<Label>> GetLabels()
        {
            return await _context
                .Label
                .AsNoTracking()
                .OrderBy(l => l.Name)
                .Select(l => l.MapToModel(true))
                .ToListAsync();
        }

        public async Task<List<Label>> GetResultEntryLabels(int resultEntryID)
        {
            Entities.ResultEntry? resultEntryEntity = await _context
                .ResultEntry
                .AsNoTracking()
                .Include(sr => sr.Labels)
                .FirstOrDefaultAsync(sr => sr.ID == resultEntryID);

            if (resultEntryEntity == null)
            {
                ExceptionThrowerHelper.ThrowWhenEntityWithIDDoesNotExist<Entities.ResultEntry>(resultEntryID);
            }

            if (resultEntryEntity.Labels == null)
            {
                return new List<Label>();
            }

            return resultEntryEntity
                .Labels
                .OrderBy(l => l.Name)
                .Select(l => l.MapToModel(true))
                .ToList();
        }

        public async Task<Label?> GetLabelByID(int id)
        {
            Entities.Label? labelEntity = await _context
                .Label
                .FindAsync(id);

            return labelEntity?.MapToModel(true);
        }

        public async Task<Label?> GetLabelByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be empty!");
            }

            Entities.Label? labelEntity = await _context
                .Label
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Name == name);

            return labelEntity?.MapToModel(true);
        }

        public async Task<int> CreateLabel(Label label)
        {
            if (await _context.Label.AnyAsync(l => l.Name == label.Name))
            {
                throw ExceptionThrowerHelper.ThrowWhenDuplicateEntityKey<Entities.Label>(nameof(Entities.Label.Name), label.Name);
            }

            Entities.Label labelEntity = label.MapToEntity(false);

            Random random = new Random();
            Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            labelEntity.Color = ColorTranslator.ToHtml(randomColor);

            _context.Add(labelEntity);
            await _context.SaveChangesAsync();

            return labelEntity.ID;
        }

        public async Task EditLabel(Label label)
        {
            Entities.Label labelEntity = label.MapToEntity(true);

            if (await LabelNameIsDuplicate(label.ID, label.Name))
            {
                throw ExceptionThrowerHelper.ThrowWhenDuplicateEntityKey<Entities.Label>(nameof(Entities.Label.Name), label.Name);
            }

            try
            {
                _context.Label.Update(labelEntity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await LabelExists(labelEntity.ID))
                {
                    throw ExceptionThrowerHelper.ThrowWhenEntityWithIDDoesNotExist<Entities.Label>(labelEntity.ID);
                }
                throw;
            }
        }

        private async Task<bool> LabelNameIsDuplicate(int id, string name)
        {
            return await _context.Label.AnyAsync(l => l.ID != id && l.Name == name);
        }

        private async Task<bool> LabelExists(int id)
        {
            if (_context.Label == null)
            {
                return false;
            }

            return await _context.Label.AnyAsync(l => l.ID == id);
        }

        private async Task<bool> EntityExists<TEntity>(int id) where TEntity : class, Entities.IEntity
        {
            if (_context.Set<TEntity>() == null)
            {
                return false;
            }

            return await _context.Set<TEntity>().AnyAsync(e => e.ID == id);
        }
    }
}

