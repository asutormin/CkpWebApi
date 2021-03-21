using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public void SetOrderIm(
            DbTransaction dbTran,
            int orderId,
            int orderImTypeId,
            int maketStatusId,
            int maketCategoryId,
            int replaceStatusId,
            string brief,
            bool needVisa,
            bool needVerify,
            int? designerId,
            string comments,
            DateTime? maxClosingDate,
            bool isViewed,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetOrderIM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PricePositionTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderImTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MaketStatusId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = maketStatusId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MaketCategoryId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = maketCategoryId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ReplaceStatusId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = replaceStatusId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Brief",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 3000,
                        Direction = ParameterDirection.Input,
                        SqlValue = brief
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Visa",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = needVisa
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@NeedVerify",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = needVerify
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@DesignerId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)designerId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Comments",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)comments ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MaxClosingDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)maxClosingDate ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Viewed",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isViewed
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

                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }
    }
}
