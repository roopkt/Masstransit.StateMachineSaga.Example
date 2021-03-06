﻿using System;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Saga;
using MassTransit.TestFramework.Fixtures;
using MassTransit.Transports.Loopback;
using StateMachineSaga.Auctions;

namespace StateMachineSaga.Tests
{
    public class AuctionSagaTestFixture : EndpointTestFixture<LoopbackTransportFactory>
    {
        public InMemorySagaRepository<AuctionSaga> Repository { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid AuctionId { get; set; }

        public IServiceBus LocalBus { get; private set; }
        public IServiceBus MockLocalBus { get; private set; }
        private const string URI = "loopback://localhost/auction_client";
        public AuctionSagaTestFixture()
        {
            LocalBus = ServiceBusFactory.New(x =>
            {
                x.ReceiveFrom(URI);

                ConfigureLocalBus(x);
            });

            Repository = SetupSagaRepository<AuctionSaga>();

            LocalBus.SubscribeSaga(Repository);
        }

        private void ConfigureLocalBus(ServiceBusConfigurator serviceBusConfigurator)
        {

        }
    }
}