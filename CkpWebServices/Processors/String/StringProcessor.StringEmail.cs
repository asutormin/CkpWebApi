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

        private Web GetCompanyEmail(int companyId, EmailData advEmail)
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

        private void CreateStringEmails(int stringId, int companyId, IEnumerable<EmailData> emailsData, DbTransaction dbTran)
        {
            foreach (var advEmail in emailsData)
                CreateStringEmail(stringId, companyId, advEmail, dbTran);
        }

        private bool NeedCreateStringEmail(IEnumerable<StringWeb> stringWebs, EmailData emailData)
        {
            return !stringWebs
                .GetActualItems()
                .Any(
                    sw =>
                        sw.WebTypeId == 2 &&
                        sw.WebResponse == 31 &&
                        sw.WebValue == emailData.Value &&
                        sw.Description == emailData.Description &&
                        sw.OrderBy == emailData.OrderBy);
        }

        private StringWeb CreateStringEmail(int stringId, int companyId, EmailData emailData, DbTransaction dbTran)
        {
            var email = GetCompanyEmail(companyId, emailData);

            if (email == null)
                email = CreateCompanyEmail(companyId, emailData, dbTran);

            var stringWeb = _stringWebFactory.Create(stringId, email, emailData.OrderBy);

            stringWeb = _repository.SetStringWeb(stringWeb, isActual: true, dbTran);

            return stringWeb;
        }

        private Web CreateCompanyEmail(int companyId, EmailData emailData, DbTransaction dbTran)
        {
            var email = _emailFactory.Create(companyId, emailData.Value, emailData.Description);

            _repository.SetWeb(email, isActual: true, dbTran);

            return email;
        }

        #endregion

        #region Update

        private void UpdateStringEmails(int stringId, int companyId, IEnumerable<StringWeb> stringEmails, IEnumerable<EmailData> emailsData, DbTransaction dbTran)
        {
            var stringWebsList = stringEmails.GetActualItems().ToList();

            for (int i = stringWebsList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringEmail(stringWebsList[i], emailsData))
                    DeleteStringEmail(stringWebsList[i], dbTran);

            foreach (var advEmail in emailsData)
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

        private bool NeedDeleteStringEmail(StringWeb stringEmail, IEnumerable<EmailData> advEmails)
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
