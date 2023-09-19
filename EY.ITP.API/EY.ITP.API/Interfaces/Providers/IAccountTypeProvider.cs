using EY.ITP.API.Models.Entities;

namespace EY.ITP.API.Interfaces.Providers
{
    public interface IAccountTypeProvider
    {
        Task<IEnumerable<AccountTypeListView>> GetAccountTypeList();
    }
}
