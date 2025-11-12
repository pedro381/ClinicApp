using Core.Entities;
using Infrastructure.Dat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

[ApiController]
[Route("clinics")]
public class ClinicsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ClinicsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clinics = await _db.Clinics
            .Select(c => new ClinicDto(c.Id, c.Name))
            .ToListAsync();
        return Ok(clinics);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var clinic = await _db.Clinics
            .Include(c => c.ClinicStocks)
                .ThenInclude(s => s.Material)
            .Include(c => c.UserClinics)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (clinic == null) return NotFound();

        var stocks = clinic.ClinicStocks.Select(s => new ClinicStockDto(
            s.MaterialId, s.Material.Name, s.QuantityAvailable, s.Material.Category.ToString()
        )).ToList();

        var movements = await _db.StockMovements
            .Where(m => m.ClinicId == id)
            .OrderByDescending(m => m.CreatedAt)
            .Take(30)
            .Select(m => new StockMovementDto(m.Id, m.ClinicId, m.MaterialId, m.Quantity, m.MovementType.ToString(), m.PerformedByUser.UserName, m.CreatedAt, m.Note))
            .ToListAsync();

        return Ok(new { Id = clinic.Id, clinic.Name, Stocks = stocks, Movements = movements });
    }

    [HttpPost]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> Create([FromBody] ClinicDto dto)
    {
        var clinic = new Clinic { Name = dto.Name };
        _db.Clinics.Add(clinic);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = clinic.Id }, new ClinicDto(clinic.Id, clinic.Name));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ClinicDto dto)
    {
        var clinic = await _db.Clinics.FindAsync(id);
        if (clinic == null) return NotFound();
        clinic.Name = dto.Name;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var clinic = await _db.Clinics.FindAsync(id);
        if (clinic == null) return NotFound();
        _db.Clinics.Remove(clinic);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
