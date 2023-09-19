using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace EY.ITP.API.DBContext
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext(DbContextOptions<CoreDBContext> options) : base(options) { }

        #region Extensions
        DbSet<StoredProcParameter> StoredProcParameter { get; set; }
        DbSet<TableTypeColumn> TableTypeColumn { get; set; }
        #endregion

        #region Tables
        public DbSet<FileValidationSave> FileValidationSave { get; set; }
        public DbSet<FileValidationUpdate> FileValidationUpdate { get; set; }
        #endregion

        #region Views
        public DbSet<FileValidationListView> FileValidationListView { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDBContext).Assembly);

            modelBuilder.HasDefaultSchema("Core");

            modelBuilder.Entity<FileValidationSave>(mb => mb.HasNoKey());

            modelBuilder.Entity<FileValidationListView>(mb => mb.HasNoKey());

            base.OnModelCreating(modelBuilder);
        }
    }
}
