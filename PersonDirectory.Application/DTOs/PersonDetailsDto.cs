using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.DTOs
{
    public class PersonDetailDto : PersonDto
    {
        public int Id { get; set; }
        public List<RelatedPersonDto> RelatedPersons { get; set; } = new();
    }
}
