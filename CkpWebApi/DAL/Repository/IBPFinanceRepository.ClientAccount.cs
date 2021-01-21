using System;
using System.Data.Common;

namespace CkpWebApi.DAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        void GetLegalPersonAccountNumber(
            DbTransaction dbTran,
            int legalPersonId,
            ref string accountNumber);

        void SetAccount(
            DbTransaction dbTran,
            ref int id,
            string number,
            DateTime accountDate,
            int companyId,
            int legalPersonId,
            int cashId,
            int businessUnitId,
            int accountStatusId,
            int accountTypeId,
            float accountSum,
            float nds,
            string description,
            string additionalDescription,
            string request,
            byte[] accountSettings,
            bool printed,
            bool unloadedTo1C,
            float prepaidSum,
            float debtSum,
            int paymentAgentId,
            float paymentAgentCommissionSum,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId);

        void SetAccountSettings(
            DbTransaction dbTran,
            ref int id,
            int? accountId,
            int? legalPersonId,
            int bankId, // Идентификатор банка ЮЛ, выбранного по умолчанию
            int unloadingDateMethod, // Метод выбора даты выгрузки счета
            string additionalDescription,
            string accountDescription,
            string actDescription,
            string shortAccountPositionFormulation,
            string advanceAccountPositionFormulation,
            string contractNumber,
            string contractDate,
            bool showAdditionalDescription,
            bool showDetailed,
            bool showContractInCaption,
            bool showOkpo,
            bool showSupplier,
            bool showPositionName,
            bool showExitDate,
            bool showExitNumber,
            bool showContract,
            bool showDiscount,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId);

        void SetAccountOrder(
            DbTransaction dbTran,
            ref int id,
            int accountId,
            int orderId,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId);

        void SetAccountPosition(
            DbTransaction dbTran,
            ref int id,
            int accountId,
            string nomenclature,
            string positionName,
            int positionCount,
            float positionCost,
            float positionSum,
            float positionDiscount,
            DateTime? firstOutDate,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId);
    }
}
