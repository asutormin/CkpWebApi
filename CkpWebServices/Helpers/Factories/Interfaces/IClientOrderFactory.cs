using CkpDAL.Entities;
using System.Collections.Generic;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IClientOrderFactory
    {
        Order Create(Order basketOrder, IEnumerable<OrderPosition> orderPositions);
    }
}
