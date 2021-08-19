using CkpDAL.Entities.String;
using CkpServices.Helpers.Factories.Interfaces.String;

namespace CkpServices.Helpers.Factories.String
{
    class StringWebFactory : IStringWebFactory
    {
        public StringWeb Create(int stringId, Web web, int orderBy)
        {
            var stringWeb = new StringWeb
            {
                WebId = web.Id,
                StringId = stringId,
                WebTypeId = web.WebTypeId,
                WebResponse = web.WebResponse,
                WebValue = web.WebValue,
                Description = web.Description,
                OrderBy = orderBy
            };

            return stringWeb;
        }
    }
}
