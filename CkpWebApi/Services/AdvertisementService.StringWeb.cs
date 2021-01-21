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
        private int CreateCompanyEmail(int companyId, AdvEmail advEmail, DbTransaction dbTran)
        {
            var webId = 0;

            // Создаём email компании
            _repository.SetStringCompanyWeb(
                dbTran: dbTran,
                id: ref webId,
                companyId: companyId,
                webTypeId: 2,
                webResponse: 31,
                webValue: advEmail.Value,
                description: advEmail.Description,
                isActual: true,
                editUserId: _editUserId);

            SetNeedRefreshOrderShoppingCartOrder();

            return webId;
        }

        #region Create

        private bool NeedCreateStringWeb(IEnumerable<StringWeb> stringWebs, AdvEmail advEmail)
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

        private void CreateStringWeb(int stringId, int companyId, AdvEmail advEmail, DbTransaction dbTran)
        {
            var web = GetCompanyEmail(companyId, advEmail);

            var webId =
                web == null
                ? CreateCompanyEmail(companyId, advEmail, dbTran)
                : web.Id;

            // Привязываем email к строке
            _repository.SetStringWeb(
                dbTran: dbTran,
                stringId: stringId,
                webId: webId,
                webTypeId: 2,
                webResponse: 31,
                webValue: advEmail.Value,
                description: advEmail.Description,
                orderBy: advEmail.OrderBy,
                isActual: true);

            SetNeedRefreshOrderShoppingCartOrder();
        }

        private void CreateStringWebs(int stringId, int companyId, IEnumerable<AdvEmail> advEmails, DbTransaction dbTran)
        {
            foreach (var advEmail in advEmails)
                CreateStringWeb(stringId, companyId, advEmail, dbTran);
        }

        #endregion

        #region Update

        private void UpdateStringWebs(int stringId, int companyId, IEnumerable<StringWeb> stringWebs, IEnumerable<AdvEmail> advEmails, DbTransaction dbTran)
        {
            var stringWebsList = stringWebs.GetActualItems().ToList();

            for (int i = stringWebsList.Count - 1; i >= 0; i--)
                if (NeedDeleteStringWeb(stringWebsList[i], advEmails))
                    DeleteStringWeb(stringWebsList[i], dbTran);

            foreach (var advEmail in advEmails)
                if (NeedCreateStringWeb(stringWebs, advEmail))
                    CreateStringWeb(stringId, companyId, advEmail, dbTran);
        }

        #endregion

        #region Delete

        private bool NeedDeleteStringWeb(StringWeb stringWeb, IEnumerable<AdvEmail> advEmails)
        {
            return !advEmails.Any(
                sw =>
                    sw.Value == stringWeb.WebValue &&
                    sw.Description == stringWeb.Description &&
                    sw.OrderBy == stringWeb.OrderBy);
        }

        private void DeleteStringWeb(StringWeb stringWeb, DbTransaction dbTran)
        {
            _repository.SetStringWeb(
                dbTran: dbTran,
                stringId: stringWeb.StringId,
                webId: stringWeb.WebId,
                webTypeId: stringWeb.WebTypeId,
                webResponse: stringWeb.WebResponse,
                webValue: stringWeb.WebValue,
                description: stringWeb.Description,
                orderBy: stringWeb.OrderBy,
                isActual: false);

            _context.Entry(stringWeb).Reload();
            SetNeedRefreshOrderShoppingCartOrder();
        }

        private void DeleteStringWebs(IEnumerable<StringWeb> stringWebs, DbTransaction dbTran)
        {
            var stringWebsList = stringWebs.GetActualItems().ToList();

            for (int i = stringWebsList.Count - 1; i >= 0; i--)
                DeleteStringWeb(stringWebsList[i], dbTran);
        }

        #endregion
    }
}
