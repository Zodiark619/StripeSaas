namespace StripeSaas.Models
{
    public class ProcessedEvent
    {
        public int Id { get; set; }
        public string EventId { get; set; }   // evt_...
        public DateTime CreatedAt { get; set; }
    }
}
