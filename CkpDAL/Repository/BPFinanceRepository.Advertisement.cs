using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public void SetOrder(
            DbTransaction dbTran,
            ref int id,
            int? parentId,
            int activityTypeId,
            int businessUnitId,
            int statusId,
            bool visa,
            bool isNeedAccount,
            string accountDescription,
            string orderNumber,
            DateTime orderDate,
            DateTime? maxExitDate,
            int companyId,
            int clientLegalPersonId,
            int supplierLegalPersonId,
            float orderSum,
            float orderPaid,
            bool isCashless,
            bool isAdvance,
            bool isPaymentWithAgent,
            bool isFactoring,
            int createdPaymentPrognosisTypeId,
            int currentPaymentPrognosisTypeId,
            DateTime? paymentArbitaryPrognosisDate,
            string description,
            string request,
            int managerId,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetOrder";
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
                        ParameterName = "@ParentId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)parentId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ActitvityTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = activityTypeId
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
                        ParameterName = "@StatusId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = statusId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Visa",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = visa
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsNeedAccount",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isNeedAccount
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountDescription",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        SqlValue = accountDescription
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderNumber",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderNumber
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MaxExitDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)maxExitDate ?? DBNull.Value
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
                        ParameterName = "@ClientLegalPersonId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = clientLegalPersonId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@SupplierLegalPersonId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = supplierLegalPersonId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderSum",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderSum
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderPaid",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderPaid
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsCashless",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isCashless
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsAdvance",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isAdvance
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsPaymentWithAgent",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isPaymentWithAgent
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsFactoring",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isFactoring
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CreatedPaymentPrognosisTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = createdPaymentPrognosisTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CurrentPaymentPrognosisTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = currentPaymentPrognosisTypeId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentArbitaryPrognosisDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentArbitaryPrognosisDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        SqlValue = description
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
                        ParameterName = "@ManagerId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = managerId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
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

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        public void SetOrder(
            DbTransaction dbTran,
            ref int id,
            int activityTypeId,
            int businessUnitId,
            DateTime orderDate,
            DateTime? maxExitDate,
            int companyId,
            int clientLegalPersonId,
            int supplierLegalPersonId,
            float orderSum,
            int managerId,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate)
        {
            SetOrder(
                dbTran: dbTran,
                id: ref id,
                parentId: null,
                activityTypeId: activityTypeId,
                businessUnitId: businessUnitId,
                statusId: 0,
                visa: false,
                isNeedAccount: false,
                accountDescription: string.Empty,
                orderNumber: string.Empty,
                orderDate: orderDate,
                maxExitDate: maxExitDate,
                companyId: companyId,
                clientLegalPersonId: clientLegalPersonId,
                supplierLegalPersonId: supplierLegalPersonId,
                orderSum: orderSum,
                orderPaid: 0,
                isCashless: false,
                isAdvance: false,
                isPaymentWithAgent: false,
                isFactoring: false,
                createdPaymentPrognosisTypeId: 1,
                currentPaymentPrognosisTypeId: 1,
                paymentArbitaryPrognosisDate: null,
                description: string.Empty,
                request: string.Empty,
                managerId: managerId,
                editUserId: editUserId,
                isActual: isActual,
                lastEditDate: ref lastEditDate);
        }

        public void SetOrderPosition(
            DbTransaction dbTran,
            ref int id,
            int orderId,
            int? parentOrderPositionId,
            int supplierId,
            int priceId,
            int pricePositionId,
            DateTime pricePositionVersionDate,
            float clientDiscount,
            float markup,
            float nds,
            float compensation,
            string description,
            float clientPackageDiscount,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate,
            bool isUnloaded,
            bool needConfirmation)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetOrderPosition";
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
                        ParameterName = "@OrderId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ParentOrderPositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)parentOrderPositionId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@SupplierId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = supplierId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PriceId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = priceId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PricePositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = pricePositionId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PricePositionVersionDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = pricePositionVersionDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ClientDiscount",
                        SqlDbType = SqlDbType.Float,
                        Direction = ParameterDirection.Input,
                        SqlValue = clientDiscount
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Markup",
                        SqlDbType = SqlDbType.Float,
                        Direction = ParameterDirection.Input,
                        SqlValue = markup
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
                        ParameterName = "@Compensation",
                        SqlDbType = SqlDbType.Float,
                        Direction = ParameterDirection.Input,
                        SqlValue = compensation
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = description
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ClientPackageDiscount",
                        SqlDbType = SqlDbType.Float,
                        Direction = ParameterDirection.Input,
                        SqlValue = clientPackageDiscount
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
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
                        ParameterName = "@IsUnloaded",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isUnloaded
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@NeedConfirmation",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = needConfirmation
                    });


                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }

        }

        public void SetRubricPosition(
            DbTransaction dbTran,
            ref int id,
            int orderPositionId,
            int rubricId,
            DateTime rubricVersionDate,
            int editUserId,
            bool isActual)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetRubricToOrderPosition";
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
                        ParameterName = "@OrderPositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderPositionId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@RubricId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = rubricId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@RubricVersionDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = rubricVersionDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
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

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
            }
        }

        public void SetGraphicPosition(
            DbTransaction dbTran,
            ref int id,
            int parentGraphicPositionId,
            int orderPositionId,
            int graphicId,
            int countPosition,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetGraphicPosition";
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
                        ParameterName = "@ParentGraphicPositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = parentGraphicPositionId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderPositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderPositionId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@GraphicId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = graphicId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CountPosition",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = countPosition
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
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

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        private void SetPositionIm(
            DbTransaction dbTran,
            int id,
            int pricePositionTypeId,
            int orderId,
            int? parentPositionId,
            string text,
            string comments,
            string url,
            string distributionUrl,
            int? legalPersonPersonalDataId,
            string xml,
            int rating,
            string ratingDescription,
            ref DateTime taskFileDate,
            ref DateTime maketFileDate,
            ref DateTime lastEditDate,
            int editUserId,
            bool isActual,
            bool newTaskFile,
            bool newMaketFile,
            int maketStatusId,
            int maketCategoryId,
            bool dontVerify,
            bool rdvRating)
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
                        SqlValue = id
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PricePositionTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = pricePositionTypeId
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
                        ParameterName = "@ParentPositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)parentPositionId ?? DBNull.Value
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
                        SqlValue = (object)xml ?? DBNull.Value
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
                        ParameterName = "@TaskFileDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = taskFileDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MaketFileDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = maketFileDate
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

                cmd.ExecuteNonQuery();

                id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                taskFileDate = Convert.ToDateTime(cmd.Parameters["@TaskFileDate"].Value);
                maketFileDate = Convert.ToDateTime(cmd.Parameters["@MaketFileDate"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        private void SetOrderIm(
            DbTransaction dbTran,
            int id,
            int pricePositionTypeId,
            int maketStatusId,
            int maketCategoryId,
            bool needVerify,
            int designerId,
            string comments,
            DateTime maxClosingDate,
            bool viewed,
            ref DateTime lastEditDate,
            int editUserId,
            bool isActual)
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
                        SqlValue = id
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PricePositionTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = pricePositionTypeId
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
                        SqlValue = designerId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Comments",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = comments
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MaxClosingDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = maxClosingDate
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Viewed",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = viewed
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

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                cmd.ExecuteNonQuery();

                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }
    }
}
