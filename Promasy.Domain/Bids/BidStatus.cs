using Promasy.Common.Persistence;

namespace Promasy.Domain.Bids
{
    public class BidStatus : Entity
    {
        public string Status { get; set; }

        public int BidId { get; set; }
        public Bid Bid { get; set; }
    }
}