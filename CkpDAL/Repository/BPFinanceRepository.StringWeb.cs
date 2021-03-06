using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using CkpDAL.Entities.String;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public Web SetWeb(Web web, bool isActual, DbTransaction dbTran)
        {
            var webId = web.Id;

            SetStringCompanyWeb(
                dbTran: dbTran,
                id: ref webId,
                companyId: web.CompanyId,
                webTypeId: web.WebTypeId,
                webResponse: web.WebResponse,
                webValue: web.WebValue,
                description: web.Description,
                isActual: isActual,
                editUserId: _editUserId);

            web.Id = webId;

            return web;
        }

        public StringWeb SetStringWeb(StringWeb stringWeb, bool isActual, DbTransaction dbTran)
        {
            SetStringWeb(
                dbTran: dbTran,
                stringId: stringWeb.StringId,
                webId: stringWeb.WebId,
                webTypeId: stringWeb.WebTypeId,
                webResponse: stringWeb.WebResponse,
                webValue: stringWeb.WebValue,
                description: stringWeb.Description,
                orderBy: stringWeb.OrderBy,
                isActual: isActual);

            return stringWeb;
        }

        #region SQL StoredProcedures

        private void SetStringCompanyWeb(
          DbTransaction dbTran,
          ref int id,
          int companyId,
          int webTypeId,
          int webResponse,
          string webValue,
          string description,
          bool isActual,
          int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetStringCompanyWeb";
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
                        ParameterName = "@WebTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = webTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@WebResponse",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = webResponse
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@WebValue",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        Direction = ParameterDirection.Input,
                        SqlValue = webValue
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

        public void SetStringWeb(
            DbTransaction dbTran,
            int stringId,
            int webId,
            int webTypeId,
            int webResponse,
            string webValue,
            string description,
            int orderBy,
            bool isActual)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetStringWeb";
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
                        ParameterName = "@WebId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = webId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@WebTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = webTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@WebResponse", //
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = webResponse
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@WebValue",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        Direction = ParameterDirection.Input,
                        SqlValue = webValue
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

        #endregion
    }
}
