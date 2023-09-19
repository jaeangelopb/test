using EY.ITP.API.DBContext;
using EY.ITP.API.Interfaces.Providers;
using EY.ITP.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EY.ITP.API.Providers
{
    public class DisclosuresProvider : BaseProvider<DisclosuresListView>, IDisclosuresProvider
    {
        public DisclosuresProvider(CommonDBContext dBContext) : base(dBContext) { }

        public DisclosuresProvider(CommonDBContext dBContext, IConfiguration configuration) : base(dBContext, configuration) { }

        public async Task<IEnumerable<DisclosuresListView>> GetDisclosuresLists()
        {
            return await _dbContext.Set<DisclosuresListView>().OfType<DisclosuresListView>().ToListAsync() ;
        }
    }
}
