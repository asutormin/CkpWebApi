using CkpDAL.Entities.String;
using CkpModel.Input.String;
using CkpServices.Helpers;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.String
{
    partial class StringProcessor
    {
        #region Get
        private Address GetCompanyAddress(int companyId, string value)
        {
            var companyAddress = _context.Addresses
                .FirstOrDefault(
                    adr =>
                        adr.CompanyId == companyId &&
                        adr.Description == value &&
                        adr.IsActual == true);

            return companyAddress;
        }

        #endregion

        #region Create

        private void CreateStringAddresses(int stringId, int companyId, IEnumerable<AddressData> advAddresses, DbTransaction dbTran)
        {
            foreach (var advAddress in advAddresses)
                CreateStringAddress(stringId, companyId, advAddress.Value, advAddress.OrderBy, dbTran);
        }

        private bool NeedCreateStringAddress(IEnumerable<StringAddress> stringAddresses, AddressData advAddress)
        {
            return !stringAddresses
                .GetActualItems()
                .Any(
                    sa =>
                        sa.Description == advAddress.Value &&
                        sa.OrderBy == advAddress.OrderBy);
        }

        private StringAddress CreateStringAddress(int stringId, int companyId, string value, int orderBy, DbTransaction dbTran)
        {
            var address = GetCompanyAddress(companyId, value);

            if (address == null)
                address = CreateCompanyAddress(companyId, value, dbTran);
            
            address = _repository.SetAddress(address, isActual: true, dbTran);
            
            // Привязываем телефон к строке
            var stringAddress = _stringAddressFactory.Create(stringId, address, orderBy);
            stringAddress = _repository.SetStringAddress(stringAddress, isActual: true, dbTran);

            return stringAddress;
        }

        private Address CreateCompanyAddress(int companyId, string value, DbTransaction dbTran)
        {
            var address = _addressFactory.Create(companyId, value);
            address = _repository.SetAddress(address, isActual: true, dbTran);

            return address;
        }

        #endregion

        #region Update

        private void UpdateStringAddresses(int stringId, int companyId, IEnumerable<StringAddress> stringAddresses, IEnumerable<AddressData> advAddresses,
            DbTransaction dbTran)
        {
            var stringAddressesList = stringAddresses.GetActualItems().ToList();

            for (int i = stringAddressesList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringAddress(stringAddressesList[i], advAddresses))
                    DeleteStringAddress(stringAddressesList[i], dbTran);

            foreach (var advAddress in advAddresses)
                if (NeedCreateStringAddress(stringAddressesList, advAddress))
                    CreateStringAddress(stringId, companyId, advAddress.Value, advAddress.OrderBy, dbTran);
        }

        #endregion

        #region Delete

        private void DeleteStringAddresses(IEnumerable<StringAddress> stringAddresses, DbTransaction dbTran)
        {
            var stringAddressesList = stringAddresses.GetActualItems().ToList();

            for (int i = stringAddressesList.Count - 1; i >= 0; i--)
                DeleteStringAddress(stringAddressesList[i], dbTran);
        }

        private bool NeedDeleteStringAddress(StringAddress stringAddress, IEnumerable<AddressData> advAddresses)
        {
            return !advAddresses.Any(
                sa =>
                    sa.Value == stringAddress.Description &&
                    sa.OrderBy == stringAddress.OrderBy);
        }

        private void DeleteStringAddress(StringAddress stringAddress, DbTransaction dbTran)
        {
            _repository.SetStringAddress(stringAddress, isActual: false, dbTran);
            _context.Entry(stringAddress).Reload();
        }

        #endregion
    }
}
