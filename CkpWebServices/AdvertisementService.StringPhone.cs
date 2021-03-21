using CkpDAL.Model.String;
using CkpEntities.Input.String;
using CkpServices.Helpers;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices
{
    public partial class AdvertisementService
    {
        private Phone GetCompanyPhone(int companyId, AdvPhone advPhone)
        {
            var companyPhone = _context.Phones
                .FirstOrDefault(
                    cph =>
                        cph.CompanyId == companyId &&
                        cph.CountryCode == advPhone.CountryCode &&
                        cph.Code == advPhone.Code &&
                        cph.Number == advPhone.Number &&
                        cph.Description == advPhone.Description &&
                        cph.IsActual == true);

            return companyPhone;
        }
        private int CreateCompanyPhone(int companyId, AdvPhone advPhone, DbTransaction dbTran)
        {
            var phoneId = 0;

            // Создаём телефон компании
            _repository.SetStringCompanyPhone(
                dbTran: dbTran,
                id: ref phoneId,
                companyId: companyId,
                phoneTypeId: 1,
                countryCode: advPhone.CountryCode,
                code: advPhone.Code,
                number: advPhone.Number,
                additionalNumber: null,
                description: advPhone.Description,
                isActual: true,
                editUserId: _editUserId);

            return phoneId;
        }

        #region Create

        private bool NeedCreateStringPhone(AdvPhone advPhone, IEnumerable<StringPhone> stringPhones)
        {
            return !stringPhones
                .GetActualItems()
                .Any(
                sp =>
                    sp.CountryCode == advPhone.CountryCode &&
                    sp.Code == advPhone.Code &&
                    sp.Number == advPhone.Number &&
                    sp.Description == advPhone.Description &&
                    sp.OrderBy == advPhone.OrderBy);
        }

        private void CreateStringPhone(int stringId, int companyId, AdvPhone advPhone, DbTransaction dbTran)
        {
            var phone = GetCompanyPhone(companyId, advPhone);

            var phoneId =
                phone == null
                ? CreateCompanyPhone(companyId, advPhone, dbTran)
                : phone.Id;

            // Привязываем телефон к строке
            _repository.SetStringPhone(
                dbTran: dbTran,
                stringId: stringId,
                phoneId: phoneId,
                phoneTypeId: 1,
                countryCode: advPhone.CountryCode,
                code: advPhone.Code,
                number: advPhone.Number,
                additionalNumber: null,
                description: advPhone.Description,
                orderBy: advPhone.OrderBy,
                isActual: true);
        }

        private void CreateStringPhones(int stringId, int companyId, IEnumerable<AdvPhone> advPhones, DbTransaction dbTran)
        {
            foreach (var advPhone in advPhones)
                CreateStringPhone(stringId, companyId, advPhone, dbTran);
        }

        #endregion

        #region Update

        private void UpdateStringPhones(int stringId, int companyId, IEnumerable<StringPhone> stringPhones, IEnumerable<AdvPhone> advPhones,
            DbTransaction dbTran)
        {
            var stringPhonesList = stringPhones.GetActualItems().ToList();

            for (int i = stringPhonesList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringPhone(stringPhonesList[i], advPhones))
                    DeleteStringPhone(stringPhonesList[i], dbTran);

            foreach (var advPhone in advPhones)
                if (NeedCreateStringPhone(advPhone, stringPhones))
                    CreateStringPhone(stringId, companyId, advPhone, dbTran);
        }

        #endregion

        #region Delete

        private bool NeedDeleteStringPhone(StringPhone stringPhone, IEnumerable<AdvPhone> advPhones)
        {
            return !advPhones.Any(
                ph =>
                    ph.CountryCode == stringPhone.CountryCode &&
                    ph.Code == stringPhone.Code &&
                    ph.Number == stringPhone.Number &&
                    ph.Description == stringPhone.Description &&
                    ph.OrderBy == stringPhone.OrderBy);
        }

        private void DeleteStringPhone(StringPhone stringPhone, DbTransaction dbTran)
        {
            _repository.SetStringPhone(
                dbTran: dbTran,
                stringId: stringPhone.StringId,
                phoneId: stringPhone.PhoneId,
                phoneTypeId: stringPhone.PhoneTypeId,
                countryCode: stringPhone.CountryCode,
                code: stringPhone.Code,
                number: stringPhone.Number,
                additionalNumber: stringPhone.AdditionalNumber,
                description: stringPhone.Description,
                orderBy: stringPhone.OrderBy,
                isActual: false);

            _context.Entry(stringPhone).Reload();
        }

        private void DeleteStringPhones(IEnumerable<StringPhone> stringPhones, DbTransaction dbTran)
        {
            var stringPhonesList = stringPhones.GetActualItems().ToList();

            for (int i = stringPhonesList.Count - 1; i >= 0; i--)
                DeleteStringPhone(stringPhonesList[i], dbTran);
        }

        #endregion
    }
}
