using Promasy.Persistence.Dao.Common;

namespace Promasy.Persistence.Dao.Commons
{
    public class BaseEntity : IEntity
    {
        public long Id { get; set; }
    }
}