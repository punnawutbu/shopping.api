using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopping.api.Shared.Models
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string SystemUrl { get; set; }
    }
    public class UserJWTPayload
    {
        public string Iss { get; set; }
        public string UserName { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string RoleId { get; set; }
        public int LifeTime { get; set; }
    }
    public class JwtToken
    {

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string Nbf { get; set; }
        public string Exp { get; set; }
        public string Iss { get; set; }

    }
    public static class TokenType
    {
        public const string OneTimeToken = "oneTimeToken";

        public const string AccessToken = "accessToken";

        public const string RefreshToken = "refreshToken";

        public const string ResetPasswordToken = "resetPasswordToken";
    }
}