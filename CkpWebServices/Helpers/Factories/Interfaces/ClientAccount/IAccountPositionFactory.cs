using CkpDAL.Entities;
using System.Collections.Generic;

namespace CkpServices.Helpers.Factories.Interfaces.ClientAccount
{
    interface IAccountPositionFactory
    {
        AccountPosition Create(int accountId, OrderPosition orderPosition, List<OrderPosition> packagePositions);
    }
}
