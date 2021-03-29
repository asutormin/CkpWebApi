using CkpDAL.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository 
    {
        public Order SetOrder(Order order, bool isActual, DbTransaction dbTran)
        {
            var orderId = order.Id;
            var lastEditDate = order.BeginDate;

            SetOrder(
                dbTran: dbTran,
                id: ref orderId,
                parentId: order.ParentOrderId,
                activityTypeId: order.ActivityTypeId,
                businessUnitId: order.BusinessUnitId,
                statusId: order.StatusId,
                visa: order.IsNeedVisa,
                isNeedAccount: order.IsNeedAccount,
                accountDescription: order.AccountDescription,
                orderNumber: order.OrderNumber,
                orderDate: order.OrderDate,
                maxExitDate: order.MaxExitDate,
                companyId: order.ClientCompanyId,
                clientLegalPersonId: order.ClientLegalPersonId,
                supplierLegalPersonId: order.SupplierLegalPersonId,
                orderSum: order.Sum,
                orderPaid: order.Paid,
                isCashless: order.IsCashless,
                isAdvance: order.IsAdvance,
                isPaymentWithAgent: order.IsPaymentWithAgent,
                isFactoring: order.IsFactoring,
                createdPaymentPrognosisTypeId: order.CreatedPaymentPrognosisTypeId,
                currentPaymentPrognosisTypeId: order.CurrentPaymentPrognosisTypeId,
                paymentArbitaryPrognosisDate: order.PaymentArbitaryPrognosisDate,
                description: order.Description,
                request: order.Request,
                managerId: order.ManagerId,
                editUserId: _editUserId,
                isActual: isActual,
                lastEditDate: ref lastEditDate);

            order.Id = orderId;
            order.BeginDate = lastEditDate;

            return order;
        }

        public OrderPosition SetOrderPosition(OrderPosition orderPosition, bool isUnloaded, bool isActual, DbTransaction dbTran)
        {
            var orderPositionId = orderPosition.Id;
            var lastEditDate = orderPosition.BeginDate;

            SetOrderPosition(
                dbTran: dbTran,
                id: ref orderPositionId,
                orderId: orderPosition.OrderId,
                parentOrderPositionId: orderPosition.ParentOrderPositionId,
                supplierId: orderPosition.SupplierId,
                priceId: orderPosition.PriceId,
                pricePositionId: orderPosition.PricePositionId,
                pricePositionVersionDate: orderPosition.PricePositionVersion,
                clientDiscount: orderPosition.Discount,
                markup: orderPosition.Markup,
                nds: orderPosition.Nds,
                compensation: orderPosition.Compensation,
                description: orderPosition.Description,
                clientPackageDiscount: orderPosition.ClientPackageDiscount,
                editUserId: _editUserId,
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                isUnloaded: isUnloaded,
                needConfirmation: orderPosition.NeedConfirmation);

            orderPosition.Id = orderPositionId;
            orderPosition.BeginDate = lastEditDate;

            _context.Entry(orderPosition).Reload();

            return orderPosition;
        }

        public RubricPosition SetRubricPosition(RubricPosition rubricPosition, bool isActual, DbTransaction dbTran)
        {
            var rubricPositionId = rubricPosition.Id;

            SetRubricPosition(
                dbTran: dbTran,
                id: ref rubricPositionId,
                orderPositionId: rubricPosition.OrderPositionId,
                rubricId: rubricPosition.RubricId,
                rubricVersionDate: rubricPosition.RubricVersion,
                editUserId: _editUserId,
                isActual: isActual);

            rubricPosition.Id = rubricPositionId;

            return rubricPosition;
        }

        public GraphicPosition SetGraphicPosition(GraphicPosition graphicPosition, bool isActual, DbTransaction dbTran)
        {
            var graphicPositionId = graphicPosition.Id;
            var lastEditDate = graphicPosition.BeginDate;

            SetGraphicPosition(
                dbTran: dbTran,
                id: ref graphicPositionId,
                parentGraphicPositionId: graphicPosition.ParenGraphicPositiontId,
                orderPositionId: graphicPosition.OrderPositionId,
                graphicId: graphicPosition.GraphicId,
                countPosition: graphicPosition.Count,
                editUserId: _editUserId,
                isActual: isActual,
                lastEditDate: ref lastEditDate);

            graphicPosition.Id = graphicPositionId;
            graphicPosition.BeginDate = lastEditDate;

            return graphicPosition;
        }

        #region SQL StoredProcedures

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

        #endregion
    }
}
