using AssetManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly Session3Context _context;

        public TasksController(Session3Context context)
        {
            _context = context;
        }

        // GET api/Tasks
        // Trả danh sách tasks cho dropdown filter trên Android
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _context.Tasks
                .Select(t => new { t.Id, t.Name })
                .OrderBy(t => t.Name)
                .ToListAsync();

            return Ok(tasks);
        }
    }
}
