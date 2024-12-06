using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Models
{
    public class Image
    {
        public int Id { get; set; }

        public Project Project { get; set; }

        public Event Event { get; set; }

        public string Link { get; set; }
    }
}
