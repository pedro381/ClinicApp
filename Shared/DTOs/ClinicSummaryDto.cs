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
