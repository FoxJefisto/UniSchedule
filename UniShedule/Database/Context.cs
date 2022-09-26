using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UniShedule.Model;

namespace UniShedule.Database
{
    class Context : DbContext
    {
        public Context () {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            //optionsBuilder.UseSqlServer(connectionString);

            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS; DATABASE=UniSheduleDB; Trusted_Connection=True");
        }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<DateInfo> DateInfos { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reminder> Reminders { get; set; }



    }
}
