using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Application.DTOs
{
    public class RelatedPersonDto
    {
        public int RelatedPersonId { get; set; }
        public RelationType RelationType { get; set; } = RelationType.ნათესავი!;
    }
}
