using CkpDAL.Entities;
using System;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IRubricPositionFactory
    {
        RubricPosition Create(int orderPositionId, int rubricId, DateTime rubricVersion);
    }
}
