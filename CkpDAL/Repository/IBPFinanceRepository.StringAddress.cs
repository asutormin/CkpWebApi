using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        public void SetStringCompanyAddress(
            DbTransaction dbTran,
            ref int id,
            int companyId,
            int? cityId,
            int? metroId,
            string street, // Улица
            string house, // Номер дома
            string corps, // Корпус
            string building, // Строение
            string description,
            bool isActual,
            int editUserId);

        public void SetStringAddress(
            DbTransaction dbTran,
            int stringId,
            int addressId,
            int? cityId,
            int? metroId,
            string street,
            string house,
            string corps,
            string building,
            string description,
            int orderBy,
            bool isActual);
    }
}
