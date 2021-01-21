using CkpWebApi.DAL.Model.String;
using CkpWebApi.Helpers;
using CkpWebApi.InputEntities.String;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpWebApi.Services
{
    public partial class AdvertisementService
    {
        #region Create

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

        private void CreateStringOccurrence(int stringId, AdvOccurrence advOccurrence, DbTransaction dbTran)
        {
            _repository.SetStringOccurrence(
                dbTran: dbTran,
                stringId: stringId,
                occurrenceId: advOccurrence.Id,
                typeId: advOccurrence.TypeId,
                orderBy: advOccurrence.OrderBy,
                isActual: true,
                editUserId: _editUserId);

            SetNeedRefreshOrderShoppingCartOrder();
        }

        private void CreateStringOccurrences(int stringId, IEnumerable<AdvOccurrence> advOcurrences, DbTransaction dbTran)
        {
            foreach (var advOccurence in advOcurrences)
                CreateStringOccurrence(stringId, advOccurence, dbTran);
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
            _repository.SetStringOccurrence(
                dbTran: dbTran,
                stringId: stringOccurrence.StringId,
                occurrenceId: stringOccurrence.OccurrenceId,
                typeId: stringOccurrence.TypeId,
                orderBy: stringOccurrence.OrderBy,
                isActual: false,
                editUserId: _editUserId);

            _context.Entry(stringOccurrence).Reload();
            SetNeedRefreshOrderShoppingCartOrder();
        }

        private void DeleteStringOccurrences(IEnumerable<StringOccurrence> stringOccurrences, DbTransaction dbTran)
        {
            var stringOccurrencesList = stringOccurrences.GetActualItems().ToList();

            for (int i = stringOccurrencesList.Count - 1; i >= 0; i--)
                DeleteStringOccurrence(stringOccurrencesList[i], dbTran);
        }

        #endregion
    }
}
