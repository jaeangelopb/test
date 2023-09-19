using EY.ITP.API.DBContext;
using EY.ITP.API.Interfaces.Providers;
using EY.ITP.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EY.ITP.API.Providers
{
    public class AccountTypeProvider : BaseProvider<AccountTypeListView>, IAccountTypeProvider
    {
        public AccountTypeProvider(CommonDBContext dBContext) : base(dBContext) { }

        public AccountTypeProvider(CommonDBContext dBContext, IConfiguration configuration) : base(dBContext, configuration) { }

        public async Task<IEnumerable<AccountTypeListView>> GetAccountTypeList()
        {
            return await _dbContext.Set<AccountTypeListView>().OfType<AccountTypeListView>().ToListAsync() ;
        }
    }
}
