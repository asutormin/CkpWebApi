using CkpDAL.Model.String;
using CkpServices.Helpers.Factories.Interfaces.String;

namespace CkpServices.Helpers.Factories.String
{
    class EmailFactory : IEmailFactory
    {
        public Web Create(int compnayId, string value, string description)
        {
            var email = new Web
            {
                Id = 0,
                CompanyId = compnayId,
                WebTypeId = 2,
                WebResponse = 31,
                WebValue = value,
                Description = description
            };

            return email;
        }
    }
}
