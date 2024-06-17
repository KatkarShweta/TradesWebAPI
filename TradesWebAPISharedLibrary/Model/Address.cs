namespace TradesWebAPISharedLibrary.Model
{
    public class Address
    {        
        public int Id { get; set; }        
        public string EntityId { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
