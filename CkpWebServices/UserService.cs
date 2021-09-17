using CkpDAL;
using CkpModel.Input;
using CkpModel.Output;
using CkpInfrastructure.Configuration;
using CkpServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CkpServices
{
    public class UserService : IUserService 
    {
        private readonly AppSettings _appSettings;
        private readonly BPFinanceContext _context;

        public UserService(IOptions<AppSettings> appSettings, BPFinanceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public AuthInfo Authenticate(AuthData authData)
        {
            var login = authData.Login;
            var passwordMd5 = CreateMd5(authData.Password);

            var userInfo = _context.LoginSettings
                .Include(ls => ls.LegalPerson.Company)
                .Include(ls => ls.LegalPerson).ThenInclude(lp  => lp.AccountSettings)
                .Where(ls => ls.Login == login && ls.PasswordMd5 == passwordMd5)
                .Select(
                    ls =>
                        new AuthInfo
                        {
                            ClientId = ls.LegalPerson.CompanyId,
                            ClientName = ls.LegalPerson.Company.Name,
                            ClientLegalPersonId = ls.LegalPersonId,
                            ClientLegalPersonName = ls.LegalPerson.Name,
                            IsNeedPrepayment = ls.LegalPerson.AccountSettings.IsNeedPrepayment,
                            Login = ls.Login
                        })
                .SingleOrDefault();

            if (userInfo == null)
                return null;

            userInfo.Token = GetToken(userInfo);

            return userInfo;
        }

        public AuthInfo GetByClientLegalPersonId(int clientLegalPersonId)
        {
            var userInfo = _context.LoginSettings
                .Include(ls => ls.LegalPerson.Company)
                .Where(ls => ls.LegalPersonId == clientLegalPersonId)
                .Select(
                    ls =>
                        new AuthInfo
                        {
                            ClientId = ls.LegalPerson.CompanyId,
                            ClientName = ls.LegalPerson.Company.Name,
                            ClientLegalPersonId = ls.LegalPersonId,
                            ClientLegalPersonName = ls.LegalPerson.Name,
                            Login = ls.Login
                        })
                .SingleOrDefault();

            return userInfo;
        }

        public static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public bool CanAccessOrderPosition(int clientLegalPersonId, int orderPositionId)
        {
            var orderPosition = _context.OrderPositions
                .Include(op => op.Order)
                .SingleOrDefault(
                    op => 
                        op.Order.ClientLegalPersonId == clientLegalPersonId &&
                        op.Id == orderPositionId);

            return orderPosition != null;
        }

        public bool CanAccessAccount(int clientLegalPersonId, int accountId)
        {
            var clientAccount = _context.Accounts
                .SingleOrDefault(
                    ac =>
                        ac.LegalPersonId == clientLegalPersonId &&
                        ac.Id == accountId);

            return clientAccount != null;
        }

        private string GetToken(AuthInfo authInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("client_legal_person_id", authInfo.ClientLegalPersonId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(480),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
