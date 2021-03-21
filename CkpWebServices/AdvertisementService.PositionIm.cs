﻿using CkpDAL.Model;
using CkpEntities.Input;
using System;
using System.Data.Common;

namespace CkpServices
{
    public partial class AdvertisementService
    {
        #region Create

        private bool NeedReCreatePositionIm(PositionIm positionIm, Advertisement adv)
        {
            return positionIm != null && positionIm.OrderPosition.PricePositionId != adv.Format.Id;
        }

        private void CreateStringPositionIm(int orderId, int orderPositionId, DbTransaction dbTran)
        {
            var positionImTypeId = 1;

            DateTime? taskFileDate = null;
            DateTime? maketFileDate = null;

            var lastEditDate = DateTime.Now;

            _repository.SetPositionIm(
                dbTran: dbTran,
                orderId: orderId,
                orderPositionId: orderPositionId,
                positionImTypeId: positionImTypeId,
                parentPositionId: null,
                maketStatusId: 1, // 1 - Черновик, 3 - Вёрстка, 4 - Готов
                maketCategoryId: 2,
                text: string.Empty,
                comments: string.Empty,
                url: string.Empty,
                distributionUrl: string.Empty,
                legalPersonPersonalDataId: null,
                xml: null,
                rating: 0,
                ratingDescription: string.Empty,
                newTaskFile: false,
                newMaketFile: false,
                taskFileDate: ref taskFileDate,
                maketFileDate: ref maketFileDate,
                dontVerify: false,
                rdvRating: false,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);
        }

