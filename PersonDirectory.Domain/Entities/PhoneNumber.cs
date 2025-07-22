using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Domain.Entities
{
    public class PhoneNumber
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public PhoneType Type { get; set; } = PhoneType.მობილური;
        public string Number { get; set; } = string.Empty;
    }
}
