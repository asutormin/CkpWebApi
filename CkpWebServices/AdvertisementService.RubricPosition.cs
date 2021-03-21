using CkpDAL.Model;
using CkpEntities.Input;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices
{
    public partial class AdvertisementService
    {
        #region Create
        private void CreateRubricPosition(int orderPositionId, AdvertisementRubric advRubric, DbTransaction dbTran)
        {
            var rubricPositionId = 0;

            _repository.SetRubricPosition(
                dbTran: dbTran,
                id: ref rubricPositionId,
                orderPositionId: orderPositionId,
                rubricId: advRubric.Id,
                rubricVersionDate: advRubric.Version,
                editUserId: _editUserId,
                isActual: true);
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

        private void UpdateRubricPosition(int orderPositionId, IEnumerable<RubricPosition> rubricPositions, AdvertisementRubric advRubric, DbTransaction dbTran)
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
        private void DeleteRubricPositions(IEnumerable<RubricPosition> rubricPositions, DbTransaction dbTran)
        {
            var rubricPositionsList = rubricPositions.ToList();

            for (int i = rubricPositionsList.Count - 1; i >= 0; i--)
                DeleteRubricPosition(rubricPositionsList[i], dbTran);
        }

        private void DeleteRubricPosition(RubricPosition rubricPosition, DbTransaction dbTran)
        {
            var rubricPositionId = rubricPosition.Id;

            _repository.SetRubricPosition(
                dbTran: dbTran,
                id: ref rubricPositionId,
                orderPositionId: rubricPosition.OrderPositionId,
                rubricId: rubricPosition.RubricId,
                rubricVersionDate: rubricPosition.RubricVersion,
                editUserId: _editUserId,
                isActual: false);

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
