using System;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using StateMachineSaga.Messages.AuctionSaga;

namespace StateMachineSaga.Auctions
{
    public class AuctionSaga : SagaStateMachine<AuctionSaga>, ISaga
    {
        public   Guid CorrelationId { get; private set; }
        public   Guid AuctionId { get; set; }
        public IServiceBus Bus { get; set; }
        public   string Title { get; set; }
        public static Event<PlaceBid> Bid { get; set; }
        public static Event<CreateAuction> CreateAuction { get; set; }
        public static Event<AuctionFailed> AuctionFailed { get; set; }
        public static Event<RetryBid> RetryBid { get; set; }
        public static Event<FinalBid> FinalBid { get; set; }

        public decimal OpenBid { get; set; }
        public decimal MaximumBid{ get; set; }
        public static State Initial { get; set; }
        public static State Completed { get; set; }
        public static State Open { get; set; }
        public static State BidPlaced { get; set; }
        public static State Faulted { get; set; }
        public static State Closed { get; set; }

        public AuctionSaga()
        {
            
        }
        public AuctionSaga(IServiceBus bus)
        {
            Bus = bus;
        }
        static AuctionSaga()
        {
            try
            {
                Define(() =>
                {
                    Correlate(Bid).By((saga, message) => saga.AuctionId == message.AuctionId);
                    Correlate(AuctionFailed).By((saga, message) => saga.AuctionId == message.AuctionId);
                    Correlate(RetryBid).By((saga, message) => saga.AuctionId == message.AuctionId);
                    Correlate(FinalBid).By((saga, message) => saga.AuctionId == message.AuctionId);

                    Initially(When(CreateAuction).Then((saga, message) => saga.OnCreate(message)).TransitionTo(Open));

                    Anytime(When(AuctionFailed).Then((saga, message) => saga.OnAuctionFailed(message)).TransitionTo(Faulted));

                    During(Open,
                        When(Bid)
                        .Then((saga, message) => saga.OnBid(message)),
                        When(FinalBid)
                        .Then((saga,message)=>saga.OnFinalBid(message)).TransitionTo(Closed));

                    During(Faulted,
                        When(RetryBid)
                            .Then((saga, message) => saga.OnRetryBid(message))
                            .TransitionTo(Open));

                    Combine(Bid, FinalBid).Into(AuctionCompleted, saga => saga.AuctionComplete);

                    Anytime(When(AuctionCompleted).Then((saga,message)=>saga.OnAuctionCompleted(message)).TransitionTo(Completed));
                });
            }
            catch (Exception exception)
            {

               Console.WriteLine(exception);
            }
        }

        private void OnAuctionCompleted(BasicEvent<AuctionSaga> message)
        {
            Console.WriteLine("Auction Completed " + message.Name + " for Auction: " + AuctionId);
        }

        public int AuctionComplete { get; set; }

        public static Event AuctionCompleted { get; set; }

        public void OnFinalBid(FinalBid message)
        {
            Console.WriteLine("Received FinalBid of $"+message.MaximumBid+" message for auction " + message.AuctionId);  
            Bus.Publish(new RequestAuctionClose(message.AuctionId) );
        }

        public void OnRetryBid(RetryBid message)
        {
            Console.WriteLine("Received RetryBid message for auction "+message.AuctionId);
        }

        public void OnAuctionFailed(AuctionFailed message)
        {
            Console.WriteLine("Received AuctionFailed message for auction " + message.AuctionId + " Marked as Faulted");
        }

        public void OnBid(PlaceBid message)
        {
            MaximumBid = message.MaximumBid;
            Console.WriteLine("Current State " + CurrentState.Name);
            Console.WriteLine("Received maximum bid of $" + message.MaximumBid);
        }

        public void OnCreate(CreateAuction message)
        {
            AuctionId = message.AuctionId;
            Title = message.Title;
            OpenBid = message.OpeningBid;
            Console.WriteLine(Title +" Auction Created with opening bid of $" + message.OpeningBid+" for Auction " +message.AuctionId);
            Console.WriteLine("Current State " + CurrentState.Name);
        }

        public AuctionSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        
    }

    public class RequestAuctionClose
    {
        public Guid AuctionId { get; set; }

        public RequestAuctionClose(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }
}
