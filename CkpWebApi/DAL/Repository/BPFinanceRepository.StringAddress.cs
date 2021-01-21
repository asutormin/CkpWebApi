﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace CkpWebApi.DAL.Repository
{
    public partial class BPFinanceRepository
    {
        public void SetStringCompanyAddress(
            DbTransaction dbTran,
            ref int id,
            int companyId,
            int? cityId,
            int? metroId,
            string street, // Улица
            string house, // Номер дома
            string corps, // Корпус
            string building, // Строение
            string description,
            bool isActual,
            int editUserId)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetStringCompanyAddress";
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
                        ParameterName = "@CityId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)cityId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MetroId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)metroId ?? DBNull.Value
                    });


                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Street",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 150,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(street) ? DBNull.Value : (object)street
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@House",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(house) ? DBNull.Value : (object)house
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Corps",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(corps) ? DBNull.Value : (object)corps
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Building",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(building) ? DBNull.Value : (object)building
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 500,
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

        public void SetStringAddress(
            DbTransaction dbTran,
            int stringId,
            int addressId,
            int? cityId,
            int? metroId,
            string street,
            string house,
            string corps,
            string building,
            string description,
            int orderBy,
            bool isActual)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetStringAddress";
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
                        ParameterName = "@AddressId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = addressId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CityId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)cityId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@MetroId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)metroId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Street",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 150,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(street) ? DBNull.Value : (object)street
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@House",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(house) ? DBNull.Value : (object)house
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Corps",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(corps) ? DBNull.Value : (object)corps
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Building",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 10,
                        Direction = ParameterDirection.Input,
                        SqlValue = string.IsNullOrEmpty(building) ? DBNull.Value : (object)building
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Description",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 500,
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
