using CkpDAL.Model.String;
using CkpEntities.Input.String;
using CkpServices.Helpers;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.String
{
    partial class StringProcessor
    {
        #region Create

        private void CreateStringOccurrences(int stringId, IEnumerable<AdvOccurrence> advOcurrences, DbTransaction dbTran)
        {
            foreach (var advOccurence in advOcurrences)
                CreateStringOccurrence(stringId, advOccurence, dbTran);
        }

        private bool NeedCreateStringOccurrence(IEnumerable<StringOccurrence> stringOccurrences, AdvOccurrence advOccurrence)
        {
            return !stringOccurrences
                .GetActualItems()
                .Any(
                so =>
                    so.OccurrenceId == advOccurrence.Id &&
                    so.TypeId == advOccurrence.TypeId &&
                    so.OrderBy == advOccurrence.OrderBy);
        }

        private StringOccurrence CreateStringOccurrence(int stringId, AdvOccurrence advOccurrence, DbTransaction dbTran)
        {
            var stringOccurrence = _stringOccurrenceFactory.Create(stringId, advOccurrence.Id, advOccurrence.TypeId, advOccurrence.OrderBy);

            stringOccurrence = _repository.SetStringOccurrence(stringOccurrence, isActual: true, dbTran);

            return stringOccurrence;
        }

        #endregion

        #region Update

        private void UpdateStringOccurences(int stringId, IEnumerable<StringOccurrence> stringOccurrences, IEnumerable<AdvOccurrence> advOccurrences,
            DbTransaction dbTran)
        {
            var stringOccurrencesList = stringOccurrences.GetActualItems().ToList();

            for (int i = stringOccurrencesList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringOccurrence(stringOccurrencesList[i], advOccurrences))
                    DeleteStringOccurrence(stringOccurrencesList[i], dbTran);

            foreach (var advOccurrence in advOccurrences)
                if (NeedCreateStringOccurrence(stringOccurrences, advOccurrence))
                    CreateStringOccurrence(stringId, advOccurrence, dbTran);
        }

        #endregion

        #region Delete

        private void DeleteStringOccurrences(IEnumerable<StringOccurrence> stringOccurrences, DbTransaction dbTran)
        {
            var stringOccurrencesList = stringOccurrences.GetActualItems().ToList();

            for (int i = stringOccurrencesList.Count - 1; i >= 0; i--)
                DeleteStringOccurrence(stringOccurrencesList[i], dbTran);
        }
        private bool NeedDeleteStringOccurrence(StringOccurrence stringOccurrence, IEnumerable<AdvOccurrence> advOccurrences)
        {
            return !advOccurrences.Any(
                o =>
                    o.Id == stringOccurrence.OccurrenceId &&
                    o.TypeId == stringOccurrence.TypeId &&
                    o.OrderBy == stringOccurrence.OrderBy);
        }
        private void DeleteStringOccurrence(StringOccurrence stringOccurrence, DbTransaction dbTran)
        {
            _repository.SetStringOccurrence(stringOccurrence, isActual: false, dbTran);
            _context.Entry(stringOccurrence).Reload();
        }

        #endregion
    }
}
