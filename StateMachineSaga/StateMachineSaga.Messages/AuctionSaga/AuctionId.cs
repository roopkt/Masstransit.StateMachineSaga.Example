namespace StateMachineSaga.Messages.AuctionSaga
{
    public class AuctionId  
    {
        protected AuctionId()
        {
            
        }
        public string InternalId { get; set; }

        public AuctionId(string internalId)
        {
            InternalId=internalId;
        }

        
    }
}