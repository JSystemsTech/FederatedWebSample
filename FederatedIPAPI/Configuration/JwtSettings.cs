using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace FederatedIPAPI.Configuration
{
    public interface IJwtSettings
    {
        int ValidFor { get; }
        string Key { get; }

        string Issuer { get; }

        string Audience { get; }
        string Subject { get; }
        string CreateJwtToken(Guid clientGuid);
        JwtSecurityToken ParseJwtToken(Microsoft.AspNetCore.Http.IHeaderDictionary headers);
        JwtSecurityToken ParseJwtToken(string token);
        string GetClaim(string token, string type);
        string GetClaim(Microsoft.AspNetCore.Http.IHeaderDictionary headers, string type);

        Guid? GetClientGuid(string token);
        Guid? GetClientGuid(Microsoft.AspNetCore.Http.IHeaderDictionary headers);
    }
    public class JwtSettings: IJwtSettings
    {
        public int ValidFor { get; set; }
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
        public string Subject { get; set; }

        internal static string DefaultConfigurationSetting = "Jwt";
        internal static IJwtSettings Create(IConfiguration Configuration)
        {
            IConfigurationSection section = Configuration.GetSection(DefaultConfigurationSetting);
            return new JwtSettings()
            {
                ValidFor = section.GetValue<int>("ValidFor"),
                Key = section.GetValue<string>("Key"),
                Issuer = section.GetValue<string>("Issuer"),
                Audience = section.GetValue<string>("Audience"),
                Subject = section.GetValue<string>("Subject")
            };
        }
        private static JwtSecurityTokenHandler JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private static string JwtClientGuid = "ClientGuid";
        private IEnumerable<Claim> BuildJwtTokenClaims(Guid clientGuid) => new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Sub, Subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(JwtClientGuid, clientGuid.ToString())
                   };
        private SigningCredentials BuildJwtSigningCredentials()
            => new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)), SecurityAlgorithms.HmacSha256);
        private JwtSecurityToken BuildJwtToken(Guid clientGuid)
            => new JwtSecurityToken(Issuer, Audience, BuildJwtTokenClaims(clientGuid), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(ValidFor), BuildJwtSigningCredentials());
        public string CreateJwtToken(Guid clientGuid) => JwtSecurityTokenHandler.WriteToken(BuildJwtToken(clientGuid));

        private string GetTokenStringFromHeader(Microsoft.AspNetCore.Http.IHeaderDictionary headers)
        {
            try
            {
                string authHeader = headers["Authorization"];
                if (authHeader != null)
                {
                    var authHeaderValue = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(authHeader);
                    if (authHeaderValue.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        return authHeaderValue.Parameter;
                    }
                    return null;
                }

                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
        public JwtSecurityToken ParseJwtToken(Microsoft.AspNetCore.Http.IHeaderDictionary headers) => ParseJwtToken(GetTokenStringFromHeader(headers));
        public JwtSecurityToken ParseJwtToken(string token) => JwtSecurityTokenHandler.CanReadToken(token) ? JwtSecurityTokenHandler.ReadJwtToken(token) : null;
        public string GetClaim(string token, string type)
            =>  ParseJwtToken(token) is JwtSecurityToken jwtSecurityToken ? jwtSecurityToken.Claims.FirstOrDefault(c=> c.Type == type).Value : null;
        public string GetClaim(Microsoft.AspNetCore.Http.IHeaderDictionary headers, string type)
            => ParseJwtToken(headers) is JwtSecurityToken jwtSecurityToken ? jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == type).Value : null;

        public Guid? GetClientGuid(string token)=> GetClaim(token, JwtClientGuid) is string clientGuidStr && string.IsNullOrWhiteSpace(clientGuidStr) ? Guid.Parse(clientGuidStr) : default(Guid?);
        public Guid? GetClientGuid(Microsoft.AspNetCore.Http.IHeaderDictionary headers) => GetClaim(headers, JwtClientGuid) is string clientGuidStr && string.IsNullOrWhiteSpace(clientGuidStr) ? Guid.Parse(clientGuidStr) : default(Guid?);

    }
}
