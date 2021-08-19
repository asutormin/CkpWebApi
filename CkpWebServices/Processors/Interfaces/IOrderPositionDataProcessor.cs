using CkpModel.Input;
using System.Threading.Tasks;

namespace CkpServices.Processors.Interfaces
{
    interface IOrderPositionDataProcessor
    {
        Task<OrderPositionData> GetOrderPositionFullDataAsync(int orderPositionId);
    }
}
