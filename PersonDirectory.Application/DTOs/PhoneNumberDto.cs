using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Application.DTOs
{
    public class PhoneNumberDto
    {
        public PhoneType Type { get; set; } = PhoneType.მობილური!;
        public string Number { get; set; } = default!;
    }
}
