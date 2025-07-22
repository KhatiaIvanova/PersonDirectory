using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.DTOs
{
    public class RelatedPersonDto
    {
        public int RelatedPersonId { get; set; }
        public string RelationType { get; set; } = default!;
    }
}
