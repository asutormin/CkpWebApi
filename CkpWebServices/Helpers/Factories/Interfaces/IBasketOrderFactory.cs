using CkpDAL.Entities;
using System;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IBasketOrderFactory
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
