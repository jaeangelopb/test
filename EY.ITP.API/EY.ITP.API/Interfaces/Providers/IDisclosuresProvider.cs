using EY.ITP.API.Models.Entities;

namespace EY.ITP.API.Interfaces.Providers
{
    public interface IDisclosuresProvider
    {
        Task<IEnumerable<DisclosuresListView>> GetDisclosuresLists();
    }
}
