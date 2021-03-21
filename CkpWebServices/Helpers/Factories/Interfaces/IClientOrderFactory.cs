using CkpDAL.Model;
using System.Collections.Generic;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IClientOrderFactory
    {
        Order Create(Order shoppingCartOrder, IEnumerable<OrderPosition> orderPositions);
    }
}
