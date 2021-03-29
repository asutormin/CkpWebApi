using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using CkpDAL.Model.String;

namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository
    {
        public StringOccurrence SetStringOccurrence(StringOccurrence stringOccurrence, bool isActual, DbTransaction dbTran)
        {
            SetStringOccurrence(
                dbTran: dbTran,
                stringId: stringOccurrence.StringId,
                occurrenceId: stringOccurrence.OccurrenceId,
                typeId: stringOccurrence.TypeId,
                orderBy: stringOccurrence.OrderBy,
                isActual: isActual,
                editUserId: _editUserId);

            return stringOccurrence;
        }

        #region SQL StoredProcedures

        private void SetStringOccurrence(
            DbTransaction dbTran,
            int stringId,
            int occurrenceId,
            int typeId,
            int orderBy,
            bool isActual,
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetStringOccurrence";
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
                        ParameterName = "@OccurrenceId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = occurrenceId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@TypeId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = typeId
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
