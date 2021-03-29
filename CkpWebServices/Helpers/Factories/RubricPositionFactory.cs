using CkpDAL.Model;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class RubricPositionFactory : IRubricPositionFactory
    {
        public RubricPosition Create(int orderPositionId, int rubricId, DateTime rubricVersion)
        {
            var rubricPosition = new RubricPosition
            {
                Id = 0,
                OrderPositionId = orderPositionId,
                RubricId = rubricId,
                RubricVersion = rubricVersion,
                BeginDate = DateTime.Now
            };

            return rubricPosition;
        }
    }
}
