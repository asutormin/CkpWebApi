using CkpWebApi.InputEntities;
using CkpWebApi.OutputEntities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CkpWebApi.Services.Interfaces
{
    public interface IAdvertisementService
    {
        Task<ActionResult<IEnumerable<PositionInfo>>> GetShoppingCartAsync(int clientLegalPersonId);
        Advertisement GetAdvertisementFull(int orderPositionId);
        void CreateAdvertisement(Advertisement advertisement);
        void UpdateAdvertisement(Advertisement advertisement);
        void DeleteAdvertisement(int orderPositionId);
        int CreateClientAccount(int[] orderPositionIds);
    }
}
