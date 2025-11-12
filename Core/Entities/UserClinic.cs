namespace Core.Entities
{
    public class UserClinic
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; }
    }
}
