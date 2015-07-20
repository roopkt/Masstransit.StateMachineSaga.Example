using System;
using MassTransit;

namespace StateMachineSaga.Messages.AuctionSaga
{
    public class CreateAuction :CorrelatedBy<Guid>
    {
        
        public string Title { get; set; }
        public string OwnerEmail { get; set; }
        public decimal OpeningBid { get; set; }
        public Guid CorrelationId { get;  set; }
        public Guid AuctionId{ get; set; }
    }
}