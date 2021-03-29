using CkpDAL;
using CkpDAL.Model;
using CkpDAL.Repository;
using CkpEntities.Input.String;
using CkpServices.Helpers.Factories.Interfaces.String;
using CkpServices.Helpers.Factories.String;
using CkpServices.Processors.Interfaces;
using System.Data.Common;

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

        #region Create

        public void CreateFullString(int businessUnitId, int companyId, int orderPositionId, AdvString advString, DbTransaction dbTran)
        {
            var stringPosition = CreateStringPosition(businessUnitId, companyId, orderPositionId, advString, dbTran);

            CreateStringAddresses(stringPosition.Id, companyId, advString.Addresses, dbTran);
            CreateStringOccurrences(stringPosition.Id, advString.Occurrences, dbTran);
            CreateStringPhones(stringPosition.Id, companyId, advString.Phones, dbTran);
            CreateStringEmails(stringPosition.Id, companyId, advString.Emails, dbTran);
        }

        #endregion

        #region Update

        public bool CanUpdateString(PositionIm positionIm)
        {
            return positionIm != null && positionIm.PositionImType.OrderImTypeId == 1;
        }

        public void UpdateFullString(int orderPositionId, AdvString advString, DbTransaction dbTran)
        {
            var stringPosition = GetString(orderPositionId);

            if (stringPosition == null)
                return;

            UpdateString(stringPosition, advString, dbTran);

            UpdateStringAddresses(stringPosition.Id, stringPosition.CompanyId, stringPosition.Addresses, advString.Addresses, dbTran);
            UpdateStringOccurences(stringPosition.Id, stringPosition.Occurrences, advString.Occurrences, dbTran);
            UpdateStringPhones(stringPosition.Id, stringPosition.CompanyId, stringPosition.Phones, advString.Phones, dbTran);
            UpdateStringEmails(stringPosition.Id, stringPosition.CompanyId, stringPosition.Webs, advString.Emails, dbTran);
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
