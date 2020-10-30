using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Samples.GameMatch.Api
{
    public static class ControllerExtensions
    {
        public static ActionResult<GmApiResult<T>> AsOkGmApiResult<T>(this T response)
            => new OkObjectResult(AsGmApiResult(response));

        public static ActionResult<GmApiResults<T>> AsOkGmApiResults<T>(this IEnumerable<T> response)
            where T : class
            => new OkObjectResult(AsGmApiResults(response));

        public static GmApiResult<T> AsGmApiResult<T>(this T response)
            => new GmApiResult<T>
               {
                   Result = response
               };

        public static GmApiResults<T> AsGmApiResults<T>(this IEnumerable<T> response)
            where T : class
            => new GmApiResults<T>
               {
                   Results = response.AsListReadOnly()
               };

        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            if (principal?.Identity == null ||
                !(principal.Identity is ClaimsIdentity claimsIdentity) ||
                claimsIdentity?.Claims == null)
            {
                return default;
            }

            var idValue = claimsIdentity.Claims
                                        .SingleOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase) &&
                                                              !string.IsNullOrEmpty(c.Value));

            return Guid.TryParse(idValue?.Value, out var guidId)
                       ? guidId
                       : default;
        }

        public static void AddOrUpdate<T>(this IBaseModelRepository<T> repository, T model)
            where T : BaseModel
        {
            if (model.Id == default)
            {
                repository.Add(model);
            }
            else
            {
                repository.Update(model);
            }
        }

        public static string GenerateJwtToken(this User user, IConfiguration configuration)
            => GenerateJwtToken(user, configuration["Jwt:SecretKey"], configuration["Jwt:Audience"], configuration["Jwt:Issuer"]);

        public static string GenerateJwtToken(this User user, string jwtSecret, string audience, string issuer)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var userId = user.Id.ToString();

            var tokenDescriptor = new SecurityTokenDescriptor
                                  {
                                      Subject = new ClaimsIdentity(new[]
                                                                   {
                                                                       new Claim("id", userId),
                                                                       new Claim(ClaimTypes.NameIdentifier, userId),
                                                                       new Claim(ClaimTypes.Email, user.Email),
                                                                       new Claim(ClaimTypes.Role, user.Role.ToString().ToLowerInvariant()),
                                                                   }),
                                      Expires = DateTime.UtcNow.AddDays(7),
                                      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                                      Audience = audience,
                                      Issuer = issuer
                                  };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
