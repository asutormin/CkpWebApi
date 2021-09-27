using CkpModel.Input;
using CkpModel.Output;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CkpServices.Interfaces
{
    public interface IOrderPositionService
    {
        bool ExistsById(int orderPositionId);
        IEnumerable<OrderPositionInfo> GetBasket(int clientLegalPersonId);
        Task<OrderPositionData> GetOrderPositionDataAsync(int orderPositionId);
        void CreateOrderPosition(OrderPositionData orderPosition);
        void UpdateOrderPosition(OrderPositionData orderPosition);
        void DeleteOrderPosition(int orderPositionId);
    }
}
