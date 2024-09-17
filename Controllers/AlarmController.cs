using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using Server.ViewModels;
using Server.Services;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using System.Diagnostics;

namespace Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly IndustrialControlAlarmSystemContext context;
        private readonly RedisCacheService redisCacheService;
        //private readonly IDistributedCache redis;
        private readonly IHubContext<AlarmHub> hubContext;

        public AlarmController(IndustrialControlAlarmSystemContext context, RedisCacheService redisCacheService, IDistributedCache reids, IHubContext<AlarmHub> hubContext)
        {
            this.context = context;
            this.redisCacheService = redisCacheService;
            //this.redis = reids;
            this.hubContext = hubContext;
        }

        /// <summary>
        /// 获取实时报警（分页）
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<Alarm>>> GetRealTimeAlarms(DateTime? time, CancellationToken cancellationToken = default,  int pageSize = 30)
        {
            string cacheKey;
            if (!time.HasValue)
            {
                time = DateTime.Now;
                cacheKey = $"RealTimeAlarms_TimeIsNull_{pageSize}";
            }
            else
            {
                cacheKey = $"RealTimeAlarms_{time.Value.Ticks}_{pageSize}";
            }

            var cachedResult = await redisCacheService.GetAsync<PagedResult<Alarm>>(cacheKey,cancellationToken);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var totalItems = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .Where(e => e.AlarmTime < time)
                .OrderByDescending(e => e.AlarmTime)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            var result = new PagedResult<Alarm>
            {
                Items = alarms,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(60), cancellationToken);
            return result;
        }

        /// <summary>
        /// 获取最近30条实时报警
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Alarm>>> GetLatest30RealTimeAlarms(CancellationToken cancellationToken = default)
        {
            string cacheKey = "Latest30RealTimeAlarms";
            var cachedResult = await redisCacheService.GetAsync<List<Alarm>>(cacheKey, cancellationToken);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .OrderByDescending(e => e.AlarmTime)
                .Take(30)
                .ToListAsync(cancellationToken);

            redisCacheService.SetAsync(cacheKey, alarms, TimeSpan.FromMinutes(60), cancellationToken);
            return alarms;
        }

        /// <summary>
        /// 获取历史报警（分页）
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="time"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<Alarm>>> GetHistoryAlarms(DateTime? time, CancellationToken cancellationToken = default, int pageSize = 30)
        {
            string cacheKey;
            if (!time.HasValue)
            {
                time = DateTime.Now;
                cacheKey = $"HistoryAlarms_TimeIsNull_{pageSize}";
            }
            else
            {
                cacheKey = $"RealTimeAlarms_{time.Value.Ticks}_{pageSize}";
            }

            var cachedResult = await redisCacheService.GetAsync<PagedResult<Alarm>>(cacheKey, cancellationToken);
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

            redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(60), cancellationToken);
            return result;
        }

        /// <summary>
        /// 获取最近30条历史报警
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Alarm>>> GetLatest30HistoryTimeAlarms(CancellationToken cancellationToken = default)
        {
            string cacheKey = "Latest30HistoryTimeAlarms";
            var cachedResult = await redisCacheService.GetAsync<List<Alarm>>(cacheKey, cancellationToken);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var alarms = await context.Alarms
                .Where(e => e.IsConfirmed == true || e.IsRecovered == true)
                .OrderByDescending(e => e.AlarmTime)
                .Take(30)
                .ToListAsync(cancellationToken);

            redisCacheService.SetAsync(cacheKey, alarms, TimeSpan.FromMinutes(60), cancellationToken);
            return alarms;
        }

        /// <summary>
        /// 依次按报警确认状态、报警恢复状态、报警级别、报警发生时间对所有报警进行排序
        /// 获取最近的10条报警
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Alarm>>> GetTenRealTimeAlarm(CancellationToken cancellationToken = default)
        {
            string cacheKey = "TenRealTimeAlarm";
            var cachedResult = await redisCacheService.GetAsync<List<Alarm>>(cacheKey, cancellationToken);
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
                .ToListAsync(cancellationToken);

            redisCacheService.SetAsync(cacheKey, alarms, TimeSpan.FromMinutes(60), cancellationToken);
            return alarms;
        }

        /// <summary>
        /// 查询报警
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="model"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<Alarm>>> GetAlarm([FromQuery] GetAlarmViewModel model, CancellationToken cancellationToken = default, int PageNumber = 1, int PageSize = 30)
        {
            string cacheKey = $"GetAlarm_{JsonSerializer.Serialize(model)}_{PageNumber}_{PageSize}";
            var cachedResult = await redisCacheService.GetAsync<PagedResult<Alarm>>(cacheKey, cancellationToken);
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

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var alarms = await query
                .OrderByDescending(e => e.AlarmTime)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(cancellationToken);
            var result = new PagedResult<Alarm>
            {
                Items = alarms,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(60), cancellationToken);
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
            if (await context.SaveChangesAsync() > 0)
            {
                string massage = "update";
                await hubContext.Clients.All.SendAsync("Massage", massage);
                await redisCacheService.RemoveAsync("Latest30RealTimeAlarms");
                await redisCacheService.RemoveAsync("TenRealTimeAlarm");
                await redisCacheService.RemoveByPatternAsync("RealTimeAlarms_");
                await redisCacheService.RemoveByPatternAsync("HistoryAlarms_");
            }
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
            if (alarm.IsConfirmed != true)
            {
                alarm.IsConfirmed = true;
                alarm.ConfirmTime = DateTime.Now;
            }
            
            if (await context.SaveChangesAsync() > 0)
            {
                string massage = "update";
                await hubContext.Clients.All.SendAsync("Massage", massage);
                await redisCacheService.RemoveAsync("Latest30HistoryTimeAlarms");
                await redisCacheService.RemoveAsync("Latest30RealTimeAlarms");

                await redisCacheService.RemoveAsync("TenRealTimeAlarm");
                await redisCacheService.RemoveByPatternAsync("HistoryAlarms_");
                await redisCacheService.RemoveByPatternAsync("RealTimeAlarms_");
            }

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
            if (alarm.IsRecovered != true)
            {
                alarm.IsRecovered = true;
                alarm.RecoverTime = DateTime.Now;
            }
            
            if (await context.SaveChangesAsync() > 0)
            {
                string massage = "update";
                await hubContext.Clients.All.SendAsync("Massage",massage);

                await redisCacheService.RemoveAsync("Latest30HistoryTimeAlarms");
                await redisCacheService.RemoveAsync("Latest30RealTimeAlarms");

                await redisCacheService.RemoveAsync("TenRealTimeAlarm");
                await redisCacheService.RemoveByPatternAsync("HistoryAlarms_");
                await redisCacheService.RemoveByPatternAsync("RealTimeAlarms_");
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var RealTimeAlarmTimes = await context.Alarms
                .Where(e => e.IsConfirmed == false || e.IsRecovered == false)
                .CountAsync();
            var HistoryAlarmTimes = await context.Alarms
                .Where(e => e.IsConfirmed == true && e.IsRecovered == true)
                .CountAsync();
            var notConfirmedAlarmTimes = await context.Alarms
                .Where(e => e.IsConfirmed == false)
                .CountAsync();
            var notRecoveredAlarmTimes = await context.Alarms
                .Where(e => e.IsRecovered == false)
                .CountAsync();
            var CountGroupByLevel = await context.Alarms
                .GroupBy(alarm => alarm.Level)
                .Select(group => new { Level = group.Key, Count = group.Count() })
                .ToDictionaryAsync(g => g.Level, g => g.Count);
            var CountGroupByType = await context.Alarms
                .GroupBy(alarm => alarm.Type)
                .Select(group => new { Type = group.Key, Count = group.Count() })
                .ToDictionaryAsync(g => g.Type, g => g.Count);
            return Ok(new { RealTimeAlarmTimes, HistoryAlarmTimes, notConfirmedAlarmTimes, notRecoveredAlarmTimes, CountGroupByLevel, CountGroupByType });
        }
    }
}
