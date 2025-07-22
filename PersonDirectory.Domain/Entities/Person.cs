using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Domain.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.ქალი;
        public string PersonalNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; } = new();
        public List<RelatedPerson> RelatedPersons { get; set; } = new();
    }
}
