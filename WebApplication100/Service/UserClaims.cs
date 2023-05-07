using System.Security.Claims;

namespace WebApplication100.Service
{
    public static class UserClaims
    {
        public static int GetUserClaimID(HttpContext httpContext)
        {
            var userClaims = GetUserClaims(httpContext);

            var stringID = userClaims.FirstOrDefault(o => o.Type == "id")?.Value;

            if (stringID == null)
            {
                return 0;
            }

            int intID = 0;

            // parse the claimId as it is stored as string
            try
            {
                intID = int.Parse(stringID);
            }
            catch (Exception ex)
            {
                throw;

            }

            return intID;
        }

        /*
         * Returns all the claims of the logged in user
         */
        private static IEnumerable<Claim> GetUserClaims(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return null;
            }

            return identity.Claims;
        }
    }
}
