using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

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

        public static string GetRoleFromToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var parts = token.Split('.');
            if (parts.Length > 1)
            {
                var payload = parts[1];
                var paddedPayload = payload.PadRight(4 * ((payload.Length + 3) / 4), '=');
                var decodedBytes = Convert.FromBase64String(paddedPayload);
                var decodedJson = System.Text.Encoding.UTF8.GetString(decodedBytes);
                var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(decodedJson);
                return tokenData["Role"]?.ToString();
            }
            return null;
        }
    }
}