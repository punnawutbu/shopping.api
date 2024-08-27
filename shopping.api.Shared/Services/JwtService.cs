using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using shopping.api.Shared.Models;
using shopping.api.Shared.ResponseMessage;

namespace shopping.api.Shared.Services
{
    public class JwtService : IJwtService
    {
        private string _secretToken;
        private readonly int _lifeTime = 60 * 60 * 24 * 30;

        public JwtService(string secretToken)
        {
            _secretToken = secretToken;
        }


        public ResponseMessage<JwtToken> CheckUserToken(string token)
        {
            var resp = new ResponseMessage<JwtToken>()
            {
                Result = new JwtToken(),
            };

            if (string.IsNullOrEmpty(token))
            {
                resp.Message = "Unauthorized Token";
                return resp;
            }


            if (!_VerifySignature(token))
            {
                resp.Message = "Unauthorized token invalid";
                return resp;
            }

            var jwt = _CheckJwt(token);
            if (jwt == null)
            {
                resp.Message = "Unauthorized token is expire";
                return resp;
            }
            return resp;

        }

        public string GenToken(UserJWTPayload req)
        {
            var startDate = DateTime.UtcNow;
            int lifeTime = _lifeTime;

            if (req.LifeTime != 0)
            {
                lifeTime = req.LifeTime;
            }

            var endDate = DateTime.UtcNow.AddSeconds(lifeTime);

            var Claims = new Claim[]
            {
                new Claim("userId", req.Iss),
                new Claim("roleId", req.RoleId),
                new Claim("systemId", req.SystemId),
                new Claim("systemName", req.SystemName),
                new Claim("au", req.UserName)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretToken));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(req.Iss, null, Claims, startDate, endDate, credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public JwtToken CheckExpireToken(string token)
        {
            var jwt = _CheckJwt(token);
            return jwt;
        }


        public bool CheckLicenseToken(string token)
        {
            return _VerifySignature(token);
        }



        #region private
        private string _GetClaimValue(JwtSecurityToken jwt, string claimType)
        {
            var claim = jwt.Claims.FirstOrDefault(claim => claim.Type == claimType);
            return (claim == null) ? null : claim?.Value;
        }

        private JwtToken _CheckJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var readToken = handler.CanReadToken(token);

            if (!readToken) return null;

            var jwt = handler.ReadJwtToken(token);
            var au = _GetClaimValue(jwt, "au");
            var iss = _GetClaimValue(jwt, "iss");
            var exp = _GetClaimValue(jwt, "exp");
            var nbf = _GetClaimValue(jwt, "nbf");
            var userId = _GetClaimValue(jwt, "userId");
            var roleId = _GetClaimValue(jwt, "roleId");
            var systemId = _GetClaimValue(jwt, "systemId");
            var systemName = _GetClaimValue(jwt, "systemName");


            DateTime now = DateTime.UtcNow;
            DateTime expireDate = new DateTime(1970, 1, 1).AddSeconds(int.Parse(exp));

            if (now > expireDate) return null;

            return new JwtToken
            {
                Iss = iss,
                Nbf = nbf,
                Exp = exp,
                UserId = userId,
                UserName = au,
                RoleId = roleId,
                SystemId = systemId,
                SystemName = systemName
            };
        }

        private async Task<bool> _CheckBlacklistToken(string token, string system)
        {
            await Task.Delay(1);
            return false;
        }

        private bool _VerifySignature(string token)
        {
            string jwt = token;
            string[] parts = jwt.Split(".".ToCharArray());
            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];//Base64UrlEncoded signature from the token

            byte[] bytesToSign = Encoding.UTF8.GetBytes(string.Join(".", header, payload));

            byte[] secret = Encoding.UTF8.GetBytes(_secretToken);

            var alg = new HMACSHA256(secret);
            var hash = alg.ComputeHash(bytesToSign);

            var computedSignature = _Base64UrlEncode(hash);

            if (signature != computedSignature)
            {
                return false;
            }

            return true;
        }

        public string GenerateLoginToken(string iss, string userName, string systemName)
        {
            var claims = new Claim[] //shound in jwt
            {
                    new Claim("iss", iss),
                    new Claim("userName", userName),
                    new Claim("systemName", systemName),
                    new Claim("type",  TokenType.OneTimeToken),
            };
            return GenerateToken(120, "devshift", claims);
        }
        public string GenerateResetToken(string iss, string userName, string systemName, string secretToken)
        {
            var claims = new Claim[] //shound in jwt
            {
                    new Claim("iss", iss),
                    new Claim("userName", userName),
                    new Claim("systemName", systemName),
                    new Claim("secretToken",  secretToken),
                    new Claim("type",  TokenType.ResetPasswordToken),
            };
            return GenerateToken(86400, "devshift", claims);
        }

        public string GenerateAccessToken(string userName, string systemName, string role, string permission, string page)
        {
            var claimsAccessToken = new Claim[]
            {
                    new Claim("userName", userName),
                    new Claim("systemName", systemName),
                    new Claim("roles",  role),
                    new Claim("permission",  permission),
                    new Claim("page",  page),
                    new Claim("type",  TokenType.AccessToken)
            };
            return GenerateToken(1800, "devshift", claimsAccessToken);
        }

        public string GenerateRefreshToken(string systemId, string userUuId)
        {

            var claimsRefreshToken = new Claim[]
            {
                new Claim("aud", systemId),
                new Claim("sbf", userUuId),
                new Claim("type",  TokenType.RefreshToken),
            };
            return GenerateToken(86400, "devshift", claimsRefreshToken);
        }

        public string GenerateToken(int lifeTime, string iss, Claim[] claims)
        {
            var startDate = DateTime.UtcNow;

            var endDate = DateTime.UtcNow.AddSeconds(lifeTime);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretToken));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(iss, null, claims, startDate, endDate, credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string _Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
        #endregion
    }
}