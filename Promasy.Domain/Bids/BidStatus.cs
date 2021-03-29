namespace Promasy.Domain.Bids
{
    public enum BidStatus
    {
        Created = 1,
        Submitted = 2,
        PostedToAuction = 3,
        Received = 4,
        NotReceived = 10,
        Declined = 20
    }
}