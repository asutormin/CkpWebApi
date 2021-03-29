using CkpDAL.Model.String;
using CkpServices.Helpers.Factories.Interfaces.String;
using System;

namespace CkpServices.Helpers.Factories.String
{
    class StringOccurrenceFactory : IStringOccurrenceFactory
    {
        public StringOccurrence Create(int stringId, int occurrenceId, int typeId, int orderBy)
        {
            var stringOccurrence = new StringOccurrence
            {
                StringId = stringId,
                OccurrenceId = occurrenceId,
                TypeId = typeId,
                OrderBy = orderBy,
                BeginDate = DateTime.Now
            };

            return stringOccurrence;
        }
    }
}
