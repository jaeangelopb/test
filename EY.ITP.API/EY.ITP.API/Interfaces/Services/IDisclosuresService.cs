using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Requests;
using EY.ITP.API.Models.Responses;

namespace EY.ITP.API.Interfaces.Services
{
    public interface IDisclosuresService
    {
        Task<IEnumerable<DisclosuresListResponse>> GetDisclosuresLists();
    }
}
