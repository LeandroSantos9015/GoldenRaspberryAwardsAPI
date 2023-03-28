using GoldenRaspberryAwardsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace GoldenRaspberryAwardsAPI.Repository
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<ModelFilmes> Filmes { get; set; }

    }
}
