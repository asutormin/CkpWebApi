using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CkpWebApi
{
    public static class ExtensionMethodsControllers
    {
        public static int GetClientLegalPersonId(this HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var claimValue = identity.FindFirst("client_legal_person_id").Value;
                var clientLegalPersonId = int.Parse(claimValue);

                return clientLegalPersonId;
            }

            return 0;
        }
    }
}
