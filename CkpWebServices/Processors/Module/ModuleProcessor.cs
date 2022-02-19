using CkpDAL;
using CkpDAL.Entities.Module;
using CkpDAL.Repository;
using CkpServices.Helpers.Factories.Interfaces.Module;
using CkpServices.Helpers.Factories.Module;
using CkpServices.Processors.Interfaces;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.Module
{
    class ModuleProcessor : IModuleProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IModulePositionFactory _modulePositionFactory;

        public ModuleProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;

            _modulePositionFactory = new ModulePositionFactory();
        }

        #region Get

        public ModulePosition GetModulePosition(int orderPositionId)
        {
            var modulePosition = _context.ModulePositions
                .SingleOrDefault(mp => mp.OrderPositionId == orderPositionId);

            return modulePosition;
        }

        #endregion

        #region Create

        public ModulePosition CreateModule(int clientId, int businessUnitId, int orderPositionId, DbTransaction dbTran)
        {
            var modulePosition = _modulePositionFactory.Create(clientId, businessUnitId, orderPositionId);
            modulePosition = _repository.SetModule(modulePosition, isActual: true, dbTran);

            return modulePosition;
        }

        #endregion

        #region Delete

        public void DeleteModule(int orderPositionId, DbTransaction dbTran)
        {
            var modulePosition = GetModulePosition(orderPositionId);

            if (modulePosition == null)
                return;

            _repository.SetModule(modulePosition, isActual: false, dbTran);
        }

        #endregion
    }
}
