using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Domain.Entities
{
    public class RelatedPerson
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int RelatedToPersonId { get; set; }
        public RelationType RelationType { get; set; } = RelationType.ნათესავი;
    }
}
