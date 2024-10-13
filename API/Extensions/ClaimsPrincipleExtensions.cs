using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Authentication;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static async Task<AppUser> GetUserByEmail(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var userToRetturn = await userManager.Users.FirstOrDefaultAsync(x => x.Email == user.GetEmail());

            if (userToRetturn == null)
                throw new AuthenticationException("User not found.");
            return userToRetturn;
        }
        public static async Task<AppUser> GetUserByEmailWithAddress(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var userToRetturn = await userManager.Users
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Email == user.GetEmail());

            if (userToRetturn == null)
                throw new AuthenticationException("User not found.");
            return userToRetturn;
        }
        public static string GetEmail(this ClaimsPrincipal user)
            {
                {
                    var email = user.FindFirstValue(ClaimTypes.Email);
                    if (email == null) throw new AuthenticationException("Email nije pronađen.");
                    return email;
                }
            }
        
    }
}
