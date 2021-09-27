using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Entities.String;
using CkpDAL.Repository;
using CkpModel.Input.String;
using CkpServices.Helpers.Factories.Interfaces.String;
using CkpServices.Helpers.Factories.String;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.String
{
    partial class StringProcessor : IStringProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IStringPositionFactory _stringFactory;
        private readonly IAddressFactory _addressFactory;
        private readonly IStringAddressFactory _stringAddressFactory;
        private readonly IStringOccurrenceFactory _stringOccurrenceFactory;
        private readonly IPhoneFactory _phoneFactory;
        private readonly IStringPhoneFactory _stringPhoneFactory;
        private readonly IEmailFactory _emailFactory;
        private readonly IStringWebFactory _stringWebFactory;

        public StringProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;

            _stringFactory = new StringPositionFactory();
            _addressFactory = new AddressFactory();
            _stringAddressFactory = new StringAddressFactory();
            _stringOccurrenceFactory = new StringOccurrenceFactory();
            _phoneFactory = new PhoneFactory();
            _stringPhoneFactory = new StringPhoneFactory();
            _emailFactory = new EmailFactory();
            _stringWebFactory = new StringWebFactory();
        }

        #region Get

        public StringPosition GetStringPosition(int orderPositionId)
        {
            var stringPosition = _context.StringPositions
                .Include(sp => sp.OrderPosition)
                .Include(sp => sp.Addresses)
                .Include(sp => sp.Occurrences)
                .Include(sp => sp.Phones)
                .Include(sp => sp.Webs)
                .SingleOrDefault(sp => sp.OrderPositionId == orderPositionId);

            return stringPosition;
        }        

        #endregion

        #region Create

        public void CreateFullString(int businessUnitId, int companyId, int orderPositionId, StringData stringData, DbTransaction dbTran)
        {
            var stringPosition = CreateStringPosition(businessUnitId, companyId, orderPositionId, stringData, dbTran);

            CreateStringAddresses(stringPosition.Id, companyId, stringData.AddressesData, dbTran);
            CreateStringOccurrences(stringPosition.Id, stringData.OccurrencesData, dbTran);
            CreateStringPhones(stringPosition.Id, companyId, stringData.PhonesData, dbTran);
            CreateStringEmails(stringPosition.Id, companyId, stringData.EmailsData, dbTran);
        }

        #endregion

        #region Update

        public bool CanUpdateString(PositionIm positionIm)
        {
            return positionIm != null && positionIm.PositionImType.OrderImTypeId == 1;
        }

        public void UpdateFullString(int orderPositionId, StringData stringData, DbTransaction dbTran)
        {
            var stringPosition = GetString(orderPositionId);

            if (stringPosition == null)
                return;

            UpdateString(stringPosition, stringData, dbTran);

            UpdateStringAddresses(stringPosition.Id, stringPosition.CompanyId, stringPosition.Addresses, stringData.AddressesData, dbTran);
            UpdateStringOccurences(stringPosition.Id, stringPosition.Occurrences, stringData.OccurrencesData, dbTran);
            UpdateStringPhones(stringPosition.Id, stringPosition.CompanyId, stringPosition.Phones, stringData.PhonesData, dbTran);
            UpdateStringEmails(stringPosition.Id, stringPosition.CompanyId, stringPosition.Webs, stringData.EmailsData, dbTran);
        }

        #endregion

        #region Delete

        public void DeleteFullString(int orderPositionId, DbTransaction dbTran)
        {
            var stringPosition = GetString(orderPositionId);

            if (stringPosition == null)
                return;

            DeleteStringAddresses(stringPosition.Addresses, dbTran);
            DeleteStringOccurrences(stringPosition.Occurrences, dbTran);
            DeleteStringPhones(stringPosition.Phones, dbTran);
            DeleteStringEmails(stringPosition.Webs, dbTran);

            DeleteString(stringPosition, dbTran);
        }

        #endregion
    }
}
