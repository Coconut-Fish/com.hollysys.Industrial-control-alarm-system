using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using com.hollysys.Industrial_control_alarm_system.Data;
using com.hollysys.Industrial_control_alarm_system.Models;
using Microsoft.EntityFrameworkCore;
using com.hollysys.Industrial_control_alarm_system.ViewModels;
using com.hollysys.Industrial_control_alarm_system.Services;
using MyProject.Services;
using System.Text.Json;

namespace com.hollysys.Industrial_control_alarm_system.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly IndustrialControlAlarmSystemContext context;
        private readonly RedisCacheService redisCacheService;
        public AlarmController(IndustrialControlAlarmSystemContext context, RedisCacheService redisCacheService)
        {
            this.context = context;
            this.redisCacheService = redisCacheService;
        }

        /// <summary>
        /// 获取实时报警（分页）
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<Alarm>>> GetRealTimeAlarms(DateTime? time, int pageSize = 30)
        {
            if (!time.HasValue)
            {
                time = DateTime.Now;
            }

            string cacheKey = $"RealTimeAlarms_{time.Value.Ticks}_{pageSize}";
            var cachedResult = await redisCacheService.GetAsync<PagedResult<Alarm>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var totalItems = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .Where(e => e.AlarmTime < time)
                .OrderByDescending(e => e.AlarmTime)
                .Take(pageSize)
                .ToListAsync();
            var result = new PagedResult<Alarm>
            {
                Items = alarms,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            await redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        /// <summary>
        /// 获取最近30条实时报警
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Alarm>>> GetLatest30RealTimeAlarms()
        {
            string cacheKey = "Latest30RealTimeAlarms";
            var cachedResult = await redisCacheService.GetAsync<List<Alarm>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .OrderByDescending(e => e.AlarmTime)
                .Take(30)
                .ToListAsync();

            await redisCacheService.SetAsync(cacheKey, alarms, TimeSpan.FromMinutes(5));
            return alarms;
        }

        /// <summary>
        /// 获取历史报警（分页）
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<Alarm>>> GetHistoryAlarms(DateTime? time, int pageSize = 30)
        {
            if (!time.HasValue)
            {
                time = DateTime.Now;
            }

            string cacheKey = $"HistoryAlarms_{time.Value.Ticks}_{pageSize}";
            var cachedResult = await redisCacheService.GetAsync<PagedResult<Alarm>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var totalItems = await context.Alarms
                .Where(e => e.IsConfirmed == true && e.IsRecovered == true)
                .CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == true && e.IsRecovered == true)
                .Where(e => e.AlarmTime < time)
                .OrderByDescending(e => e.AlarmTime)
                .Take(pageSize)
                .ToListAsync();
            var result = new PagedResult<Alarm>
            {
                Items = alarms,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            await redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        /// <summary>
        /// 获取最近30条历史报警
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Alarm>>> GetLatest30HistoryTimeAlarms()
        {
            string cacheKey = "Latest30HistoryTimeAlarms";
            var cachedResult = await redisCacheService.GetAsync<List<Alarm>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == true || e.IsRecovered == true)
                .OrderByDescending(e => e.AlarmTime)
                .Take(30)
                .ToListAsync();

            await redisCacheService.SetAsync(cacheKey, alarms, TimeSpan.FromMinutes(5));
            return alarms;
        }

        /// <summary>
        /// 依次按报警确认状态、报警恢复状态、报警级别、报警发生时间对所有报警进行排序
        /// 获取最近的10条报警
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Alarm>>> GetTenRealTimeAlarm()
        {
            string cacheKey = "TenRealTimeAlarm";
            var cachedResult = await redisCacheService.GetAsync<List<Alarm>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .OrderByDescending(e => e.IsConfirmed)
                .ThenByDescending(e => e.IsRecovered)
                .ThenBy(e => e.Level)
                .ThenByDescending(e => e.AlarmTime)
                .Take(10)
                .ToListAsync();

            await redisCacheService.SetAsync(cacheKey, alarms, TimeSpan.FromMinutes(5));
            return alarms;
        }

        /// <summary>
        /// 查询报警
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<Alarm>>> GetAlarm([FromQuery] GetAlarmViewModel model, int PageNumber = 1, int PageSize = 30)
        {
            string cacheKey = $"GetAlarm_{JsonSerializer.Serialize(model)}_{PageNumber}_{PageSize}";
            var cachedResult = await redisCacheService.GetAsync<PagedResult<Alarm>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var query = context.Alarms.AsQueryable();

            if (model.IsConfirmed.HasValue)
            {
                query = query.Where(e => e.IsConfirmed == model.IsConfirmed.Value);
            }

            if (model.IsRecovered.HasValue)
            {
                query = query.Where(e => e.IsRecovered == model.IsRecovered.Value);
            }

            if (model.Level.HasValue)
            {
                query = query.Where(e => e.Level == model.Level.Value);
            }

            if (model.Source != null)
            {
                query = query.Where(e => e.Source.Contains(model.Source));
            }

            if (model.Type != null)
            {
                query = query.Where(e => e.Type.Contains(model.Type));
            }

            if (model.Start_AlarmTime.HasValue && model.End_AlarmTime.HasValue)
            {
                query = query
                    .Where(e => e.AlarmTime >= model.Start_AlarmTime.Value)
                    .Where(e => e.AlarmTime <= model.End_AlarmTime);
            }
            else if (model.Start_AlarmTime.HasValue && !model.End_AlarmTime.HasValue)
            {
                query = query.Where(e => e.AlarmTime >= model.Start_AlarmTime.Value);
            }
            else if (model.End_AlarmTime.HasValue && !model.Start_AlarmTime.HasValue)
            {
                query = query.Where(e => e.AlarmTime <= model.End_AlarmTime);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var alarms = await query
                .OrderByDescending(e => e.AlarmTime)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            var result = new PagedResult<Alarm>
            {
                Items = alarms,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            await redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task AddAlarm(AddAlarmViewModel model)
        {
            var alarm = new Alarm
            {
                AlarmTime = DateTime.Now,
                Source = model.Source,
                Type = model.Type,
                Level = model.Level,
                AdditionalInfo = model.AdditionalInfo,
                IsConfirmed = false,
                IsRecovered = false
            };
            context.Alarms.Add(alarm);
            await redisCacheService.RemoveAsync("Latest30RealTimeAlarms");
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 更新确认状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult> ConfirmAlarm(int id)
        {
            var alarm = await context.Alarms.FindAsync(id);
            if (alarm == null)
            {
                return NotFound();
            }
            alarm.IsConfirmed = true;
            alarm.ConfirmTime = DateTime.Now;
            await context.SaveChangesAsync();
            await redisCacheService.RemoveByPatternAsync("RealTimeAlarms_");
            await redisCacheService.RemoveByPatternAsync("HistoryAlarms_");
            return NoContent();
        }

        /// <summary>
        /// 更新恢复状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult> RecoverAlarm(int id)
        {
            var alarm = await context.Alarms.FindAsync(id);
            if (alarm == null)
            {
                return NotFound();
            }
            alarm.IsRecovered = true;
            alarm.RecoverTime = DateTime.Now;
            await context.SaveChangesAsync();
            await redisCacheService.RemoveByPatternAsync("RealTimeAlarms_");
            await redisCacheService.RemoveByPatternAsync("HistoryAlarms_");
            return NoContent();
        }
    }
}
