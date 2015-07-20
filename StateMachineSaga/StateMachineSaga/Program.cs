using Topshelf;

namespace StateMachineSaga
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(c =>
            {
                c.Service<EndpointConfiguration>(s =>
                {
                    s.ConstructUsing(name => new EndpointConfiguration());
                    s.WhenStarted(o => o.Start());
                    s.WhenStopped(o => o.Stop());
                });

                c.SetServiceName("StateMachineSagaEndpoint");
                c.SetDisplayName("AuctionSaga Endpoint Demo");
                c.SetDescription("An Auction Saga");
                c.RunAsLocalService();
            });
        }
    }
}
