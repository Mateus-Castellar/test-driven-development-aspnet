namespace Store.Core.Messages
{
    public abstract class Message
    {
        public string MessageType { get; protected set; } = null!;
        public Guid AggregateId { get; protected set; }

        public Message()
        {
            MessageType = GetType().Name;
        }
    }
}