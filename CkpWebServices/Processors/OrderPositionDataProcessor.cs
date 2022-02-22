using CkpDAL;
using CkpDAL.Entities;
using CkpModel.Input;
using CkpModel.Input.Module;
using CkpModel.Input.String;
using CkpModel.Input.String.Conditions;
using CkpModel.Input.String.Requirements;
using CkpServices.Helpers;
using CkpServices.Helpers.Providers;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CkpServices.Processors
{
    class OrderPositionDataProcessor : IOrderPositionDataProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly string _positionImGraphicsFolderPathTemplate;

        public OrderPositionDataProcessor(BPFinanceContext context, string orderImFolderTemplate, string dbName)
        {
            _context = context;

            var orderImFolder = string.Format(orderImFolderTemplate, dbName);
            _positionImGraphicsFolderPathTemplate = orderImFolder + "{0}\\{1}\\Graphics";
        }

        public async Task<OrderPositionData> GetOrderPositionFullDataAsync(int orderPositionId)
        {
            var opd = await GetOrderPositionDataAsync(orderPositionId);

            var orderPosition = await _context.OrderPositions
                .Include(op => op.ChildOrderPositions)
                .SingleAsync(op => op.Id == orderPositionId);

            var childOrderPositionIds = orderPosition
                .ChildOrderPositions
                .Select(cop => cop.Id);

            if (childOrderPositionIds.Any())
            {
                opd.Childs = new List<OrderPositionData>();

                foreach (var childOrderPositionId in childOrderPositionIds)
                {
                    var childOpd = await GetOrderPositionDataAsync(childOrderPositionId);
                    opd.Childs.Add(childOpd);
                }

                //var childOpdTasks = childOrderPositionIds.Select(id => GetOrderPositionDataAsync(id));
                //var childOpds = await Task.WhenAll(childOpdTasks);

                //opd.Childs.AddRange(childOpds);
            }

            return opd;
        }

        private async Task<OrderPositionData> GetOrderPositionDataAsync(int orderPositionId)
        {
            var orderPosition = await _context.OrderPositions
                .Include(op => op.Order.ClientCompany)
                .Include(op => op.PricePosition)
                .Include(op => op.RubricPositions)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.PositionIm)
                .SingleAsync(op => op.Id == orderPositionId);

            var opd = new OrderPositionData
            {
                OrderId = orderPosition.OrderId,
                OrderPositionId = orderPosition.Id,
                ClientId = orderPosition.Order.ClientCompanyId,
                ClientLegalPersonId = orderPosition.Order.ClientLegalPersonId,
                SupplierId = orderPosition.SupplierId,
                FormatData = new FormatData
                {
                    Id = orderPosition.PricePositionId,
                    Version = orderPosition.PricePositionVersion,
                    FormatTypeId = orderPosition.PricePosition.PricePositionTypeId,
                    FirstSize = orderPosition.PricePosition.FirstSize,
                    SecondSize = orderPosition.PricePosition.SecondSize,
                    PackageLength = orderPosition.PricePosition.PackageLength,
                    Description = orderPosition.PricePosition.Description
                },
                PriceId = orderPosition.PriceId,
                RubricData = GetRubricData(orderPosition),
                GraphicsData = GetGraphicsData(orderPosition)
            };

            if (orderPosition.PositionIm != null)
            {
                var positionImTypeId = orderPosition.PositionIm.PositionImTypeId;

                if (positionImTypeId == 1)
                    opd.StringData = await GetStringDataAsync(orderPositionId);

                if (positionImTypeId == 2)
                    opd.ModuleData = await GetModuleDataAsync(orderPositionId);
            }

            return opd;
        }

        private static RubricData GetRubricData(OrderPosition orderPosition)
        {
            var rubricPosition = orderPosition.RubricPositions.FirstOrDefault();

            var rubricData = rubricPosition == null
                ? null
                : new RubricData { Id = rubricPosition.RubricId, Version = rubricPosition.RubricVersion };

            return rubricData;
        }

        private static List<GraphicData> GetGraphicsData(OrderPosition orderPosition)
        {
            var graphicsData = orderPosition.GraphicPositions
                .Where(gp => gp.Id == gp.ParenGraphicPositiontId)
                .Select(
                    gp =>
                        new GraphicData
                        {
                            Id = gp.GraphicId,
                            Childs = gp.GetChilds().Select(cgp => cgp.GraphicId).ToList()
                        })
                .ToList();

            return graphicsData;
        }

        private async Task<StringData> GetStringDataAsync(int orderPositionId)
        {
            var stringData = await _context.StringPositions
                .Include(s => s.Phones)
                .Include(s => s.Webs)
                .Include(s => s.Addresses)
                .Include(s => s.Occurrences)
                .Where(s => s.OrderPositionId == orderPositionId)
                .Select(
                    s =>
                        new StringData
                        {
                            Id = s.Id,
                            Date = s.Date,
                            AnonymousCompanyName = s.AnonymousCompanyName,
                            VacancyName = s.VacancyName,
                            VacancyAdditional = s.VacancyAdditional,
                            RequirementsData = new RequirementsData
                            {
                                Value = s.Requirement,
                                AgeData = new AgeData
                                {
                                    From = s.AgeFrom,
                                    To = s.AgeTo
                                },
                                CitizenshipId = s.CitizenshipId,
                                EducationId = s.EducationId,
                                ExperienceData = new ExperienceData
                                {
                                    Id = s.ExperienceId,
                                    Value = s.ExperienceValue
                                },
                                GenderId = s.GenderId
                            },
                            Responsibility = s.Responsibility,
                            ConditionsData = new ConditionsData
                            {
                                Value = s.Condition,
                                SalaryData = new SalaryData
                                {
                                    From = s.SalaryFrom,
                                    To = s.SalaryTo,
                                    IsSalaryPercent = s.IsSalaryPercent,
                                    Description = s.SalaryDescription,
                                    CurrencyId = s.CurrencyId,
                                },
                                WorkGraphicData = new WorkGraphicData
                                {
                                    Id = s.WorkGraphicId,
                                    Comment = s.WorkGraphic
                                },
                                IsFood = s.IsFood,
                                IsHousing = s.IsHousing
                            },
                            ContactData = new ContactData
                            {
                                FirstName = s.ContactFirstName,
                                SecondName = s.ContactSecondName,
                                LastName = s.ContactLastName
                            },
                            LogoData = new LogoData
                            {
                                Base64String = s.Logo == null ? null : Convert.ToBase64String(s.Logo),
                                FileName = s.LogoFileName
                            },
                            PhonesData = s.Phones
                                .Where(p => p.IsActual)
                                .Select(
                                    p =>
                                        new PhoneData
                                        {
                                            CountryCode = p.CountryCode,
                                            Code = p.Code,
                                            Number = p.Number,
                                            AdditionalNumber = p.AdditionalNumber,
                                            Description = p.Description,
                                            OrderBy = p.OrderBy
                                        }),
                            EmailsData = s.Webs
                                .Where(w => w.IsActual)
                                .Select(
                                    w =>
                                        new EmailData
                                        {
                                            Value = w.WebValue,
                                            Description = w.Description,
                                            OrderBy = w.OrderBy
                                        }),
                            AddressesData = s.Addresses
                                .Where(a => a.IsActual)
                                .Select(
                                    a =>
                                        new AddressData
                                        {
                                            Value = a.Description,
                                            OrderBy = a.OrderBy
                                        }),
                            OccurrencesData = s.Occurrences
                                .Select(
                                    o =>
                                        new OccurrenceData
                                        {
                                            Id = o.OccurrenceId,
                                            TypeId = o.TypeId,
                                            OrderBy = o.OrderBy
                                        })
                        })
                .AsSingleQuery()
                .SingleAsync();

            return stringData;
        }

        private async Task<ModuleData> GetModuleDataAsync(int orderPositionId)
        {
            var module = new ModuleData();

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

                    var bytes = await File.ReadAllBytesAsync(filePath);

                    module.Base64String = Convert.ToBase64String(bytes);
                    module.Name = file.Name;
                }
            }

            return module;
        }
    }
}
