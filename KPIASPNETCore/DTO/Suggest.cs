using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPIASPNETCore.DTO
{
    public class Suggest
    {
        public int Id { get; set; }
        public string Suggestion { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }

    }
}
