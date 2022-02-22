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

        private Phone GetCompanyPhone(int companyId, PhoneData phoneData)
        {
            var phone = _context.Phones
                .FirstOrDefault(
                    cph =>
                        cph.CompanyId == companyId &&
                        cph.CountryCode == phoneData.CountryCode &&
                        cph.Code == phoneData.Code &&
                        cph.Number == phoneData.Number &&
                        cph.AdditionalNumber == phoneData.AdditionalNumber &&
                        cph.Description == phoneData.Description &&
                        cph.IsActual == true);

            return phone;
        }

        #endregion

        #region Create

        private void CreateStringPhones(int stringId, int companyId, IEnumerable<PhoneData> phonesData, DbTransaction dbTran)
        {
            foreach (var advPhone in phonesData)
                CreateStringPhone(stringId, companyId, advPhone, dbTran);
        }

        private bool NeedCreateStringPhone(PhoneData phoneData, IEnumerable<StringPhone> stringPhones)
        {
            return !stringPhones
                .GetActualItems()
                .Any(
                    sp =>
                        sp.CountryCode == phoneData.CountryCode &&
                        sp.Code == phoneData.Code &&
                        sp.Number == phoneData.Number &&
                        sp.Description == phoneData.Description &&
                        sp.AdditionalNumber == phoneData.AdditionalNumber &&
                        sp.OrderBy == phoneData.OrderBy);
        }

        private StringPhone CreateStringPhone(int stringId, int companyId, PhoneData phonesData, DbTransaction dbTran)
        {
            var phone = GetCompanyPhone(companyId, phonesData);

            if (phone == null)
                phone = CreateCompanyPhone(companyId, phonesData, dbTran);

            var stringPhone = _stringPhoneFactory.Create(stringId, phone, phonesData.OrderBy);
            _repository.SetStringPhone(stringPhone, isActual: true, dbTran);

            return stringPhone;
        }

        private Phone CreateCompanyPhone(int companyId, PhoneData phoneData, DbTransaction dbTran)
        {
            var phone = _phoneFactory.Create(companyId,
                phoneData.CountryCode,
                phoneData.Code,
                phoneData.Number,
                phoneData.AdditionalNumber,
                phoneData.Description);

            phone = _repository.SetPhone(phone, isActual: true, dbTran);

            return phone;
        }

        #endregion

        #region Update

        private void UpdateStringPhones(int stringId, int companyId, IEnumerable<StringPhone> stringPhones, IEnumerable<PhoneData> phonesData,
            DbTransaction dbTran)
        {
            var stringPhonesList = stringPhones.GetActualItems().ToList();

            for (int i = stringPhonesList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringPhone(stringPhonesList[i], phonesData))
                    DeleteStringPhone(stringPhonesList[i], dbTran);

            foreach (var advPhone in phonesData)
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
                    ph.AdditionalNumber == stringPhone.AdditionalNumber &&
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
