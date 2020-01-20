using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;

namespace Promasy.Persistence.Dao.Internals
{
    public class Version : BaseEntity
    {
        public string AllowedVersion { get; set; }
    }
}
