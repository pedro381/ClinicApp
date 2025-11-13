namespace Shared.DTOs
{
    public record StockMovementDto(Guid Id, Guid ClinicId, Guid MaterialId, string MaterialName, int Quantity, string MovementType, string PerformedBy, DateTime CreatedAt, string Note);

}
