using System.IdentityModel.Tokens.Jwt;

namespace FFTMS.RazorPages.Helpers
{
    public static class JwtHelper
    {
        public static string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.First(claim => claim.Type == "UserId").Value;
        }
    }
}