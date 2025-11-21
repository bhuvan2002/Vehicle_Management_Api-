using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using VehicleManagementAPI.Data;

namespace VehicleManagementAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, dbContext, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, ApplicationDbContext dbContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

                var tokenValidationResult = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                });

                if (tokenValidationResult.IsValid)
                {
                    var jwtToken = (JwtSecurityToken)tokenValidationResult.SecurityToken;
                    var userIdClaim = jwtToken.Claims.First(x => x.Type == "nameid").Value;

                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        var user = await dbContext.Users
                            .Include(u => u.Role)
                            .FirstOrDefaultAsync(u => u.UserId == userId);

                        if (user != null)
                        {
                            context.Items["User"] = user;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JWT validation failed: {ex.Message}");
            }
        }
    }
}