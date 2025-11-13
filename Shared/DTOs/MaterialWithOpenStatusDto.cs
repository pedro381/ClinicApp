namespace Shared.DTOs;

public record MaterialWithOpenStatusDto(
    Guid Id, 
    string Name, 
    string Category, 
    int Quantity,
    int DistributedQuantity,
    List<ClinicOpenStatusDto> OpenInClinics);

public record ClinicOpenStatusDto(
    Guid ClinicId,
    string ClinicName,
    bool IsOpen,
    DateTime? OpenedAt);

