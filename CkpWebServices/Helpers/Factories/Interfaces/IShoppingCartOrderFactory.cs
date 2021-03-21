using CkpDAL.Model;
using System;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IShoppingCartOrderFactory
    {
        Order Create(
            int clientLegalPersonId,
            int clientCompanyId,
            int supplierLegalPersonId,
            DateTime maxExitDate,
            float sum,
            int managerId);
    }
}
