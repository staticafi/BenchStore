using Entities = BenchStoreDAL.Entities;

namespace BenchStoreBL.Models.Mappers
{
    public static class LabelMapper
    {
        public static Label MapToModel(this Entities.Label labelEntity, bool mapId)
        {
            return mapId
            ? new Label
            {
                ID = labelEntity.ID,
                Name = labelEntity.Name,
                Color = labelEntity.Color,
            }
            : new Label
            {
                Name = labelEntity.Name,
                Color = labelEntity.Color,
            };
        }

        public static Entities.Label MapToEntity(this Label label, bool mapId)
        {
            return mapId
                ? new Entities.Label
                {
                    ID = label.ID,
                    Name = label.Name,
                    Color = label.Color,
                }
                : new Entities.Label
                {
                    Name = label.Name,
                    Color = label.Color,
                };
        }

    }
}

