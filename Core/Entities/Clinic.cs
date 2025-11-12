namespace Core.Entities
{
    public class Clinic
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public ICollection<UserClinic> UserClinics { get; set; } = new List<UserClinic>();
        public ICollection<ClinicStock> ClinicStocks { get; set; } = new List<ClinicStock>();
    }
}
