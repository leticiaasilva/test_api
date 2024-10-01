using System;
using Microsoft.EntityFrameworkCore;

namespace SQLiteTest
{
    public class MyDbContext : DbContext
    {
        public DbSet<MyEntity> MyEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./test.db");
        }
    }

    public class MyEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new MyDbContext())
            {
                context.Database.EnsureCreated(); // This should create the .db file
            }

            Console.WriteLine("Database created successfully.");
        }
    }
}
