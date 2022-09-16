using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UniShedule.Model;

namespace UniShedule.Database
{
    class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS; DATABASE=UniSheduleDB; Trusted_Connection=True");
        }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<DateInfo> DateInfos { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }



    }
}
