using Server.Data;
using Server.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace Server.Services
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
