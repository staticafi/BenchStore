using Microsoft.EntityFrameworkCore;

using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Exceptions
{
    public static class ExceptionThrowerHelper
    {
        public static ArgumentException ThrowWhenEntityWithIDDoesNotExist<TEntity>(int id) where TEntity : class, Entities.IEntity
        {
            return new ArgumentException($"{typeof(TEntity).Name} with {nameof(Entities.IEntity.ID)}: '{id}' does not exist!");
        }

        public static InvalidOperationException ThrowWhenDbSetIsNull<TEntity>() where TEntity : class, Entities.IEntity
        {
            return new InvalidOperationException($"{nameof(DbSet<TEntity>)} '{typeof(TEntity).Name}' is null!");
        }

        public static ArgumentException ThrowWhenDuplicateEntityKey<TEntity>(string keyName, string key) where TEntity : class, Entities.IEntity
        {
            return new ArgumentException($"{typeof(TEntity).Name} with {keyName}: '{key}' already exists!");
        }
    }
}

