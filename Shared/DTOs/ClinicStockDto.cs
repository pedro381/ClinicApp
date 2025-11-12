namespace Shared.DTOs
{
    public record ClinicStockDto(Guid MaterialId, string MaterialName, int QuantityAvailable, string Category, bool IsOpen, DateTime? OpenedAt);

}
