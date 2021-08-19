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

        private Phone GetCompanyPhone(int companyId, PhoneData advPhone)
        {
            var phone = _context.Phones
                .FirstOrDefault(
                    cph =>
                        cph.CompanyId == companyId &&
                        cph.CountryCode == advPhone.CountryCode &&
                        cph.Code == advPhone.Code &&
                        cph.Number == advPhone.Number &&
                        cph.Description == advPhone.Description &&
                        cph.IsActual == true);

            return phone;
        }

        #endregion

        #region Create

        private void CreateStringPhones(int stringId, int companyId, IEnumerable<PhoneData> advPhones, DbTransaction dbTran)
        {
            foreach (var advPhone in advPhones)
                CreateStringPhone(stringId, companyId, advPhone, dbTran);
        }

        private bool NeedCreateStringPhone(PhoneData advPhone, IEnumerable<StringPhone> stringPhones)
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

        private StringPhone CreateStringPhone(int stringId, int companyId, PhoneData advPhone, DbTransaction dbTran)
        {
            var phone = GetCompanyPhone(companyId, advPhone);

            if (phone == null)
                phone = CreateCompanyPhone(companyId, advPhone, dbTran);

            var stringPhone = _stringPhoneFactory.Create(stringId, phone, advPhone.OrderBy);
            _repository.SetStringPhone(stringPhone, isActual: true, dbTran);

            return stringPhone;
        }

        private Phone CreateCompanyPhone(int companyId, PhoneData advPhone, DbTransaction dbTran)
        {
            var phone = _phoneFactory.Create(companyId, advPhone.CountryCode, advPhone.Code, advPhone.Number, advPhone.Description);

            phone = _repository.SetPhone(phone, isActual: true, dbTran);

            return phone;
        }

        #endregion

        #region Update

        private void UpdateStringPhones(int stringId, int companyId, IEnumerable<StringPhone> stringPhones, IEnumerable<PhoneData> advPhones,
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

        private void DeleteStringPhones(IEnumerable<StringPhone> stringPhones, DbTransaction dbTran)
        {
            var stringPhonesList = stringPhones.GetActualItems().ToList();

            for (int i = stringPhonesList.Count - 1; i >= 0; i--)
                DeleteStringPhone(stringPhonesList[i], dbTran);
        }

        private bool NeedDeleteStringPhone(StringPhone stringPhone, IEnumerable<PhoneData> advPhones)
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
            _repository.SetStringPhone(stringPhone, isActual: false, dbTran);
            _context.Entry(stringPhone).Reload();
        }

        #endregion
    }
}
