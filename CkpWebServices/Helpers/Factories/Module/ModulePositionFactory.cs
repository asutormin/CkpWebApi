using CkpDAL.Entities.Module;
using CkpServices.Helpers.Factories.Interfaces.Module;
using System;

namespace CkpServices.Helpers.Factories.Module
{
    class ModulePositionFactory : IModulePositionFactory
    {
        public ModulePosition Create(int companyId, int businessUnitId, int orderPositionId)
        {
            var modulePosition = new ModulePosition
            {
                CompanyId = companyId,
                BusinessUnitId = businessUnitId,
                OrderPositionId = orderPositionId,
                VacancyName = string.Empty,
                BeginDate = DateTime.Now
            };

            return modulePosition;
        }
    }
}
