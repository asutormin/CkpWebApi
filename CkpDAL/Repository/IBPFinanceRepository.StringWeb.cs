using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        public void SetStringCompanyWeb(
          DbTransaction dbTran,
          ref int id,
          int companyId,
          int webTypeId,
          int webResponse,
          string webValue,
          string description,
          bool isActual,
          int editUserId);

        public void SetStringWeb(
            DbTransaction dbTran,
            int stringId,
            int webId,
            int webTypeId,
            int webResponse,
            string webValue,
            string description,
            int orderBy,
            bool isActual);
    }
}
