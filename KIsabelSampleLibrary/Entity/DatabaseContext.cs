using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Entity
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Sample> Samples { get; set; }
        public DbSet<SamplesFolder> SamplesFolders { get; set; }
        public DbSet<DrumKit> DrumKits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=samplesdata.db");
        }
    }
}
