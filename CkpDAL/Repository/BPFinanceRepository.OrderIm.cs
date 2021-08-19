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
        public OrderIm SetOrderIm(OrderIm orderIm, bool isActual, DbTransaction dbTran)
        {
            var lastEditDate = orderIm.BeginDate;

            SetOrderIm(
                dbTran: dbTran,
                orderId: orderIm.OrderId,
                orderImTypeId: orderIm.OrderImTypeId,
                maketStatusId: orderIm.MaketStatusId,
                maketCategoryId: orderIm.MaketCategoryId,
                replaceStatusId: orderIm.ImReplaceStatusId,
                brief: orderIm.Brief,
                needVisa: orderIm.NeedVisa,
                needVerify: orderIm.NeedVerify,
                designerId: orderIm.DesignerId,
                comments: orderIm.Comments,
                maxClosingDate: orderIm.MaxClosingDate,
                isViewed: orderIm.IsViewed,
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            orderIm.BeginDate = lastEditDate;

            return orderIm;
        }

        public PositionIm SetPositionIm(
            PositionIm positionIm,
            bool newTaskFile, bool newMaketFile,
            bool isActual, DbTransaction dbTran)
        {
            DateTime? taskFileDate = positionIm.TaskFileDate;
            DateTime? maketFileDate = positionIm.MaketFileDate;

            var lastEditDate = DateTime.Now;

            SetPositionIm(
                dbTran: dbTran,
                orderId: positionIm.OrderId,
                orderPositionId: positionIm.OrderPositionId,
                positionImTypeId: positionIm.PositionImTypeId,
                parentPositionId: positionIm.ParentPositionId,
                maketStatusId: positionIm.MaketStatusId,
                maketCategoryId: positionIm.MaketCategoryId,
                text: positionIm.Text,
                comments: positionIm.Comments,
                url: positionIm.Url,
                distributionUrl: positionIm.DistributionUrl,
                legalPersonPersonalDataId: positionIm.LegalPersonPersonalDataId,
                xml: positionIm.Xml,
                rating: positionIm.Rating,
                ratingDescription: positionIm.RatingDescription,
                newTaskFile: newTaskFile,
                newMaketFile: newMaketFile,
                taskFileDate: ref taskFileDate,
                maketFileDate: ref maketFileDate,
                dontVerify: positionIm.DontVerify,
                rdvRating: positionIm.RdvRating,
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            positionIm.TaskFileDate = taskFileDate;
            positionIm.MaketFileDate = maketFileDate;
            positionIm.BeginDate = lastEditDate;

            return positionIm;
        }

        #region SQL StoredProcedures

        private void SetOrderIm(
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

        private void SetPositionIm(
            DbTransaction dbTran,
            int orderId,
            int orderPositionId,
            int positionImTypeId,
            int? parentPositionId,
            int maketStatusId,
            int maketCategoryId,
            string text,
            string comments,
            string url,
            string distributionUrl,
            int? legalPersonPersonalDataId,
            string xml,
            int rating,
            string ratingDescription,
            bool newTaskFile,
            bool newMaketFile,
            ref DateTime? taskFileDate,
            ref DateTime? maketFileDate,
            bool dontVerify,
            bool rdvRating,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetPositionIM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderPositionId
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
                        ParameterName = "@PricePositionTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = positionImTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ParentPositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)parentPositionId ?? DBNull.Value
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
                        ParameterName = "@Text",
                        SqlDbType = SqlDbType.Text,
                        Direction = ParameterDirection.Input,
                        SqlValue = text
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Comments",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 4000,
                        Direction = ParameterDirection.Input,
                        SqlValue = comments
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Url",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 500,
                        Direction = ParameterDirection.Input,
                        SqlValue = url
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@DistributionUrl",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        Direction = ParameterDirection.Input,
                        SqlValue = distributionUrl
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LegalPersonPersonalDataId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)legalPersonPersonalDataId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Xml",
                        SqlDbType = SqlDbType.Text,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(xml) ? DBNull.Value : (object)xml
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Rating",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = rating
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@RatingDescription",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 500,
                        Direction = ParameterDirection.Input,
                        SqlValue = ratingDescription
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@NewTaskFile",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = newTaskFile
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@NewMaketFile",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = newMaketFile
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@TaskFileDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = (object)taskFileDate ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MaketFileDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = (object)maketFileDate ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@DontVerify",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = dontVerify
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@RdvRating",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = rdvRating
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

                taskFileDate = cmd.Parameters["@TaskFileDate"].Value == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(cmd.Parameters["@TaskFileDate"].Value);

                maketFileDate = cmd.Parameters["@MaketFileDate"].Value == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(cmd.Parameters["@MaketFileDate"].Value);

                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        #endregion
    }
}
