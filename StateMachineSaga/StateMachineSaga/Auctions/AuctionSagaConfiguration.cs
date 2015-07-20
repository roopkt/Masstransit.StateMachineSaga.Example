using System;
using MassTransit;
using MassTransit.Log4NetIntegration;
using MassTransit.Saga;

namespace StateMachineSaga.Auctions
{
    public class AuctionSagaConfiguration 
    {
        public IServiceBus Bus;
        public void Configure()
        {

            try
            {
                Bus = ServiceBusFactory.New(sbc =>
                {
                    sbc.UseLog4Net();
                    sbc.UseRabbitMq();
                    sbc.ReceiveFrom("rabbitmq://localhost/greeting_saga");

                    sbc.Subscribe(subs => subs.Saga(new InMemorySagaRepository<AuctionSaga>()));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
            Bus.Dispose();
        }
    }
}