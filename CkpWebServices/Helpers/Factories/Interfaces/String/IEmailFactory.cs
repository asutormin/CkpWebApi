using CkpDAL.Entities.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IEmailFactory
    {
        Web Create(int compnayId, string value, string description);
    }
}
