using CkpDAL;
using CkpDAL.Model;
using CkpDAL.Repository;
using CkpEntities.Input;
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

        public RubricPosition CreateRubricPosition(int orderPositionId, AdvertisementRubric advRubric, DbTransaction dbTran)
        {
            var rubricPosition = _rubricPositionFactory.Create(orderPositionId, advRubric.Id, advRubric.Version);
            rubricPosition = _repository.SetRubricPosition(rubricPosition, isActual: true, dbTran);

            return rubricPosition;
        }

        private bool NeedCreateRubricPosition(IEnumerable<RubricPosition> rubricPositions, AdvertisementRubric advRubric)
        {
            if (rubricPositions.Any(
                rp =>
                    rp.RubricId == advRubric.Id &&
                    rp.RubricVersion == advRubric.Version))
                return false;

            return true;
        }

        #endregion

        #region Update

        public void UpdateRubricPosition(int orderPositionId, IEnumerable<RubricPosition> rubricPositions, AdvertisementRubric advRubric, DbTransaction dbTran)
        {
            var rubricPositionsList = rubricPositions.ToList();

            // Удаляем все позиции рубрик, у которых не совпадает RubricId и RubricVersion
            for (int i = rubricPositionsList.Count - 1; i >= 0; i--)
                if (advRubric == null || NeedDeleteRubricPosition(rubricPositionsList[i], advRubric))
                    DeleteRubricPosition(rubricPositionsList[i], dbTran);

            // Если позиция рубрики с RubricId и RubricVersion существует - выходим
            if (!NeedCreateRubricPosition(rubricPositions, advRubric))
                return;

            // Иначе создаём новую позицию рубрики
            CreateRubricPosition(orderPositionId, advRubric, dbTran);
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

        private bool NeedDeleteRubricPosition(RubricPosition rubricPosition, AdvertisementRubric advRubric)
        {
            if (rubricPosition.RubricId != advRubric.Id ||
                rubricPosition.RubricVersion != advRubric.Version)
                return true;

            return false;
        }

        #endregion
    }
}
