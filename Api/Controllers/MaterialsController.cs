using Core.Entities;
using Core.Entities.Enums;
using Infrastructure.Dat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

[ApiController]
[Route("materials")]
public class MaterialsController : ControllerBase
{
    private readonly AppDbContext _db;
    public MaterialsController(AppDbContext db) => _db = db;

    // 🔹 GET /materials
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Materials
            .Select(m => new MaterialDto(m.Id, m.Name, m.Category.ToString()))
            .ToListAsync());

    // 🔹 GET /materials/by-category/{category}
    [HttpGet("by-category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        if (!Enum.TryParse<MaterialCategory>(category, true, out var parsedCategory))
            return BadRequest($"Categoria inválida: {category}");

        var materials = await _db.Materials
            .Where(m => m.Category == parsedCategory)
            .Select(m => new MaterialDto(m.Id, m.Name, m.Category.ToString()))
            .ToListAsync();

        return Ok(materials);
    }

    // 🔹 GET /materials/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var m = await _db.Materials.FindAsync(id);
        if (m == null) return NotFound();
        return Ok(new MaterialDto(m.Id, m.Name, m.Category.ToString()));
    }

    // 🔹 POST /materials
    [HttpPost]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> Create([FromBody] MaterialDto dto)
    {
        if (!Enum.TryParse<MaterialCategory>(dto.Category, true, out var parsedCategory))
            return BadRequest($"Categoria inválida: {dto.Category}");

        var material = new Material
        {
            Name = dto.Name,
            Category = parsedCategory
        };

        _db.Materials.Add(material);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = material.Id },
            new MaterialDto(material.Id, material.Name, material.Category.ToString()));
    }

    // 🔹 PUT /materials/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> Update(Guid id, [FromBody] MaterialDto dto)
    {
        var m = await _db.Materials.FindAsync(id);
        if (m == null) return NotFound();

        if (!Enum.TryParse<MaterialCategory>(dto.Category, true, out var parsedCategory))
            return BadRequest($"Categoria inválida: {dto.Category}");

        m.Name = dto.Name;
        m.Category = parsedCategory;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("batch")]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> CreateBatch([FromBody] MaterialCreateBatchRequest request)
    {
        foreach (var item in request.Materials)
        {
            var material = new Material
            {
                Name = item.Name,
                Category = Enum.Parse<MaterialCategory>(item.Category)
            };

            _db.Materials.Add(material);

            // Cria o movimento de entrada (estoque)
            _db.StockMovements.Add(new StockMovement
            {
                MaterialId = material.Id,
                ClinicId = Guid.Empty, // definir depois se quiser vincular
                Quantity = item.Quantity,
                MovementType = Core.Entities.Enums.MovementType.Entrada,
                PerformedByUserId = Guid.Empty // o usuário logado depois
            });
        }

        await _db.SaveChangesAsync();
        return Ok();
    }


    // 🔹 DELETE /materials/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var m = await _db.Materials.FindAsync(id);
        if (m == null) return NotFound();

        _db.Materials.Remove(m);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
