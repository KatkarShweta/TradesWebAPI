namespace TradesWebAPISharedLibrary.Model
{
    public interface IEntity
    {
        public List<Address>? Addresses { get; set; }
        public List<Dates> Dates { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }
        public string Id { get; set; }
        public List<Name> Names { get; set; }
    }
}
