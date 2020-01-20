using Promasy.Persistence.Dao.Common;

namespace Promasy.Persistence.Dao.Vocabulary
{
    public class Cpv : IEntity
    {
        public string CpvCode { get; set; }
        public string CpvEng { get; set; }
        public string CpvUkr { get; set; }
        public int CpvLevel { get; set; }
        public bool Terminal { get; set; }
    }
}