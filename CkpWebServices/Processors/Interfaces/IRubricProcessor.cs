using CkpDAL.Entities;
using CkpModel.Input;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IRubricProcessor
    {
        RubricPosition CreateRubricPosition(int orderPositionId, RubricData advRubric, DbTransaction dbTran);
        void UpdateRubricPosition(int orderPositionId, IEnumerable<RubricPosition> rubricPositions, RubricData advRubric, DbTransaction dbTran);
        void DeleteRubricPositions(IEnumerable<RubricPosition> rubricPositions, DbTransaction dbTran);
    }
}
