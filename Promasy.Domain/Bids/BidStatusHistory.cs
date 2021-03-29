namespace Promasy.Domain.Bids
{
    public class BidStatusHistory : Entity
    {
        public BidStatus Status { get; set; }

        public int BidId { get; set; }
        public Bid Bid { get; set; }
    }
}