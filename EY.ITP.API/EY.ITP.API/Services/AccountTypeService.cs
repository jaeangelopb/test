using EY.ITP.API.Interfaces.Providers;
using EY.ITP.API.Interfaces.Services;
using EY.ITP.API.Models.Responses;

namespace EY.ITP.API.Services
{
    public class AccountTypeService : IAccountTypeService
    {
        private readonly IAccountTypeProvider _provider;

        public AccountTypeService(IAccountTypeProvider provider)
            => _provider = provider ?? throw new ArgumentException(nameof(provider));

        public async Task<IEnumerable<AccountTypeListResponse>> GetAccountTypeList()
        {
            var records = await _provider.GetAccountTypeList();

            List<AccountTypeListResponse> mappedData = records.Select(r => new AccountTypeListResponse()
            {
                AccountTypeId= r.AccountTypeId,
                AccountType= r.AccountType,
                Sorting = r.Sorting
            }).ToList();

            return mappedData;
        }
    }
}
