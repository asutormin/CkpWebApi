using CkpDAL.Entities.Module;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public ModulePosition SetModule(ModulePosition modulePosition, bool isActual, DbTransaction dbTran)
        {
            var moduleId = modulePosition.Id;
            var lastEditDate = modulePosition.BeginDate;

            SetModule(
                dbTran: dbTran,
                moduleId: ref moduleId,
                companyId: modulePosition.CompanyId,
                businessUnitId: modulePosition.BusinessUnitId,
                orderPositionId: modulePosition.OrderPositionId,
                vacancyName: modulePosition.VacancyName,
                editUserId: _editUserId,
                isActual: isActual,
                lastEditDate: ref lastEditDate);

            modulePosition.Id = moduleId;
            modulePosition.BeginDate = lastEditDate;

            return modulePosition;
        }

        #region SQL StoredProcedures

        private void SetModule(
            DbTransaction dbTran,
            ref int moduleId, // Идентификатор модуля
            int companyId, // Идентификатро компании
            int businessUnitId, // Идентификатор бизнес еденицы
            int orderPositionId,
            string vacancyName,
            int editUserId, // пользователь изменивший запись
            bool isActual, // Закрыть или обновить запись
            ref DateTime lastEditDate) // Дата последнего редактирования записи
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetModule";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = moduleId
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
                        ParameterName = "@BusinessUnitId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = businessUnitId
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
                        ParameterName = "@VacancyName",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 500,
                        Direction = ParameterDirection.Input,
                        SqlValue = vacancyName
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

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LastEditDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = lastEditDate
                    });

                cmd.ExecuteNonQuery();

                moduleId = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }

        }
        #endregion
    }
}
