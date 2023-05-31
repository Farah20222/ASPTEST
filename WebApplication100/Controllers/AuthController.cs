using Amazon.Auth.AccessControlPolicy;
using Amazon.JSII.JsonModel.Api.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;
using static WebApplication100.DTO.AuthDTOs;

namespace WebApplication100.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        // to access user database methods
        private readonly IUserRepository userRepository;

        // to generate authentication token
        private readonly ITokenHandler tokenHandler;

        // to set the timezone to regional time
        private readonly ITimeZoneService timeZoneService;

        public AuthController(IUserRepository userRepository, ITokenHandler tokenHandler, ITimeZoneService timeZoneService)
        {
            this.tokenHandler = tokenHandler;
            this.userRepository = userRepository;
            this.timeZoneService = timeZoneService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            var user = await userRepository.AuthenticateAsync(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return BadRequest("Email or Password is incorrect");
            }

            var token = await tokenHandler.CreateTokenAsync(user);

            return Ok(new { token, user });
        }


        [HttpPost("UserRegistration")]
        public async Task<IActionResult> RegisterUser([FromForm] AddUser addUser)
        {
            var userExists = await userRepository.GetByEmailAsync(addUser.Email);
            if (userExists != null)
            {
                return BadRequest("User already exists");
            }

            var user = new UserProfile()
            {
                FirstName = addUser.FirstName,
                LastName = addUser.LastName,
                Email = addUser.Email,
                Phone = addUser.Phone,
                Password = addUser.Password,
                CreatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow),
                RoleName = "customer",
            };

            user = await userRepository.RegisterAsync(user);

            user.Password = null;
            user.PasswordSalt = null;

            var token = await tokenHandler.CreateTokenAsync(user);

            return Ok(new { token, user });
        }

        [HttpPost("VendorRegisteration")]
        public async Task<IActionResult> RegisterVendor([FromForm] AddVendor addUser)
        {
            var userExists = await userRepository.GetByEmailAsync(addUser.Email);
            if (userExists != null)
            {
                return BadRequest("User already exists");
            }

            var user = new UserProfile()
            {
                FirstName = addUser.FirstName,
                LastName = addUser.LastName,
                Email = addUser.Email,
                Phone = addUser.Phone,
                Password = addUser.Password,
                CreatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow),
                RoleName = "vendor",
            };

            user = await userRepository.RegisterAsync(user);
            user.Password = null;
            user.PasswordSalt = null;

            var token = await tokenHandler.CreateTokenAsync(user);

            return Ok(new { token, user });
        }


        [HttpGet("User/{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<UserProfile>> GetUser(int id)
        {
            var user = await userRepository.GetByUserId(id);
            if (user == null)
            {
                return NotFound($"No user found with the given id: {id}");
            }
            return Ok(user);
        }

        [HttpPut("ChangePassword")]
        [ActionName("UpdatePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            int userId = UserClaims.GetUserClaimID(HttpContext);

            if (await userRepository.VerifyCurrentPasswordAsync(userId, updatePasswordRequest.CurrentPassword) == false)
            {
                return BadRequest("User is not found or current password is incorrect");
            }
            await userRepository.ChangePassAsync(userId, updatePasswordRequest.CurrentPassword);

            return Ok("Password has been changed successfully.");
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgetPasswordRequest([FromBody] ForgetPasswordDTO forgetPassword)
        {
            var user = await userRepository.GetByEmailAsync(forgetPassword.Email);
            if (user == null)
            {
                return NotFound("No user found with this email: " + forgetPassword.Email);
            }

            var token = await tokenHandler.CreateTokenAsync(user);
            var date = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow);

            user = await userRepository.AddChangePassTokenAsync(user, token, date);

            var passwordResetLink = token;

            return Ok(token);
        }

        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            var user = await userRepository.GetByTokenAsync(resetPassword.Token);
            if (User == null)
            {
                return NotFound("User is not found");
            }

            var now = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow);
            if (!(user.ForgetCreatedAt > now.AddHours(-24) && user.ForgetCreatedAt <= now))
            {
                return BadRequest("Reset password link has expired");
            }
            user = await userRepository.ForgetPasswordAsync(user, resetPassword.NewPassword);
            return Ok(user);
        }


    }
}
