using PersonDirectory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.DTOs
{
    public class PhoneNumberDto
    {
        public PhoneType Type { get; set; } = PhoneType.მობილური!;
        public string Number { get; set; } = default!;
    }
}
