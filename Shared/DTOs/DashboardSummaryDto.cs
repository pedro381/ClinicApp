namespace Shared.DTOs
{
    public class DashboardSummaryDto
    {
        public List<ClinicSummaryDto> Clinics { get; set; } = new();
        public int TotalClinics => Clinics.Count;
        public int TotalMaterials => Clinics.Sum(c => c.DistinctMaterials);
        public int TotalQuantity => Clinics.Sum(c => c.TotalQuantity);
    }
}
