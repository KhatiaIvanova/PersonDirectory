using PersonDirectory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.DTOs
{
    public class PersonDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public  Gender  Gender { get; set; } = default!;
        public string PersonalNumber { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();
    }
}
