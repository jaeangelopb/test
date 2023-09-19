using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace EY.ITP.API.DBContext
{
    public class CommonDBContext : DbContext
    {
        public CommonDBContext(DbContextOptions<CommonDBContext> options) : base(options) { }

        #region Extensions
        #endregion

        #region Tables
        #endregion

        #region Views
        public DbSet<AccountTypeListView> AccountTypeListView { get; set; }
        public DbSet<DisclosuresListView> DisclosuresListView { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommonDBContext).Assembly);

            modelBuilder.HasDefaultSchema("Common");

            modelBuilder.Entity<AccountTypeListView>(mb => mb.HasNoKey());
            modelBuilder.Entity<DisclosuresListView>(mb => mb.HasNoKey());

            base.OnModelCreating(modelBuilder);
        }
    }
}
