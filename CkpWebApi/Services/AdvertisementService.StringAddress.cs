using CkpWebApi.DAL.Model.String;
using CkpWebApi.Helpers;
using CkpWebApi.InputEntities.String;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpWebApi.Services
{
    public partial class AdvertisementService
    {
        private Address GetCompanyAddress(int companyId, AdvAddress advAddress)
        {
            var companyAddress = _context.Addresses
                .FirstOrDefault(
                    adr =>
                        adr.CompanyId == companyId &&
                        adr.Description == advAddress.Value &&
                        adr.IsActual == true);

            return companyAddress;
        }
        private int CreateCompanyAddress(int companyId, AdvAddress advAddress, DbTransaction dbTran)
        {
            var addressId = 0;

            // Создаём адрес компании
            _repository.SetStringCompanyAddress(
                dbTran: dbTran,
                id: ref addressId,
                companyId: companyId,
                cityId: null,
                metroId: null,
                street: null,
                house: null,
                corps: null,
                building: null,
                description: advAddress.Value,
                isActual: true,
                editUserId: _editUserId);

            SetNeedRefreshOrderShoppingCartOrder();

            return addressId;
        }


        #region Create

        private bool NeedCreateStringAddress(IEnumerable<StringAddress> stringAddresses, AdvAddress advAddress)
        {
            return !stringAddresses
                .GetActualItems()
                .Any(
                    sa =>
                        sa.Description == advAddress.Value &&
                        sa.OrderBy == advAddress.OrderBy);
        }

        private void CreateStringAddress(int stringId, int companyId, AdvAddress advAddress, DbTransaction dbTran)
        {
            var address = GetCompanyAddress(companyId, advAddress);

            var addressId =
                address == null
                ? CreateCompanyAddress(companyId, advAddress, dbTran)
                : address.Id;

            // Привязываем телефон к строке
            _repository.SetStringAddress(
                dbTran: dbTran,
                stringId: stringId,
                addressId: addressId,
                cityId: null,
                metroId: null,
                street: null,
                house: null,
                corps: null,
                building: null,
                description: advAddress.Value,
                orderBy: advAddress.OrderBy,
                isActual: true);

            SetNeedRefreshOrderShoppingCartOrder();
        }

        private void CreateStringAddresses(int stringId, int companyId, IEnumerable<AdvAddress> advAddresses, DbTransaction dbTran)
        {
            foreach (var advAddress in advAddresses)
                CreateStringAddress(stringId, companyId, advAddress, dbTran);
        }

        #endregion

        #region Update

        private void UpdateStringAddresses(int stringId, int companyId, IEnumerable<StringAddress> stringAddresses, IEnumerable<AdvAddress> advAddresses,
            DbTransaction dbTran)
        {
            var stringAddressesList = stringAddresses.GetActualItems().ToList();

            for (int i = stringAddressesList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringAddress(stringAddressesList[i], advAddresses))
                    DeleteStringAddress(stringAddressesList[i], dbTran);

            foreach (var advAddress in advAddresses)
                if (NeedCreateStringAddress(stringAddressesList, advAddress))
                    CreateStringAddress(stringId, companyId, advAddress, dbTran);
        }

        #endregion

        #region Delete

        private bool NeedDeleteStringAddress(StringAddress stringAddress, IEnumerable<AdvAddress> advAddresses)
        {
            return !advAddresses.Any(
                sa =>
                    sa.Value == stringAddress.Description &&
                    sa.OrderBy == stringAddress.OrderBy);
        }

        private void DeleteStringAddress(StringAddress stringAddress, DbTransaction dbTran)
        {
            _repository.SetStringAddress(
                dbTran: dbTran,
                stringId: stringAddress.StringId,
                addressId: stringAddress.AddressId,
                cityId: stringAddress.CityId,
                metroId: stringAddress.MetroId,
                street: stringAddress.Street,
                house: stringAddress.House,
                corps: stringAddress.Corps,
                building: stringAddress.Building,
                description: stringAddress.Description,
                orderBy: stringAddress.OrderBy,
                isActual: false);

            _context.Entry(stringAddress).Reload();
            SetNeedRefreshOrderShoppingCartOrder();
        }

        private void DeleteStringAddresses(IEnumerable<StringAddress> stringAddresses, DbTransaction dbTran)
        {
            var stringAddressesList = stringAddresses.GetActualItems().ToList();

            for (int i = stringAddressesList.Count - 1; i >= 0; i--)
                DeleteStringAddress(stringAddressesList[i], dbTran);
        }

        #endregion
    }
}
