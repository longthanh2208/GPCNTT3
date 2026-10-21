using AssetManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PMTasksController : ControllerBase
    {
        private readonly Session3Context _context;

        public PMTasksController(Session3Context context)
        {
            _context = context;
        }

        // GET api/PMTasks?activeDate=2025-10-21&assetId=0&taskId=0
        // Trả về danh sách PMTask (active theo ngày hoặc theo km) kèm thông tin Asset + Task + ScheduleType
        [HttpGet]
        public async Task<IActionResult> GetActiveTasks(
            [FromQuery] DateOnly? activeDate,
            [FromQuery] int? assetId,
            [FromQuery] int? taskId)
        {
            // Mặc định active date là hôm nay nếu không truyền
            var date = activeDate ?? DateOnly.FromDateTime(DateTime.Today);

            // Ngày giới hạn hiển thị 4 ngày tới
            var datePlus4 = date.AddDays(4);

            // Lấy tất cả tasks kèm join Asset, Task, PMScheduleType
            var query = _context.Pmtasks
                .Include(t => t.Asset)
                .Include(t => t.Task)
                .Include(t => t.PmscheduleType)
                .AsQueryable();

            // Lọc theo Asset nếu có filter
            if (assetId.HasValue && assetId.Value > 0)
                query = query.Where(t => t.AssetId == assetId.Value);

            // Lọc theo Task nếu có filter
            if (taskId.HasValue && taskId.Value > 0)
                query = query.Where(t => t.TaskId == taskId.Value);

            var allTasks = await query.ToListAsync();

            // Lấy km mới nhất của từng asset từ bảng AssetOdometers
            var latestOdometers = await _context.AssetOdometers
                .GroupBy(o => o.AssetId)
                .Select(g => new
                {
                    AssetId = g.Key,
                    LatestKm = g.OrderByDescending(o => o.ReadDate).Select(o => o.OdometerAmount).FirstOrDefault()
                })
                .ToListAsync();

            var odometerDict = latestOdometers.ToDictionary(o => o.AssetId ?? 0, o => o.LatestKm);

            // Lọc ra các active task theo quy tắc đề bài:
            // - Time-based: scheduleDate <= activeDate+4 (bao gồm quá hạn, hôm nay, 4 ngày tới)
            // - Run-based: latestKm của asset >= scheduleKilometer
            var result = new List<PMTaskResponse>();

            foreach (var t in allTasks)
            {
                var scheduleTypeName = t.PmscheduleType?.Name ?? "";
                bool isRunBased = scheduleTypeName.ToLower().Contains("run") ||
                                  (t.ScheduleKilometer.HasValue && !t.ScheduleDate.HasValue);

                if (isRunBased)
                {
                    // Run-based: hiện task nếu km hiện tại của asset >= km ngưỡng
                    if (t.ScheduleKilometer.HasValue && t.AssetId.HasValue)
                    {
                        odometerDict.TryGetValue(t.AssetId.Value, out decimal latestKm);
                        if (latestKm >= t.ScheduleKilometer.Value || (t.TaskDone == true))
                        {
                            result.Add(MapToResponse(t, "Run-based", isRunBased: true));
                        }
                    }
                }
                else
                {
                    // Time-based: hiện task nếu scheduleDate nằm trong khoảng [bất kỳ quá hạn ... +4 ngày]
                    if (t.ScheduleDate.HasValue)
                    {
                        if (t.ScheduleDate.Value <= datePlus4 || t.TaskDone == true)
                        {
                            result.Add(MapToResponse(t, scheduleTypeName, isRunBased: false));
                        }
                    }
                }
            }

            return Ok(result);
        }

        // PUT api/PMTasks/{id}
        // Body: { "taskDone": true/false }
        // Cập nhật trạng thái hoàn thành của một tác vụ
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskDone(int id, [FromBody] UpdateTaskDoneRequest request)
        {
            var task = await _context.Pmtasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = $"Không tìm thấy PMTask có ID = {id}" });

            task.TaskDone = request.TaskDone;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã cập nhật trạng thái tác vụ thành công.", id, taskDone = request.TaskDone });
        }

        // POST api/PMTasks
        // Thêm mới một tác vụ bảo trì phòng ngừa
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreatePMTaskRequest request)
        {
            // Kiểm tra bắt buộc
            if (request.AssetId <= 0 || request.TaskId <= 0 || request.PMScheduleTypeId <= 0)
                return BadRequest(new { message = "AssetId, TaskId, PMScheduleTypeId là bắt buộc." });

            var newTask = new Pmtask
            {
                AssetId = request.AssetId,
                TaskId = request.TaskId,
                PmscheduleTypeId = request.PMScheduleTypeId,
                ScheduleDate = request.ScheduleDate,
                ScheduleKilometer = request.ScheduleKilometer,
                TaskDone = false
            };

            _context.Pmtasks.Add(newTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActiveTasks), new { id = newTask.Id },
                new { message = "Tạo tác vụ thành công.", id = newTask.Id });
        }

        // ─── Helper ──────────────────────────────────────────────────────────────

        private static PMTaskResponse MapToResponse(Pmtask t, string scheduleTypeName, bool isRunBased)
        {
            return new PMTaskResponse
            {
                Id = t.Id,
                AssetName = t.Asset?.AssetName ?? "",
                AssetSN = t.Asset?.AssetSn ?? "",
                TaskName = t.Task?.Name ?? "",
                ScheduleType = scheduleTypeName,
                ScheduleDate = t.ScheduleDate?.ToString("yyyy-MM-dd"),
                ScheduleKilometer = t.ScheduleKilometer,
                TaskDone = t.TaskDone ?? false,
                IsRunBased = isRunBased
            };
        }
    }

    // ─── DTOs ────────────────────────────────────────────────────────────────────

    public class PMTaskResponse
    {
        public int Id { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string AssetSN { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string ScheduleType { get; set; } = string.Empty;
        public string? ScheduleDate { get; set; }
        public decimal? ScheduleKilometer { get; set; }
        public bool TaskDone { get; set; }
        public bool IsRunBased { get; set; }
    }

    public class UpdateTaskDoneRequest
    {
        public bool TaskDone { get; set; }
    }

    public class CreatePMTaskRequest
    {
        public int AssetId { get; set; }
        public int TaskId { get; set; }
        public int PMScheduleTypeId { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public decimal? ScheduleKilometer { get; set; }
    }
}
