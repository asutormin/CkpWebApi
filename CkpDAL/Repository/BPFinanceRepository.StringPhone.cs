using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public void SetStringCompanyPhone(
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
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetStringCompanyPhone";
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
                        ParameterName = "@CompanyId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = companyId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PhoneTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = phoneTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CountryCode",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = countryCode
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Code",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(code) ? DBNull.Value : (object)code
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
                        ParameterName = "@AdditionalNumber",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(additionalNumber) ? DBNull.Value : (object)additionalNumber
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(description) ? DBNull.Value : (object)description
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
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
            }
        }

        public void SetStringPhone(
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
            bool isActual)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetStringPhone";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@StringId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = stringId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PhoneId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = phoneId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PhoneTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)phoneTypeId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CountryCode",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = countryCode
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Code",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(code) ? DBNull.Value : (object)code
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
                        ParameterName = "@AdditionalNumber",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(additionalNumber) ? DBNull.Value : (object)additionalNumber
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(description) ? DBNull.Value : (object)description
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderBy",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderBy
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                cmd.ExecuteNonQuery();
            }
        }
    }
}
