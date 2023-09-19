using EY.ITP.API.Interfaces.Providers;
using EY.ITP.API.Interfaces.Services;
using EY.ITP.API.Models.Responses;

namespace EY.ITP.API.Services
{
    public class DisclosuresService : IDisclosuresService
    {
        private readonly IDisclosuresProvider _provider;

        public DisclosuresService(IDisclosuresProvider provider)
            => _provider = provider ?? throw new ArgumentException(nameof(provider));

        public async Task<IEnumerable<DisclosuresListResponse>> GetDisclosuresLists()
        {
            var records = await _provider.GetDisclosuresLists();

            List<DisclosuresListResponse> mappedData = records.Select(r => new DisclosuresListResponse()
            {
                DisclosuresId = r.DisclosuresId,
                Jurisdiction = r.Jurisdiction,
                Description = r.Description,
                Group = r.Group,
                Sorting = r.Sorting
            }).ToList();

            return mappedData;
        }
    }
}
