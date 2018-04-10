using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using wbible.Models;

namespace wbible.DataContexts
{
    public class WBibleContext : DbContext
    {
        public DbSet<BookStats> Stats { get; set; }
        public DbSet<Readers> Readers { get; set; }
        public DbSet<Corpus> Corpus { get; set; }
        public DbSet<WxD> WXD { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./wbible.db");
        }
    }
}