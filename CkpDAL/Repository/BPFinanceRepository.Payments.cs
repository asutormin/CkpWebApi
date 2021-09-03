using CkpDAL.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository : IBPFinanceRepository
    {
        public Payment SetPayment(Payment payment, bool isActual, DbTransaction dbTran)
        {
            var paymentId = payment.Id;
            var lastEditDate = payment.BeginDate;
            var number = payment.Number;

            SetPayment(
                dbTran: dbTran,
                id: ref paymentId,
                number: ref number,
                accountsNumber: payment.AccountsNumber,
                ordersNumber: payment.OrdersNumber,
                isDistribution: payment.IsDistribution,
                paymentTypeId: payment.PaymentTypeId,
                companyId: payment.CompanyId,
                businessUnitId: payment.BusinessUnitId,
                legalPersonId: payment.LegalPersonId,
                cashId: payment.CashId,
                summ: payment.Summ,
                undisposedSumm: payment.UndisposedSum,
                paymentDate: payment.PaymentDate,
                employeeId: payment.EmployeeId,
                paymentFoundation: payment.PaymentFoundation,
                description: payment.Description,
                paymentAgentId: payment.PaymentAgentId,
                editUserId: _editUserId,
                isActual: isActual,
                lastEditDate: ref lastEditDate);

            payment.Id = paymentId;
            payment.Number = number;
            payment.BeginDate = lastEditDate;

            return payment;
        }

        public OrderPayment SetOrderPayment(OrderPayment orderPayment, bool isActual, DbTransaction dbTran)
        {
            var orderPaymentId = orderPayment.Id;

            SetDistributionPaymentOrder(
                dbTran: dbTran,
                id: ref orderPaymentId,
                paymentId: orderPayment.PaymentId,
                orderId: orderPayment.OrderId,
                paidSum: orderPayment.PaidSum,
                distributionDate: orderPayment.DistributionDate,
                description: orderPayment.Description,
                isActual: isActual,
                editUserId: _editUserId);

            orderPayment.Id = orderPaymentId;

            return orderPayment;
        }

        public void ChangeOrderPaid(Order order, DbTransaction dbTran)
        {
            ChangeOrderPaid(
                dbTran: dbTran,
                id: order.Id,
                editUserId: _editUserId);
        }

        #region SQL StoredProcedures

        /// <summary>
        /// Сохранение платежа черех хранимую процедуру.
        /// </summary>
        /// <param name="dbTran">Транзакция, в которой происходит сохранение.</param>
        /// <param name="id">Идентификатор платежа / поступления.</param>
        /// <param name="number">Номер платежа / поступления.</param>
        /// <param name="accountsNumber">Номера счетов.</param>
        /// <param name="ordersNumber">Номерa заказов.</param>
        /// <param name="isDistribution">Является распределением другого платежа / поступления.</param>
        /// <param name="paymentTypeId">Тип платежа / поступлени.я</param>
        /// <param name="companyId">Идентификатор компании.</param>
        /// <param name="businessUnitId">Идентификатор бизнес еденицы.</param>
        /// <param name="legalPersonId">Идентификатор юр.лица компании.</param>
        /// <param name="cashId">Идентификатор кассы.</param>
        /// <param name="summ">Сумма поступления / платежа.</param>
        /// <param name="undisposedSumm">Нераспределенная сумма платежа / поступления.</param>
        /// <param name="paymentDate">Дата платежа.</param>
        /// <param name="employeeId">Идентификатор сотрудника (подотчетного лица).</param>
        /// <param name="paymentFoundation">Основание платежа / поступления.</param>
        /// <param name="description">Примечание.</param>
        /// <param name="paymentAgentId">Идентификатор платёжного агента.</param>
        /// <param name="editUserId">Идентификатор пользователя.</param>
        /// <param name="isActual">Признак удаления записи.</param>
        /// <param name="lastEditDate">Последняя дата редактирования.</param>
        private void SetPayment(
            DbTransaction dbTran,
            ref int id,
            ref string number,
            string accountsNumber,
            string ordersNumber,
            bool isDistribution,
            int paymentTypeId,
            int companyId,
            int businessUnitId,
            int legalPersonId,
            int cashId,
            float summ,
            float undisposedSumm,
            DateTime paymentDate,
            int? employeeId,
            string paymentFoundation,
            string description,
            int paymentAgentId,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetPayment";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                // Идентификатор платежа / поступления
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = id
                    });

                // Номер платежа / поступления
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Number",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = number
                    });

                // Номера счетов
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AccountsNumber",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)accountsNumber ?? DBNull.Value
                    });

                // Номера заказов
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrdersNumber",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)ordersNumber ?? DBNull.Value
                    });

                // Является распределением другого платежа / поступления
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsDistribution",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isDistribution
                    });

                // Тип платежа / поступления
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentTypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentTypeId
                    });

                // Идентификатор компании
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CompanyId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = companyId
                    });

                // Идентификатор бизнес еденицы
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@BusinessUnitId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = businessUnitId
                    });

                // Идентификатор юр.лица компании
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LegalPersonId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = legalPersonId
                    });

                // Идентификатор кассы
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CashId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = cashId
                    });

                // Сумма поступления / платежа
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Summ",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = summ
                    });

                // Нераспределенная сумма платежа / поступления
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@UndisposedSumm",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = undisposedSumm
                    });

                // Дата платежа
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentDate
                    });

                // Сотрудник (подотчетное лицо)
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EmployeeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)employeeId ?? DBNull.Value
                    });

                // Основание платежа / поступления
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentFoundation",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentFoundation
                    });

                // Примечание
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)description ?? DBNull.Value
                    });

                // Идентификатор платёжного агента
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentAgentId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentAgentId
                    });

                // Идентификатор пользователя
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                // Признак удаления записи
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                // Последняя дата редактирования
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
                number = Convert.ToString(cmd.Parameters["@Number"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }

        /// <summary>
        /// Сохранение распределения платежа на заказ.
        /// </summary>
        /// <param name="dbTran"></param>
        /// <param name="id">Идентификатор распределения платежа.</param>
        /// <param name="paymentId">Идентификатор распределяемого платежа.</param>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <param name="paidSum">Распределяемая сумма.</param>
        /// <param name="distributionDate">Дата распределения.</param>
        /// <param name="description">Описание распределения.</param>
        /// <param name="editUserId">Идентификатор пользователя.</param>
        /// <param name="isActual">Признак удаления записи.</param>
        private void SetDistributionPaymentOrder(
            DbTransaction dbTran,
            ref int id,
            int paymentId,
            int orderId,
            float paidSum,
            DateTime distributionDate,
            string description,
            int editUserId,
            bool isActual)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetDistributionPaymentOrder";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                // Идентификатор распределения платежа
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = id
                    });

                // Идентификатор распределяемого платежа
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaymentId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = paymentId
                    });

                // Идентификатор заказа
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderId
                    });

                // Распределяемая сумма
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@PaidSumm",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = paidSum
                    });

                // Дата распределения
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@DistributionDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = distributionDate
                    });

                // Описание распределения
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 300,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)description ?? DBNull.Value
                    });

                // Идентификатор пользователя
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                // Признак удаления записи
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

        /// <summary>
        /// Пересчитывает и сохраняет сумму оплаты заказа.
        /// </summary>
        /// <param name="dbTran">Транзакция, в которой происходит сохранение.</param>
        /// <param name="id">Идентификатор заказа.</param>
        /// <param name="editUserId">Идентификатор пользователя.</param>
        private void ChangeOrderPaid(
            DbTransaction dbTran,
            int id,
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spChangeOrderPaid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                // Идентификатор заказа
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = id
                    });

                // Идентификатор пользователя
                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
