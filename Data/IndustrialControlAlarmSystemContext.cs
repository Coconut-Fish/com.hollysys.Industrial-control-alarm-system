using Microsoft.EntityFrameworkCore;
using com.hollysys.Industrial_control_alarm_system.Models;
using System.Security.Claims;

namespace com.hollysys.Industrial_control_alarm_system.Data
{
    public class IndustrialControlAlarmSystemContext : DbContext
    {
        public IndustrialControlAlarmSystemContext(DbContextOptions<IndustrialControlAlarmSystemContext> options)
            : base(options)
        {
        }

        public DbSet<Alarm> Alarms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alarm>().ToTable("Alarm");

        }
    }
}
