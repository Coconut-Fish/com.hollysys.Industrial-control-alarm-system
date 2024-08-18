using Microsoft.EntityFrameworkCore;
using Server.Models;
using System.Security.Claims;

namespace Server.Data
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