        private DateTime? CreateModulePositionImTaskVersion(int orderId, int orderPositionId, DbTransaction dbTran)
        {
            var positionImTypeId = 2;

            DateTime? taskFileDate = null;
            DateTime? maketFileDate = null;

            var lastEditDate = DateTime.Now;

            _repository.SetPositionIm(
                dbTran: dbTran,
                orderId: orderId,
                orderPositionId: orderPositionId,
                positionImTypeId: positionImTypeId,
                parentPositionId: null,
                maketStatusId: 1, // 1 - Черновик, 4 - Готово
                maketCategoryId: 2,
                text: string.Empty,
                comments: string.Empty,
                url: string.Empty,
                distributionUrl: string.Empty,
                legalPersonPersonalDataId: null,
                xml: null,
                rating: 0,
                ratingDescription: string.Empty,
                newTaskFile: true,
                newMaketFile: false,
                taskFileDate: ref taskFileDate,
                maketFileDate: ref maketFileDate,
                dontVerify: false,
                rdvRating: false,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            return taskFileDate;
        }

        private void CreateFullPositionIm(int orderId, int orderPositionId, Advertisement adv, DbTransaction dbTran)
        {
            if (adv.String != null)
            {
                CreateStringPositionIm(orderId, orderPositionId, dbTran);
                CreateFullString(adv.ClientId, orderPositionId, adv.String, dbTran);
                CreateOrUpdateOrderIm(orderId, orderImTypeId: 1, dbTran);
            }

            if (adv.Module != null)
            {
                var bytes = GetBase64Bytes(adv.Module.Base64String);

                CreateModulePositionIm(orderId, orderPositionId, bytes, dbTran);
                CreateModuleGraphics(orderPositionId, adv.Module.Name, bytes);
                CreateOrUpdateOrderIm(orderId, orderImTypeId: 2, dbTran);
            }
        }

        private void CreateOrUpdateOrderIm(int orderId, int orderImTypeId, DbTransaction dbTran)
        {
            var orderIm = GetOrderIm(orderId, orderImTypeId);

            if (orderIm == null)
                CreateOrderIm(orderId, orderImTypeId, dbTran);
            else
                SetOrderIm(orderIm, true, dbTran);
        }

        #endregion

        #region Update

        private void UpdatePositionIm(PositionIm positionIm, DbTransaction dbTran)
        {
            DateTime? taskFileDate = positionIm.TaskFileDate;
            DateTime? maketFileDate = positionIm.MaketFileDate;

            var lastEditDate = DateTime.Now;

            _repository.SetPositionIm(
                dbTran: dbTran,
                orderId: positionIm.OrderId,
                orderPositionId: positionIm.OrderPositionId,
                positionImTypeId: positionIm.PositionImTypeId,
                parentPositionId: positionIm.ParentPositionId,
                maketStatusId: positionIm.MaketStatusId, // Готов
                maketCategoryId: positionIm.MaketCategoryId,
                text: positionIm.Text,
                comments: positionIm.Comments,
                url: positionIm.Url,
                distributionUrl: positionIm.DistributionUrl,
                legalPersonPersonalDataId: positionIm.LegalPersonPersonalDataId,
                xml: positionIm.Xml,
                rating: positionIm.Rating,
                ratingDescription: positionIm.RatingDescription,
                newTaskFile: false,
                newMaketFile: false,
                taskFileDate: ref taskFileDate,
                maketFileDate: ref maketFileDate,
                dontVerify: positionIm.DontVerify,
                rdvRating: positionIm.RdvRating,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);
        }

        private DateTime? UpdateModulePositionImTaskVersion(PositionIm positionIm, DbTransaction dbTran)
        {
            DateTime? taskFileDate = null;
            DateTime? maketFileDate = null;

            var lastEditDate = DateTime.Now;

            _repository.SetPositionIm(
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
                newTaskFile: true,
                newMaketFile: false,
                taskFileDate: ref taskFileDate,
                maketFileDate: ref maketFileDate,
                dontVerify: positionIm.DontVerify,
                rdvRating: positionIm.RdvRating,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            return taskFileDate;
        }

        private void UpdateFullPositionIm(PositionIm positionIm, Advertisement adv, DbTransaction dbTran)
        {
            if (NeedReCreatePositionIm(positionIm, adv))
            {
                DeleteFullPositionIm(positionIm, dbTran);
                CreateFullPositionIm(adv.OrderId, adv.OrderPositionId, adv, dbTran);
                return;
            }

            if (CanUpdateFullString(positionIm))
                UpdateFullString(positionIm.OrderPositionId, adv.String, dbTran);

            if (CanUpdateModule(positionIm))
                UpdateFullModulePositionIm(positionIm, adv.Module, dbTran);
        }

        #endregion

        #region Delete

        private bool CanDeletePositionIm(PositionIm positionIm)
        {
            return positionIm != null;
        }

        private void DeletePositionIm(PositionIm positionIm, DbTransaction dbTran)
        {
            var taskFileDate = positionIm.TaskFileDate;
            var maketFileDate = positionIm.MaketFileDate;
            var lastEditDate = positionIm.BeginDate;

            _repository.SetPositionIm(
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
                newTaskFile: false,
                newMaketFile: false,
                taskFileDate: ref taskFileDate,
                maketFileDate: ref maketFileDate,
                dontVerify: positionIm.DontVerify,
                rdvRating: positionIm.RdvRating,
                isActual: false,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            _context.Entry(positionIm).Reload();
        }

        private void DeleteFullPositionIm(PositionIm positionIm, DbTransaction dbTran)
        {
            if (!CanDeletePositionIm(positionIm))
                return;

            DeletePositionIm(positionIm, dbTran);
            
            if (CanDeleteFullString(positionIm))
                DeleteFullString(positionIm.OrderPositionId, dbTran);

            //DeleteModule(positionIm.OrderPositionId, DateTime.Now);
            DeleteModuleGraphics(positionIm.OrderPositionId, DateTime.Now);

            var orderIm = GetOrderIm(positionIm.OrderId, positionIm.PositionImType.OrderImTypeId);

            if (NeedDeleteOrderIm(orderIm))
                DeleteOrderIm(orderIm, dbTran);
            else
            {
                //if (NeedUpdateOrderIm(orderIm))
                    SetOrderIm(orderIm, true, dbTran);
            }
        }

        #endregion
    }
}