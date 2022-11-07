using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.ASPNETCore.Mini.Models
{
    public class ApplicationPartManager
    {
        public List<ApplicationPart> ApplicationParts { get; set; } = new List<ApplicationPart>();
    }

    public class ApplicationPart
    {
        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
