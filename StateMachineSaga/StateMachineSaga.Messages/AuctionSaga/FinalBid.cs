using System;

namespace StateMachineSaga.Messages.AuctionSaga
{
    public class FinalBid
    {
        public decimal MaximumBid { get; set; }
        public Guid AuctionId { get; set; }
    }
}