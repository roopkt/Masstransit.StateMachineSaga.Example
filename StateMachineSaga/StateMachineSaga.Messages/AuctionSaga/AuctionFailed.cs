using System;

namespace StateMachineSaga.Messages.AuctionSaga
{
    public class AuctionFailed
    {
        public Guid AuctionId { get; set; }
    }
}