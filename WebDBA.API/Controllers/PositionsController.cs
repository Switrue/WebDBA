using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDBA.API.Models;
using WebDBA.Migrator.Migration;

namespace WebDBA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PositionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<PositionDto>>> GetPositions()
        {
            try
            {
                var positions = await _context.Positions
                    .Select(p => new PositionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        StructuralUnitId = p.StructuralUnitId
                    })
                    .ToListAsync();

                return Ok(positions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
