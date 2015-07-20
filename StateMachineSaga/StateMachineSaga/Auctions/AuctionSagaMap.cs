using MassTransit;
using MassTransit.NHibernateIntegration;

namespace StateMachineSaga.Auctions
{
    public class AuctionSagaMap : SagaClassMapping<AuctionSaga>
    {
        public AuctionSagaMap()
        {
            this.StateProperty(x => x.CurrentState);
            Property(x=>x.AuctionId);
            Property(x => x.Title);
            Property(x => x.OpenBid);
            Property(x => x.MaximumBid);
        }
    }
}