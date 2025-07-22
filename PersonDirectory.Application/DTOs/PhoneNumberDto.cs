using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.DTOs
{
    public class PhoneNumberDto
    {
        public string Type { get; set; } = default!;
        public string Number { get; set; } = default!;
    }
}
