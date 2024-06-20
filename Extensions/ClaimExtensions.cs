using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kallum.Extensions
{
    public static class ClaimExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            // Check if the user is null
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Try to find the user ID claim
            var userIdClaim = user.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            // If the user ID claim is found, return its value
            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }

            // If the user ID claim is not found, throw an exception or return a default value, depending on your requirements
            throw new InvalidOperationException("User ID claim not found.");
        }
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;
        }
    }
}
