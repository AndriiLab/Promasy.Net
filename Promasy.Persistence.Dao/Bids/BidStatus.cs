using Promasy.Persistence.Dao.Commons;

namespace Promasy.Persistence.Dao.Bids
{
    public class BidStatus : Entity
    {
        public string Status { get; set; }

        public long BidId { get; set; }
        public Bid Bid { get; set; }
    }
}