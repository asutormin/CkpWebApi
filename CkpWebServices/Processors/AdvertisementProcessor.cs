using CkpDAL;
using CkpDAL.Model;
using CkpEntities.Input;
using CkpEntities.Input.Module;
using CkpEntities.Input.String;
using CkpEntities.Input.String.Conditions;
using CkpEntities.Input.String.Requirements;
using CkpServices.Helpers;
using CkpServices.Helpers.Providers;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CkpServices.Processors
{
    class AdvertisementProcessor : IAdvertisementProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly string _positionImGraphicsFolderPathTemplate;

        public AdvertisementProcessor(BPFinanceContext context, string orderImFolderTemplate, string dbName)
        {
            _context = context;

            var orderImFolder = string.Format(orderImFolderTemplate, dbName);
            _positionImGraphicsFolderPathTemplate = orderImFolder + "{0}\\{1}\\Graphics";
        }

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

        private Advertisement GetAdvertisement(int orderPositionId)
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
                                FormatTypeId = op.PricePosition.PricePositionTypeId,
                                FirstSize = op.PricePosition.FirstSize,
                                SecondSize = op.PricePosition.SecondSize
                            },
                            PriceId = op.PriceId,
                            Rubric = GetAdvRubric(op),
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

        private static AdvertisementRubric GetAdvRubric(OrderPosition orderPosition)
        {
            var rubricPosition = orderPosition.RubricPositions.FirstOrDefault();

            var advRubric = rubricPosition == null
                ? new AdvertisementRubric()
                : new AdvertisementRubric { Id = rubricPosition.RubricId, Version = rubricPosition.RubricVersion };

            return advRubric;
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

        private AdvString GetAdvString(int orderPositionId)
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
                                Base64String = s.Logo == null ? null : Convert.ToBase64String(s.Logo),
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

        private AdvModule GetAdvModule(int orderPositionId)
        {
            var module = new AdvModule();

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

            return module;
        }
    }
}
