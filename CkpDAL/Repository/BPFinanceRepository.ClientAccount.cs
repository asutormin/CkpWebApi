using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public void GetLegalPersonAccountNumber(
            DbTransaction dbTran,
            int legalPersonId,
            ref string accountNumber)
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

        public void SetAccount(
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

        public void SetAccountSettings(
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
    }
}
