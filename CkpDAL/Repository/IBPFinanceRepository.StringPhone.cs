using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        void SetStringCompanyPhone(
            DbTransaction dbTran,
            ref int id,
            int companyId,
            int phoneTypeId,
            string countryCode,
            string code,
            string number,
            string additionalNumber,
            string description,
            bool isActual,
            int editUserId);

        void SetStringPhone(
            DbTransaction dbTran,
            int stringId,
            int phoneId,
            int? phoneTypeId,
            string countryCode,
            string code,
            string number,
            string additionalNumber,
            string description,
            int orderBy,
            bool isActual);
    }
}
