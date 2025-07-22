namespace PersonDirectory.Application.DTOs
{
    public class PersonDetailDto : PersonDto
    {
        public int Id { get; set; }
        public List<RelatedPersonDto> RelatedPersons { get; set; } = new();
    }
}
