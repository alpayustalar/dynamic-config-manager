using ConfigurationLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationLibrary.DBConnection
{
    /// <summary>
    /// EF Core üzerinden SQL Server ile bağlantı kuran DbContext sınıfıdır.
    /// Konfigürasyon kayıtlarını veritabanından okumak için kullanılır.
    /// </summary>
    public class ConfigurationDbContext : DbContext
    {
        private readonly string _connectionString;

        /// <summary>
        /// Bağlantı stringi ile DbContext nesnesi oluşturur.
        /// </summary>
        /// <param name="connectionString">SQL Server bağlantı bilgisi</param>
        public ConfigurationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Konfigürasyon kayıtlarını temsil eden DbSet.
        /// </summary>
        public DbSet<ConfigurationItem> ConfigurationRecords => Set<ConfigurationItem>();

        /// <summary>
        /// Veritabanı sağlayıcısı olarak SQL Server yapılandırılır.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        /// <summary>
        /// Tablo adlarını manuel olarak eşleştirir.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfigurationItem>().ToTable("ConfigurationRecords");
        }
    }
}
