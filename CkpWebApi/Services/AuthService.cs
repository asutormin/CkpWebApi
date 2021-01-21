using CkpWebApi.Helpers;
using CkpWebApi.OutputEntities;
using CkpWebApi.Services.Interfaces;
using DebtsWebApi.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CkpWebApi.Services
{
    public class AuthService : IAuthService 
    {
        private readonly AppSettings _appSettings;
        private readonly BPFinanceContext _context;

        public AuthService(IOptions<AppSettings> appSettings, BPFinanceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public AuthInfo Authenticate(string login, string password, bool createMd5)
        {
            var passwordMd5 = createMd5 ? CreateMd5(password) : password;
            var authInfo = GetAuthInfo(login, passwordMd5);

            if (authInfo == null)
                return null;

            authInfo.Token = GetToken(authInfo.ClientLegalPersonId.ToString());

            return authInfo;
        }

        public AuthInfo SetLogin(string oldLogin, string newLogin)
        {
            var loginSettings = _context.LoginSettings
                .SingleOrDefault(ls => ls.Login == oldLogin);

            if (loginSettings == null)
                return null;

            loginSettings.Login = newLogin;
            _context.SaveChanges();

            var authInfo = Authenticate(loginSettings.Login, loginSettings.PasswordMd5, false);

            return authInfo;
        }

        public AuthInfo SetPassword(string login, string newPassword)
        {
            var loginSettings = _context.LoginSettings
                .SingleOrDefault(ls => ls.Login == login);

            if (loginSettings == null)
                return null;

            var newPasswordMd5 = CreateMd5(newPassword);

            loginSettings.PasswordMd5 = newPasswordMd5;
            _context.SaveChanges();

            var authInfo = Authenticate(loginSettings.Login, loginSettings.PasswordMd5, false);

            return authInfo;
        }

        private AuthInfo GetAuthInfo(string login, string passwordMd5)
        {
            var authInfo = _context.LoginSettings
                .Include(ls => ls.LegalPerson.Company)
                .Where(ls => ls.Login == login && ls.PasswordMd5 == passwordMd5)
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

            return authInfo;
        }

        private string GetToken(string name)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, name)
                }),
                Expires = DateTime.UtcNow.AddMinutes(480),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
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
    }
}
