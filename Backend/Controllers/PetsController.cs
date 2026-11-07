using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]   // -> /api/pets
public class PetsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PetsController(AppDbContext db) => _db = db;

    // GET /api/pets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pet>>> GetAll() =>
        await _db.Pets.ToListAsync();

    // POST /api/pets
    [HttpPost]
    public async Task<ActionResult<Pet>> Create(Pet pet)
    {
        _db.Pets.Add(pet);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = pet.Id }, pet);
    }
}
