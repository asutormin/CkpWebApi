using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpModel.Input;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using CkpServices.Processors.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors
{
    class RubricProcessor : IRubricProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IRubricPositionFactory _rubricPositionFactory;

        public RubricProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;
                 
            _rubricPositionFactory = new RubricPositionFactory();
        }

        #region Create

        public RubricPosition CreateRubricPosition(int orderPositionId, RubricData rubricData, DbTransaction dbTran)
        {
            var rubricPosition = _rubricPositionFactory.Create(orderPositionId, rubricData.Id, rubricData.Version);
            rubricPosition = _repository.SetRubricPosition(rubricPosition, isActual: true, dbTran);

            return rubricPosition;
        }

        private bool NeedCreateRubricPosition(IEnumerable<RubricPosition> rubricPositions, RubricData rubricData)
        {
            if (rubricPositions.Any(
                rp =>
                    rp.RubricId == rubricData.Id &&
                    rp.RubricVersion == rubricData.Version))
                return false;

            return true;
        }

        #endregion

        #region Update

        public void UpdateRubricPosition(int orderPositionId, IEnumerable<RubricPosition> rubricPositions, RubricData rubricData, DbTransaction dbTran)
        {
            var rubricPositionsList = rubricPositions.ToList();

            // Удаляем все позиции рубрик, у которых не совпадает RubricId и RubricVersion
            for (int i = rubricPositionsList.Count - 1; i >= 0; i--)
                if (rubricData == null || NeedDeleteRubricPosition(rubricPositionsList[i], rubricData))
                    DeleteRubricPosition(rubricPositionsList[i], dbTran);

            // Если позиция рубрики с RubricId и RubricVersion существует - выходим
            if (!NeedCreateRubricPosition(rubricPositions, rubricData))
                return;

            // Иначе создаём новую позицию рубрики
            CreateRubricPosition(orderPositionId, rubricData, dbTran);
        }

        #endregion

        #region Delete

        public void DeleteRubricPositions(IEnumerable<RubricPosition> rubricPositions, DbTransaction dbTran)
        {
            var rubricPositionsList = rubricPositions.ToList();

            for (int i = rubricPositionsList.Count - 1; i >= 0; i--)
                DeleteRubricPosition(rubricPositionsList[i], dbTran);
        }

        private void DeleteRubricPosition(RubricPosition rubricPosition, DbTransaction dbTran)
        {
            _repository.SetRubricPosition(rubricPosition, isActual: false, dbTran);
            _context.Entry(rubricPosition).Reload();
        }

        private bool NeedDeleteRubricPosition(RubricPosition rubricPosition, RubricData rubricData)
        {
            if (rubricPosition.RubricId != rubricData.Id ||
                rubricPosition.RubricVersion != rubricData.Version)
                return true;

            return false;
        }

        #endregion
    }
}
