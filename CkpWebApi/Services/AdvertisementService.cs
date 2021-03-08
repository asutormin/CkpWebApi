using CkpWebApi.OutputEntities;
using CkpWebApi.Helpers;
using DebtsWebApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CkpWebApi.InputEntities;
using Microsoft.EntityFrameworkCore.Storage;
using CkpWebApi.DAL.Repository;
using CkpWebApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using CkpWebApi.DAL.Model;
using CkpWebApi.InputEntities.String;
using CkpWebApi.InputEntities.String.Requirements;
using CkpWebApi.InputEntities.String.Conditions;
using CkpWebApi.InputEntities.Module;
using System.Data.Common;
using CkpWebApi.Helpers.Providers;
using System.IO;
using System;
using System.Text;

namespace CkpWebApi.Services
{
    public partial class AdvertisementService : IAdvertisementService
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly string _positionImSampleTemplate;
        private readonly string _positionImFilePathTemplate;
        private readonly string _positionImGraphicsFolderPathTemplate;
        private readonly string _positionImTmpFileTemplate;
        private readonly string _verstkaFolderPathTemplate;

        private readonly string _orderDescription; // = "Корзина_ЛК";
        private readonly int _orderBusinessUnitId;
        private readonly int _editUserId;
        private readonly int _managerId;

        private bool _needRefreshShoppingCartOrder = false;

        public AdvertisementService(BPFinanceContext context, IOptions<AppSettings> appSettingsAccessor, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            _repository = new BPFinanceRepository(_context);

            var orderImFolderTemplate = appSettingsAccessor.Value.OrderImFolderTemplate;
            var dbName = appSettingsAccessor.Value.DatabaseName;
            var orderImFolder = string.Format(orderImFolderTemplate, dbName);
            
            _positionImSampleTemplate = orderImFolder + "{0}\\{1}\\{2}_{3}\\{4}.jpg";
            _positionImFilePathTemplate = orderImFolder + "{0}\\{1}\\{1}.tif";
            _positionImGraphicsFolderPathTemplate = orderImFolder + "{0}\\{1}\\Graphics";
            _positionImTmpFileTemplate = orderImFolder + "{0}\\~{1}.tif";
            _verstkaFolderPathTemplate = string.Empty;
            //_verstkaFolderPathTemplate = "\\\\file-server.local\\Beta\\!SDACHA\\{0}\\" + dbName + "\\Rdv\\z{1} - {2}\\";

            _orderBusinessUnitId = appParamsAccessor.Value.OrderBusinessUnitId;
            _editUserId = appParamsAccessor.Value.EditUserId;
            _managerId = appParamsAccessor.Value.ManagerId;
            _orderDescription = appParamsAccessor.Value.ShoppingCartOrderDescription;
        }        

        public async Task<ActionResult<IEnumerable<PositionInfo>>> GetShoppingCartAsync(int clientLegalPersonId)
        {
            var result = await _context.OrderPositions
                .Include(op => op.Price)
                .Include(op => op.Order.AccountOrder)
                .Include(op => op.ParentOrderPosition)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions).ThenInclude(rp => rp.Rubric)
                .Where(
                    op =>
                        op.ParentOrderPositionId == null &&
                        op.Order.ClientLegalPersonId == clientLegalPersonId &&
                        op.Order.ActivityTypeId == 20 &&
                        op.Order.Description == _orderDescription &&
                        op.Order.BusinessUnitId == _orderBusinessUnitId)
                .SelectPositions()
                .ToListAsync();

