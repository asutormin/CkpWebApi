using CkpDAL.Model.String;
using CkpEntities.Input.String;
using CkpServices.Helpers;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.String
{
    partial class StringProcessor
    {
        #region Get

        private Web GetCompanyEmail(int companyId, AdvEmail advEmail)
        {
            var companyEmail = _context.Webs
                .FirstOrDefault(
                    w =>
                        w.CompanyId == companyId &&
                        w.WebTypeId == 2 &&
                        w.WebResponse == 31 &&
                        w.WebValue == advEmail.Value &&
                        w.Description == advEmail.Description &&
                        w.IsActual == true);

            return companyEmail;
        }

        #endregion

        #region Create

        private void CreateStringEmails(int stringId, int companyId, IEnumerable<AdvEmail> advEmails, DbTransaction dbTran)
        {
            foreach (var advEmail in advEmails)
                CreateStringEmail(stringId, companyId, advEmail, dbTran);
        }

        private bool NeedCreateStringEmail(IEnumerable<StringWeb> stringWebs, AdvEmail advEmail)
        {
            return !stringWebs
                .GetActualItems()
                .Any(
                    sw =>
                        sw.WebTypeId == 2 &&
                        sw.WebResponse == 31 &&
                        sw.WebValue == advEmail.Value &&
                        sw.Description == advEmail.Description &&
                        sw.OrderBy == advEmail.OrderBy);
        }

        private StringWeb CreateStringEmail(int stringId, int companyId, AdvEmail advEmail, DbTransaction dbTran)
        {
            var email = GetCompanyEmail(companyId, advEmail);

            if (email == null)
                email = CreateCompanyEmail(companyId, advEmail, dbTran);

            var stringWeb = _stringWebFactory.Create(stringId, email, advEmail.OrderBy);

            stringWeb = _repository.SetStringWeb(stringWeb, isActual: true, dbTran);

            return stringWeb;
        }

        private Web CreateCompanyEmail(int companyId, AdvEmail advEmail, DbTransaction dbTran)
        {
            var email = _emailFactory.Create(companyId, advEmail.Value, advEmail.Description);

            _repository.SetWeb(email, isActual: true, dbTran);

            return email;
        }

        #endregion

        #region Update

        private void UpdateStringEmails(int stringId, int companyId, IEnumerable<StringWeb> stringEmails, IEnumerable<AdvEmail> advEmails, DbTransaction dbTran)
        {
            var stringWebsList = stringEmails.GetActualItems().ToList();

            for (int i = stringWebsList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringEmail(stringWebsList[i], advEmails))
                    DeleteStringEmail(stringWebsList[i], dbTran);

            foreach (var advEmail in advEmails)
                if (NeedCreateStringEmail(stringEmails, advEmail))
                    CreateStringEmail(stringId, companyId, advEmail, dbTran);
        }

        #endregion

        #region Delete

        private void DeleteStringEmails(IEnumerable<StringWeb> stringWebs, DbTransaction dbTran)
        {
            var stringWebsList = stringWebs.GetActualItems().ToList();

            for (int i = stringWebsList.Count - 1; i >= 0; i--)
                DeleteStringEmail(stringWebsList[i], dbTran);
        }

        private bool NeedDeleteStringEmail(StringWeb stringEmail, IEnumerable<AdvEmail> advEmails)
        {
            return !advEmails.Any(
                sw =>
                    sw.Value == stringEmail.WebValue &&
                    sw.Description == stringEmail.Description &&
                    sw.OrderBy == stringEmail.OrderBy);
        }

        private void DeleteStringEmail(StringWeb stringEmail, DbTransaction dbTran)
        {
            _repository.SetStringWeb(stringEmail, isActual: false, dbTran);
            _context.Entry(stringEmail).Reload();
        }

        #endregion 
    }
}
