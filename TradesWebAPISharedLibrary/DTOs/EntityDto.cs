namespace TradesWebAPISharedLibrary.DTOs
{
    public class EntityDto : IEntity
    {
        public List<AddressDto>? Addresses { get; set; }
        public List<DatesDto> Dates { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }
        public string Id { get; set; }
        public List<NameDto> Names { get; set; }
    }
}
