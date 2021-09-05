using CkpDAL.Entities.String;
using CkpModel.Input.String;
using CkpModel.Output.String;
using CkpServices.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<List<AddressInfo>> GetAddressesAsync(int clientLegalPersonId, string description)
        {
            var addresses = await _context.Addresses
                .Include(a => a.Company).ThenInclude(co => co.LegalPersons)
                .Where(
                    a =>
                        a.Company.LegalPersons
                            .Select(lp => lp.Id)
                            .Contains(clientLegalPersonId) &&
                        (string.IsNullOrEmpty(description) ||
                            a.Description.ToUpper()
                            .Contains(description.ToUpper())))
                .OrderByDescending(a => a.Id)
                .Take(10)
                .Select(
                    a => 
                        new AddressInfo
                        {
                            Id = a.Id,
                            Description = a.Description
                        })
                .ToListAsync();

            return addresses;
        }

        #endregion

        #region Create

        private void CreateStringAddresses(int stringId, int companyId, IEnumerable<AddressData> addressesData, DbTransaction dbTran)
        {
            foreach (var addressData in addressesData)
                CreateStringAddress(stringId, companyId, addressData.Value, addressData.OrderBy, dbTran);
        }

        private bool NeedCreateStringAddress(IEnumerable<StringAddress> stringAddresses, AddressData addressData)
        {
            return !stringAddresses
                .GetActualItems()
                .Any(
                    sa =>
                        sa.Description == addressData.Value &&
                        sa.OrderBy == addressData.OrderBy);
        }

        private StringAddress CreateStringAddress(int stringId, int companyId, string value, int orderBy, DbTransaction dbTran)
        {
            var address = GetCompanyAddress(companyId, value);

            if (address == null)
                address = CreateCompanyAddress(companyId, value, dbTran);
            
            address = _repository.SetAddress(address, isActual: true, dbTran);
            
            // Привязываем адрес к строке
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

        private void UpdateStringAddresses(int stringId, int companyId, IEnumerable<StringAddress> stringAddresses, IEnumerable<AddressData> addressesData,
            DbTransaction dbTran)
        {
            var stringAddressesList = stringAddresses.GetActualItems().ToList();

            for (int i = stringAddressesList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringAddress(stringAddressesList[i], addressesData))
                    DeleteStringAddress(stringAddressesList[i], dbTran);

            foreach (var addressData in addressesData)
                if (NeedCreateStringAddress(stringAddressesList, addressData))
                    CreateStringAddress(stringId, companyId, addressData.Value, addressData.OrderBy, dbTran);
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
