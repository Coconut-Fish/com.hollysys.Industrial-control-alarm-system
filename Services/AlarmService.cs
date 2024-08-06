using com.hollysys.Industrial_control_alarm_system.Data;
using com.hollysys.Industrial_control_alarm_system.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace com.hollysys.Industrial_control_alarm_system.Services
{
    public class AlarmService : IAlarmService
    {
        private readonly IndustrialControlAlarmSystemContext _context;
        public AlarmService(IndustrialControlAlarmSystemContext context)
        {
            _context = context;
        }
    }
}
