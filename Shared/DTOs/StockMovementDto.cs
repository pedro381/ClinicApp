namespace Shared.DTOs
{
    public record StockMovementDto(Guid Id, Guid ClinicId, Guid MaterialId, int Quantity, string MovementType, string PerformedBy, DateTime CreatedAt, string Note);

}
