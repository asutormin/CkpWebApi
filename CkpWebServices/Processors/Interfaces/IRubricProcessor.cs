using CkpDAL.Model;
using CkpEntities.Input;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IRubricProcessor
    {
        RubricPosition CreateRubricPosition(int orderPositionId, AdvertisementRubric advRubric, DbTransaction dbTran);
        void UpdateRubricPosition(int orderPositionId, IEnumerable<RubricPosition> rubricPositions, AdvertisementRubric advRubric, DbTransaction dbTran);
        void DeleteRubricPositions(IEnumerable<RubricPosition> rubricPositions, DbTransaction dbTran);
    }
}
