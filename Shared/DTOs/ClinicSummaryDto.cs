using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class ClinicSummaryDto
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = "";
        public int DistinctMaterials { get; set; }
        public int TotalQuantity { get; set; }
    }
}
