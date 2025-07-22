using PersonDirectory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
