using System;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Log4NetIntegration;
using MassTransit.NHibernateIntegration;
using MassTransit.NHibernateIntegration.Saga;
using StateMachineSaga.Auctions;

namespace StateMachineSaga
{
    public class EndpointConfiguration
    {
        public IServiceBus Bus;
        public void Start()
        {
            Configure();
        }

        public void Stop()
        {
            Bus.Dispose();
        }
        public void Configure()
        {
            
        }

        private static Action<ServiceBusConfigurator> Config()
        {
            return sbc =>
            {
                sbc.UseLog4Net();
                sbc.UseRabbitMq();
                sbc.ReceiveFrom("rabbitmq://localhost/greeting_saga");

               

                var sessionFactory =
                    new SqlServerSessionFactoryProvider(
                        @"Data Source=.\SQLExpress;Initial Catalog=auctiondb;Integrated Security=True;Enlist=false",
                        new[] {typeof (AuctionSagaMap)}).GetSessionFactory();


                sbc.Subscribe(
                    subs => subs.Saga(new NHibernateSagaRepository<AuctionSaga>(sessionFactory)));
            };
        }

        
    }
}
