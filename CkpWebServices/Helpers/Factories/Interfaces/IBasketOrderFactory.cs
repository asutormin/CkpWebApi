using CkpDAL.Entities;
using System;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IBasketOrderFactory
    {
        Order Create(
            int businessUnitId,
            int clientLegalPersonId,
            int clientCompanyId,
            int supplierLegalPersonId,
            bool isAdvance,
            DateTime maxExitDate,
            float sum,
            int managerId);
    }
}
