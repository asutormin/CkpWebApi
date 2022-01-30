using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using CkpDAL.Entities;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public string GetAccountNumber(int legalPersonId, DbTransaction dbTran)
        {
            var accountNumber = string.Empty;

            GetLegalPersonAccountNumber(
                dbTran: dbTran,
                legalPersonId: legalPersonId,
                accountNumber: ref accountNumber);

            return accountNumber;
        }

        public Account SetAccount(Account account, bool isActual, DbTransaction dbTran)
        {
            var accountId = account.Id;
            var lastEditDate = account.BeginDate;

            SetAccount(
                dbTran: dbTran,
                id: ref accountId,
                number: account.Number,
                accountDate: account.Date,
                companyId: account.CompanyId,
                legalPersonId: account.LegalPersonId,
                cashId: account.CashId,
                businessUnitId: account.BusinessUnitId,
                accountStatusId: account.StatusId,
                accountTypeId: account.TypeId,
                accountSum: account.Sum,
                nds: account.Nds,
                description: account.Description,
                additionalDescription: account.AdditionalDescription,
                request: account.Request,
                accountSettings: null,
                printed: account.Printed,
                unloadedTo1C: account.UnloadedTo1C,
                prepaidSum: account.Prepaid,
                debtSum: account.Debt,
                paymentAgentId: account.PaymentAgentId, // Отсутствует
                paymentAgentCommissionSum: account.PaymentAgentCommissionSum,
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            account.Id = accountId;
            account.BeginDate = lastEditDate;

            return account;
        }

        public AccountPosition SetAccountPosition(AccountPosition accountPosition, bool isActual, DbTransaction dbTran)
        {
            var accountPositionId = accountPosition.Id;
            var lastEditDate = accountPosition.BeginDate;

            SetAccountPosition(
                dbTran: dbTran,
                id: ref accountPositionId,
                accountId: accountPosition.AccountId,
                nomenclature: accountPosition.Nomenclature,
                positionName: accountPosition.Name,
                positionCount: accountPosition.Count,
                positionCost: accountPosition.Cost,
                positionSum: accountPosition.Sum,
                positionDiscount: accountPosition.Discount,
                firstOutDate: accountPosition.FirstOutDate,
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            accountPosition.Id = accountPositionId;
            accountPosition.BeginDate = lastEditDate;

            return accountPosition;
        }

        public AccountSettings SetAccountSettings(AccountSettings accountSettings, bool isActual, DbTransaction dbTran)
        {
            var accountSettingsId = accountSettings.Id;
            var lastEditDate = accountSettings.BeginDate;

            SetAccountSettings(
                dbTran: dbTran,
                id: ref accountSettingsId,
                accountId: accountSettings.AccountId,
                legalPersonId: accountSettings.LegalPersonId,
                bankId: accountSettings.LegalPersonBankId,
                unloadingDateMethod: accountSettings.UnloadingDateMethod,
                unloadingTo1CTypeId: accountSettings.UnloadingTypeId,
                unloadingTo1CDayNumber: accountSettings.UnloadingDayNumber,
                unloadingTo1CActionId: accountSettings.UnloadingTo1CActionId,
                additionalDescription: accountSettings.AdditionalDescription,
                accountDescription: accountSettings.AccountDescription,
                actDescription: accountSettings.ActDescription,
                shortAccountPositionFormulation: accountSettings.ShortAccountPositionFormulation,
                advanceAccountPositionFormulation: accountSettings.AdvanceAccountPositionFormulation,
                contractNumber: accountSettings.ContractNumber,
                contractDate: accountSettings.ContractDate,
                showAdditionalDescription: accountSettings.ShowAdditionalDescription,
                showDetailed: accountSettings.ShowDetailed,
                showContractInCaption: accountSettings.ShowContractInCaption,
                showOkpo: accountSettings.ShowOkpo,
                showSupplier: accountSettings.ShowSupplier,
                showPositionName: accountSettings.ShowPositionName,
                showExitDate: accountSettings.ShowExitDate,
                showExitNumber: accountSettings.ShowExitNumber,
                showContract: accountSettings.ShowContract,
                showDiscount: accountSettings.ShowDiscount,
                addressId: accountSettings.AddressId,
                isNeedPrepayment: accountSettings.IsNeedPrepayment,
                interactionBusinessUnitId: accountSettings.InteractionBusinessUnitId,
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            accountSettings.Id = accountSettingsId;
            accountSettings.BeginDate = lastEditDate;

            return accountSettings;
        }

        public AccountOrder SetAccountOrder(AccountOrder accountOrder, bool isActual, DbTransaction dbTran)
        {
            var accountOrderId = accountOrder.Id;
            var lastEditDate = accountOrder.BeginDate;

            SetAccountOrder(
                dbTran: dbTran,
                id: ref accountOrderId,
                accountId: accountOrder.AccountId,
                orderId: accountOrder.OrderId,
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            accountOrder.Id = accountOrderId;
            accountOrder.BeginDate = lastEditDate;

            return accountOrder;
        }

        #region SQL StoredProcedures

        public void GetLegalPersonAccountNumber(
            int legalPersonId,
            ref string accountNumber,
            DbTransaction dbTran)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spGetLegalPersonAccountNumber";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LegalPersonId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = legalPersonId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountNumber",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = accountNumber
                    });

                cmd.ExecuteNonQuery();

                accountNumber = Convert.ToString(cmd.Parameters["@AccountNumber"].Value);
            }
        }

        private void SetAccount(
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
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetAccount";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = id
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Number",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = number
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CompanyId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = companyId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LegalPersonId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = legalPersonId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CashId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = cashId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@BusinessUnitId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = businessUnitId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountStatusId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountStatusId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountSum",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountSum
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Nds",
                        SqlDbType = SqlDbType.Float,
                        Direction = ParameterDirection.Input,
                        SqlValue = nds
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)description ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AdditionalDescription",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)additionalDescription ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Request",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)request ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountSettings",
                        SqlDbType = SqlDbType.Image,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)accountSettings ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Printed",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = printed
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@UnloadedTo1C",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = unloadedTo1C
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PrepaidSum",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = prepaidSum
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@DebtSum",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = debtSum
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentAgentId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentAgentId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentAgentCommissionSum",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentAgentCommissionSum
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LastEditDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = lastEditDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        public void SetAccountPosition(
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
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetAccountPosition";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = id
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Nomenclature",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        SqlValue = nomenclature
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PositionName",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        SqlValue = positionName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PositionCount",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = positionCount
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PositionCost",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = positionCost
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PositionSum",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = positionSum
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PositionDiscount",
                        SqlDbType = SqlDbType.Float,
                        Direction = ParameterDirection.Input,
                        SqlValue = positionDiscount
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@FirstOutDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)firstOutDate ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LastEditDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = lastEditDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        private void SetAccountSettings(
            DbTransaction dbTran,
            ref int id,
            int? accountId,
            int? legalPersonId,
            int bankId, // Идентификатор банка ЮЛ, выбранного по умолчанию
            int unloadingDateMethod, // Метод выбора даты выгрузки счета
            int unloadingTo1CTypeId, // Тип выгрузки в 1С ("День в день", "По факту", "Раз в месяц")
            int unloadingTo1CDayNumber, // Какого числа выгружать в 1С
            int unloadingTo1CActionId, // Идентификатор действия при загрузке в 1С
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
            int addressId,
            bool isNeedPrepayment,
            int interactionBusinessUnitId,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetAccountSetting";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = id
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)accountId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LegalPersonId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)legalPersonId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@BankId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = bankId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@UnloadingDateMethod",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = unloadingDateMethod
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@UnloadingTo1CTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = unloadingTo1CTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@UnloadingTo1CDayNumber",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = unloadingTo1CDayNumber
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@UnloadingTo1CActionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = unloadingTo1CActionId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AdditionalDescription",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 500,
                        Direction = ParameterDirection.Input,
                        SqlValue = additionalDescription
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountDescription",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 1000,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountDescription
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ActDescription",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 1000,
                        Direction = ParameterDirection.Input,
                        SqlValue = actDescription
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShortAccountPositionFormulation",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 150,
                        Direction = ParameterDirection.Input,
                        SqlValue = shortAccountPositionFormulation
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AdvanceAccountPositionFormulation",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 150,
                        Direction = ParameterDirection.Input,
                        SqlValue = advanceAccountPositionFormulation
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ContractNumber",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 20,
                        Direction = ParameterDirection.Input,
                        SqlValue = contractNumber
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ContractDate",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = contractDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowAdditionalDescription",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showAdditionalDescription
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowDetailed",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showDetailed
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowContractInCaption",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showContractInCaption
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowOkpo",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showOkpo
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowSupplier",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showSupplier
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowPositionName",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showPositionName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowExitDate",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showExitDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowExitNumber",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showExitNumber
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowContract",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showContract
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ShowDiscount",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = showDiscount
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AddressId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = addressId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsNeedPrepayment",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isNeedPrepayment
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CkpressInteractionBusinessUnitId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = interactionBusinessUnitId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LastEditDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = lastEditDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        public void SetAccountOrder(
            DbTransaction dbTran,
            ref int id,
            int accountId,
            int orderId,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetAccountOrder";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = id
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LastEditDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = lastEditDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        #endregion
    }
}
