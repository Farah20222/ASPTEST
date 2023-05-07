//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using WebApplication100.Models;

//namespace WebApplication100.Repository.Services
//{
//    public class TokenHandler: ITokenHandler
//    {
//        // to obtain JWT configurations
//        private readonly IConfiguration Configuration;
//        // to set the time zone to the regional time
//        private readonly ITimeZoneService timeZoneService;

//        public TokenHandler(IConfiguration Configuration, ITimeZoneService timeZoneService)
//        {
//            this.Configuration = Configuration;
//            this.timeZoneService = timeZoneService;
//        }

//        /// <summary>
//        /// Generates a token for the user when logged in
//        /// </summary>
//        /// 
//        /// <param name="user">the user model to be generated for</param>
//        /// 
//        /// <returns type="string">
//        /// The token created
//        /// </returns>
//        public Task<string> CreateTokenAsync(UserProfile user)
//        {
//            // gets key from settings
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

//            // generates claims to be assigned to token
//            var Claims = new List<Claim>();
//            Claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
//            Claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
//            Claims.Add(new Claim(ClaimTypes.Email, user.Email));
//            Claims.Add(new Claim("id", user.UserId.ToString()));
//            Claims.Add(new Claim(ClaimTypes.Role, user.RoleName));

//            // creates encryption algorithm
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            // creates token
//            var token = new JwtSecurityToken(
//                Configuration["Jwt:Issuer"],
//                Configuration["Jwt:Audience"],
//                Claims,
//                expires: timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow.AddDays(1)),
//                signingCredentials: credentials);

//            // write token and return token string
//            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
//        }
//    }
//}
