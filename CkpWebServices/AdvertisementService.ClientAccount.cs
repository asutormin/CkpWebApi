using CkpDAL.Model;
using CkpServices.Helpers;
using System;
using System.Data.Common;

namespace CkpServices
{
    public partial class AdvertisementService
    {
        private string GetAccountNumber(int legalPersonId, DbTransaction dbTran)
        {
            var accountNumber = string.Empty;

            _repository.GetLegalPersonAccountNumber(
                dbTran: dbTran,
                legalPersonId: legalPersonId,
                accountNumber: ref accountNumber);

            return accountNumber;
        }

        private int CreateAccount(string number, float sum, BusinessUnit businessUnit, LegalPerson clientLegalPerson, DbTransaction dbTran)
        {
            var accountId = 0;
            var accountDate = DateTime.Now;
            var nds = businessUnit.AccountsWithNds ? 20 : 0;
            var lastEditDate = DateTime.Now;

            _repository.SetAccount(
                dbTran: dbTran,
                id: ref accountId,
                number: number,
                accountDate: accountDate,
                companyId: clientLegalPerson.CompanyId,
                legalPersonId: clientLegalPerson.Id,
                cashId: businessUnit.CashId,
                businessUnitId: businessUnit.Id,
                accountStatusId: 3,
                accountTypeId: 1,
                accountSum: sum,
                nds: nds,
                description: null,
                additionalDescription: null,
                request: null,
                accountSettings: null,
                printed: false,
                unloadedTo1C: false,
                prepaidSum: 0,
                debtSum: 0,
                paymentAgentId: 1, // Отсутствует
                paymentAgentCommissionSum: 0,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            return accountId;
        }

        private void CreateAccountPosition(int accountId, OrderPosition orderPosition, DbTransaction dbTran)
        {
            var accountPositionId = 0;
            var nomenclature = orderPosition.GetAccountNumenclature();
            var positionName = orderPosition.GetAccountPositionName();
            var positionCount = orderPosition.GetPositionsCount();
            var positionSum = orderPosition.GetClientSum();
            var positionCost = positionSum / positionCount;
            var firstOutDate = orderPosition.GetFirstOutDate();
            var lastEditDate = DateTime.Now;

            _repository.SetAccountPosition(
                dbTran: dbTran,
                id: ref accountPositionId,
                accountId: accountId,
                nomenclature: nomenclature,
                positionName: positionName,
                positionCount: positionCount,
                positionCost: positionCost,
                positionSum: positionSum,
                positionDiscount: 0,
                firstOutDate: firstOutDate,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);
        }

        private void CreateAccountSettings(int accountId, AccountSettings legalPersonAccountSettings, DbTransaction dbTran)
        {
            var accountSettingsId = 0;
            var lastEditDate = DateTime.Now;

            _repository.SetAccountSettings(
                dbTran: dbTran,
                id: ref accountSettingsId,
                accountId: accountId,
                legalPersonId: null,
                bankId: legalPersonAccountSettings.LegalPersonBankId,
                unloadingDateMethod: legalPersonAccountSettings.UnloadingDateMethod,
                additionalDescription: legalPersonAccountSettings.AdditionalDescription,
                accountDescription: legalPersonAccountSettings.AccountDescription,
                actDescription: legalPersonAccountSettings.ActDescription,
                shortAccountPositionFormulation: legalPersonAccountSettings.ShortAccountPositionFormulation,
                advanceAccountPositionFormulation: legalPersonAccountSettings.AdvanceAccountPositionFormulation,
                contractNumber: legalPersonAccountSettings.ContractNumber,
                contractDate: legalPersonAccountSettings.ContractDate,
                showAdditionalDescription: legalPersonAccountSettings.ShowAdditionalDescription,
                showDetailed: legalPersonAccountSettings.ShowDetailed,
                showContractInCaption: legalPersonAccountSettings.ShowContractInCaption,
                showOkpo: legalPersonAccountSettings.ShowOkpo,
                showSupplier: legalPersonAccountSettings.ShowSupplier,
                showPositionName: legalPersonAccountSettings.ShowPositionName,
                showExitDate: legalPersonAccountSettings.ShowExitDate,
                showExitNumber: legalPersonAccountSettings.ShowExitNumber,
                showContract: legalPersonAccountSettings.ShowContract,
                showDiscount: legalPersonAccountSettings.ShowDiscount,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);
        }

        private void CreateAccountOrder(int accountId, int orderId, DbTransaction dbTran)
        {
            var accountOrderId = 0;
            var lastEditDate = DateTime.Now;

            _repository.SetAccountOrder(
                dbTran: dbTran,
                id: ref accountOrderId,
                accountId: accountId,
                orderId: orderId,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);
        }
    }
}
