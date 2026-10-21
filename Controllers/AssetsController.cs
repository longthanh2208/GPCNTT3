using AssetManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly Session3Context _context;

        public AssetsController(Session3Context context)
        {
            _context = context;
        }

        // GET api/Assets
        // Trả danh sách assets cho dropdown filter trên Android
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assets = await _context.Assets
                .Select(a => new { a.Id, a.AssetName, a.AssetSn })
                .OrderBy(a => a.AssetName)
                .ToListAsync();

            return Ok(assets);
        }
    }
}
