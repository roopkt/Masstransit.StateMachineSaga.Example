using System;

namespace StateMachineSaga.Messages.AuctionSaga
{
    public class RetryBid
    {
        public Guid AuctionId { get; set; }
    }
}