            return result;
        }

        #region

        public Advertisement GetAdvertisementFull(int orderPositionId)
        {
            var advertisement = GetAdvertisement(orderPositionId);

            var childOrderPositionIds = _context.OrderPositions
                .Include(op => op.ChildOrderPositions)
                .Single(op => op.Id == orderPositionId)
                .ChildOrderPositions
                .Select(cop => cop.Id);

            if (childOrderPositionIds.Any())
                advertisement.Childs = new List<Advertisement>();

            foreach (var childOrderPositionId in childOrderPositionIds)
            {
                var childAdvertesement = GetAdvertisement(childOrderPositionId);
                advertisement.Childs.Add(childAdvertesement);
            }

            return advertisement;
        }

        public Advertisement GetAdvertisement(int orderPositionId)
        {
            var advertisement = _context.OrderPositions
                .Include(op => op.Order.ClientCompany)
                .Include(op => op.PricePosition)
                .Include(op => op.RubricPositions)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Where(op => op.Id == orderPositionId)
                .Select(
                    op =>
                        new Advertisement
                        {
                            OrderId = op.OrderId,
                            OrderPositionId = op.Id,
                            ClientId = op.Order.ClientCompanyId,
                            ClientLegalPersonId = op.Order.ClientLegalPersonId,
                            SupplierId = op.SupplierId,
                            Format = new AdvertisementFormat
                            {
                                Id = op.PricePositionId,
                                Version = op.PricePositionVersion,
                                FormatTypeId = op.PricePosition.PricePositionTypeId
                            },
                            PriceId = op.PriceId,
                            Rubric = new AdvertisementRubric
                            {
                                Id = op.RubricPositions.First().RubricId,
                                Version = op.RubricPositions.First().RubricVersion
                            },
                            Graphics = GetAdvGraphics(op)
                        })
                .Single();

            var positionIm = _context.OrderPositions
                .Include(op => op.PositionIm)
                .Single(op => op.Id == orderPositionId)
                .PositionIm;

            if (positionIm != null)
            {
                var positionImTypeId = positionIm.PositionImTypeId;

                if (positionImTypeId == 1)
                    advertisement.String = GetAdvString(orderPositionId);

                if (positionImTypeId == 2)
                    advertisement.Module = GetAdvModule(orderPositionId);
            }

            return advertisement;
        }

        private static List<AdvertisementGraphic> GetAdvGraphics(OrderPosition orderPosition)
        {
            var advGraphics = orderPosition.GraphicPositions
                .Where(gp => gp.Id == gp.ParenGraphicPositiontId)
                .Select(
                    gp =>
                        new AdvertisementGraphic
                        {
                            Id = gp.GraphicId,
                            Childs = gp.GetChilds().Select(cgp => cgp.GraphicId).ToList()
                        })
                .ToList();

            return advGraphics;
        }

        public AdvString GetAdvString(int orderPositionId)
        {
            var advString = _context.StringPositions
                .Include(s => s.Phones)
                .Include(s => s.Webs)
                .Include(s => s.Addresses)
                .Include(s => s.Occurrences)
                .Where(s => s.OrderPositionId == orderPositionId)
                .Select(
                    s =>
                        new AdvString
                        {
                            Id = s.Id,
                            Date = s.Date,
                            VacancyName = s.VacancyName,
                            VacancyAdditional = s.VacancyAdditional,
                            Requirements = new AdvRequirements
                            {
                                Value = s.Requirement,
                                Age = new AdvAge
                                {
                                    From = s.AgeFrom,
                                    To = s.AgeTo
                                },
                                CitizenshipId = s.CitizenshipId,
                                EducationId = s.EducationId,
                                Experience = new AdvExperience
                                {
                                    Id = s.ExperienceId,
                                    Value = s.ExperienceValue
                                },
                                GenderId = s.GenderId
                            },
                            Responsibility = s.Responsibility,
                            Conditions = new AdvConditions
                            {
                                Value = s.Condition,
                                Salary = new AdvSalary
                                {
                                    From = s.SalaryFrom,
                                    To = s.SalaryTo,
                                    IsSalaryPercent = s.IsSalaryPercent,
                                    Description = s.SalaryDescription,
                                    CurrencyId = s.CurrencyId,
                                },
                                WorkGraphic = new AdvWorkGraphic
                                {
                                    Id = s.WorkGraphicId,
                                    Comment = s.WorkGraphic
                                },
                                IsFood = s.IsFood,
                                IsHousing = s.IsHousing
                            },
                            Contact = new AdvContact
                            {
                                FirstName = s.ContactFirstName,
                                SecondName = s.ContactSecondName,
                                LastName = s.ContactLastName
                            },
                            Logo = new AdvLogo
                            {
                                Base64String = s.Logo == null ? null : Convert.ToBase64String(s.Logo), //Encoding.ASCII.GetString(s.Logo),
                                FileName = s.LogoFileName
                            },
                            Phones = s.Phones
                                .Where(p => p.IsActual)
                                .Select(
                                    p =>
                                        new AdvPhone
                                        {
                                            CountryCode = p.CountryCode,
                                            Code = p.Code,
                                            Number = p.Number,
                                            Description = p.Description,
                                            OrderBy = p.OrderBy
                                        }),
                            Emails = s.Webs
                                .Where(w => w.IsActual)
                                .Select(
                                    w =>
                                        new AdvEmail
                                        {
                                            Value = w.WebValue,
                                            Description = w.Description,
                                            OrderBy = w.OrderBy
                                        }),
                            Addresses = s.Addresses
                                .Where(a => a.IsActual)
                                .Select(
                                    a =>
                                        new AdvAddress
                                        {
                                            Value = a.Description,
                                            OrderBy = a.OrderBy
                                        }),
                            Occurrences = s.Occurrences
                                .Where(o => o.EndDate > DateTime.Now)
                                .Select(
                                    o =>
                                        new AdvOccurrence
                                        {
                                            Id = o.OccurrenceId,
                                            TypeId = o.TypeId,
                                            OrderBy = o.OrderBy
                                        })

                        })
                .Single();

            return advString;
        }

        public AdvModule GetAdvModule(int orderPositionId)
        {
            var module = new AdvModule();

            //var filePath = new OrderImFilePathProvider(_positionImFilePathTemplate)
            //    .GetByValue(orderPositionId);

            var folderPath = new OrderImGraphicsFolderPathProvider(_positionImGraphicsFolderPathTemplate)
                .GetByValue(orderPositionId);

            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] files = directoryInfo.GetFiles("*.tif");

                if (files.Length > 0)
                {
                    var file = files.First();
                    var filePath = file.FullName;

                    var bytes = File.ReadAllBytes(filePath);

                    module.Base64String = Convert.ToBase64String(bytes);
                    module.Name = file.Name;
                }
            }

            //byte[] bytes;

            //if (File.Exists(filePath))
            //{
            //    bytes = File.ReadAllBytes(filePath);
            //    module.Base64String = Convert.ToBase64String(bytes);
            //}

            return module;
        }

        #endregion

        #region Create
        public void CreateAdvertisement(Advertisement adv)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                // Создаём заказ если это необходимо
                var order = GetShoppingCartOrder(adv.ClientLegalPersonId);

                int orderId =
                    order == null
                    ? CreateShoppingCartOrder(adv, dbTran)
                    : order.Id;

                // Получааем скидку клиента
                var clientDiscount = GetClientDiscount(adv.ClientLegalPersonId, adv.Format.FormatTypeId);

                // Сохраняем позицию заказа
                int orderPositionId = CreateFullOrderPosition(orderId, null, clientDiscount, adv, dbTran);

                // Сохраняем пакетные позиции заказа
                foreach (var child in adv.Childs)
                    CreateFullOrderPosition(orderId, orderPositionId, 0, child, dbTran);

                if (order != null)
                    RefreshOrder(orderId, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        private void ChangeOrderPositionsOrder(IEnumerable<OrderPosition> orderPositions, int orderId, DbTransaction dbTran)
        {
            foreach (var orderPosition in orderPositions)
            {
                orderPosition.OrderId = orderId;

                // Меняем статус позиции ИМ-а на "Вёрстка"
                if (orderPosition.PositionIm != null)
                    SetPositionImStatusVerstka(orderPosition.PositionIm, dbTran);

                UpdateOrderPosition(orderPosition, dbTran);

                if (orderPosition.ChildOrderPositions != null)
                    ChangeOrderPositionsOrder(orderPosition.ChildOrderPositions, orderId, dbTran);
            }
        }

        public int CreateClientAccount(int[] orderPositionIds)
        {
            var accountId = 0;

            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();
                
                // Создаём заказ
                var orderPositions = GetOrderPositionsByIds(orderPositionIds);
                var shoppingCartOrder = GetOrderById(orderPositions.First().OrderId);
                var orderId = CreateOrder(shoppingCartOrder, orderPositions, dbTran);

                // Привязываем к заказу позиции
                ChangeOrderPositionsOrder(orderPositions, orderId, dbTran);

                // Обновляем ИМ-ы заказа-корзины
                var orderImTypeIds = orderPositions
                    .Where(op => op.PositionIm != null)
                    .GroupBy(op => op.PositionIm.PositionImType.OrderImType.Id)
                    .Select(g => g.Key)
                    .ToList();

                foreach(var orderImTypeId in orderImTypeIds)
                {
                    // Создаём ИМ нового заказа
                    CreateOrderIm(orderId, orderImTypeId, dbTran);

                    // Для строк меняем статус ИМ-а на "Готов"
                    if (orderImTypeId == 1)
                        SetOrderImStatusReady(orderId, orderImTypeId, dbTran);
                    else
                        // Для всех остальных (модули и пр.) меняем статус ИМ-а заказа на "Вёрстка"
                        SetOrderImStatusVerstka(orderId, orderImTypeId, dbTran);

                    // Если ИМ заказа корзины больше не нужен - удаляем его
                    var shoppingCartOrderIm = GetOrderIm(shoppingCartOrder.Id, orderImTypeId);
                    if (NeedDeleteOrderIm(shoppingCartOrderIm))
                        DeleteOrderIm(shoppingCartOrderIm, dbTran);
                }

                // Обновляем заказ-корзину
                RefreshOrder(shoppingCartOrder, dbTran);

                // Получаем номер счёта
                var accountNumber = GetAccountNumber(shoppingCartOrder.SupplierLegalPersonId, dbTran);

                // Создаём счёт
                var accountSum = orderPositions.GetClientSum();
                var businessUnit = shoppingCartOrder.BusinessUnit;
                var clientLegalPerson = shoppingCartOrder.ClientLegalPerson;
                
                accountId = CreateAccount(accountNumber, accountSum, businessUnit, clientLegalPerson, dbTran);

                // Создаём позиции счёта
                foreach (var orderPosition in orderPositions)
                    CreateAccountPosition(accountId, orderPosition, dbTran);

                // Создаём настройки счёта
                var legalPersonAccountSettings = clientLegalPerson.AccountSettings;
                CreateAccountSettings(accountId, legalPersonAccountSettings, dbTran);

                // Создаём связку счёт-заказ
                CreateAccountOrder(accountId, orderId, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }

            return accountId;
        }

        #endregion

        #region Update
        public void UpdateAdvertisement(Advertisement adv)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                var orderPosition = GetOrderPositionsByIds(new[] { adv.OrderPositionId }).Single();

                var shoppingCartOrder = GetOrderById(orderPosition.OrderId);

                // Обновляем позицию заказа
                UpdateFullOrderPosition(orderPosition, adv, dbTran);

                // Перебираем переданные пакетные позиции
                if (adv.Childs != null)
                {
                    foreach (var child in adv.Childs)
                    {
                        var childOrderPosition = _context.OrderPositions
                            .SingleOrDefault(op => op.Id == child.OrderPositionId);

                        // Если переданной позиции не существует - создаём её, иначе - обновляем
                        if (NeedCreateFullOrderPosition(childOrderPosition))
                            CreateFullOrderPosition(shoppingCartOrder.Id, orderPosition.Id, 0, child, dbTran);
                        else
                            UpdateFullOrderPosition(childOrderPosition, child, dbTran);
                    }
                }

                var childOrderPositions = orderPosition.ChildOrderPositions;

                // Если в существующих пакетных позициях нет переданных - удаляем их
                foreach (var childOrderPosition in childOrderPositions)
                    if (NeedDeleteFullOrderPosition(childOrderPosition.Id, adv.Childs))
                        DeleteFullOrderPosition(childOrderPosition, dbTran);

                if (_needRefreshShoppingCartOrder)
                    RefreshOrder(shoppingCartOrder, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        #endregion

        #region Delete
        public void DeleteAdvertisement(int orderPositionId)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                var orderPosition = _context.OrderPositions
                    .Include(op => op.Price)
                    .Include(op => op.RubricPositions)
                    .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                    .Include(op => op.PositionIm).ThenInclude(pi => pi.PositionImType).ThenInclude(pit => pit.OrderImType)
                    .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Price)
                    .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.RubricPositions)
                    .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.GraphicPositions).ThenInclude(cgp => cgp.Graphic)
                    .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PositionIm).ThenInclude(cpi => cpi.PositionImType).ThenInclude(pit => pit.OrderImType)
                    .SingleOrDefault(op => op.Id == orderPositionId);

                if (orderPosition == null)
                    return;

                var childOrderPositions = orderPosition.ChildOrderPositions
                    .ToList();

                for (int i = childOrderPositions.Count - 1; i >= 0; i--)
                    DeleteFullOrderPosition(childOrderPositions[i], dbTran);

                DeleteFullOrderPosition(orderPosition, dbTran);

                RefreshOrder(orderPosition.OrderId, dbTran);

                //_context.SaveChanges();
                dbTran.Commit();
            }
        }

        #endregion
    }
}
