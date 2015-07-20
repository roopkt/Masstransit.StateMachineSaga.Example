using MassTransit;
using MassTransit.Pipeline.Inspectors;
using MassTransit.TestFramework;
using StateMachineSaga.Auctions;
using StateMachineSaga.Messages.AuctionSaga;
using Xunit;

namespace StateMachineSaga.Tests
{
    public class AuctionSagaTests : IUseFixture<AuctionSagaTestFixture>
    {
        private AuctionSagaTestFixture _context;

        public void SetFixture(AuctionSagaTestFixture data)
        {
            _context = data;
        }

        [Fact]
        public void AuctionSaga_Created_When_CreateAuction_Received_And_Transitioned_To_OpenState()
        {
            _context.CorrelationId = NewId.NextGuid();
            _context.AuctionId = NewId.NextGuid();


            PipelineViewer.Trace(_context.LocalBus.InboundPipeline);
            PipelineViewer.Trace(_context.LocalBus.OutboundPipeline);


            var message = new CreateAuction()
            {
                CorrelationId = _context.CorrelationId,
                AuctionId = _context.AuctionId
            };
            _context.LocalBus.Publish(message);


            var saga = _context.Repository.ShouldContainSaga(_context.CorrelationId);


            saga.ShouldBeInState(AuctionSaga.Open);
        }


        [Fact]
        public void When_AuctionFailed_Received_Transitioned_To_FaultedState()
        {
            _context.CorrelationId = NewId.NextGuid();
            _context.AuctionId = NewId.NextGuid();

            _context.LocalBus.Publish(new CreateAuction()
            {
                CorrelationId = _context.CorrelationId,
                AuctionId = _context.AuctionId
            });

            var saga = _context.Repository.ShouldContainSaga(_context.CorrelationId);

            _context.LocalBus.Publish(new AuctionFailed()
            {
                AuctionId = _context.AuctionId
            });

            saga.ShouldBeInState(AuctionSaga.Faulted);
        }

        [Fact]
        public void When_FinalBid_Received_Transitioned_To_ClosedState()
        {
            _context.CorrelationId = NewId.NextGuid();
            _context.AuctionId = NewId.NextGuid();

            _context.LocalBus.Publish(new CreateAuction()
            {
                CorrelationId = _context.CorrelationId,
                AuctionId = _context.AuctionId
            });

            var saga = _context.Repository.ShouldContainSaga(_context.CorrelationId);

            _context.LocalBus.Publish(new FinalBid()
            {
                AuctionId = _context.AuctionId
            });

            saga.ShouldBeInState(AuctionSaga.Closed);
        }

    }
}
