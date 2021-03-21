using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        public void SetStringOccurrence(
            DbTransaction dbTran,
            int stringId,
            int occurrenceId,
            int typeId,
            int orderBy,
            bool isActual,
            int editUserId);
    }
}
