using Infrastructure.Dat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

[ApiController]
[Route("dashboard")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        // por clinica: qtd materiais distintos e soma das quantidades
        var q = await _db.Clinics
            .Select(c => new ClinicSummaryDto
            {
                ClinicId = c.Id,
                ClinicName = c.Name,
                DistinctMaterials = c.ClinicStocks.Count(),
                TotalQuantity = c.ClinicStocks.Sum(s => s.QuantityAvailable)
            }).ToListAsync();

        var dto = new DashboardSummaryDto { Clinics = q };
        return Ok(dto);
    }

    [HttpGet("clinic/{id:guid}")]
    public async Task<IActionResult> ClinicDetails(Guid id)
    {
        var clinic = await _db.Clinics
            .Include(c => c.ClinicStocks)
                .ThenInclude(s => s.Material)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinic == null) return NotFound();

        var stocks = clinic.ClinicStocks
            .Where(s => s.QuantityAvailable > 0)
            .Select(s => new ClinicStockDto(s.MaterialId, s.Material.Name, s.QuantityAvailable, s.Material.Category.ToString(), s.IsOpen, s.OpenedAt))
            .ToList();

        var movements = await _db.StockMovements
            .Include(m => m.Material)
            .Include(m => m.PerformedByUser)
            .Where(m => m.ClinicId == id)
            .OrderByDescending(m => m.CreatedAt)
            .Take(20)
            .Select(m => new StockMovementDto(m.Id, m.ClinicId, m.MaterialId, m.Material.Name, m.Quantity, m.MovementType.ToString(), m.PerformedByUser.UserName, m.CreatedAt, m.Note))
            .ToListAsync();

        return Ok(new
        {
            clinic.Id,
            clinic.Name,
            Stocks = stocks,
            RecentMovements = movements
        });
    }
}
