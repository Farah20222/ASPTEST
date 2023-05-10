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

        /// <summary>
        /// HTTP POST endpoint handler for user login
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns> The user object and token</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            // Authenticate the user by calling the 'AuthenticateAsync' method
            var user = await userRepository.AuthenticateAsync(loginRequest.Email, loginRequest.Password);

            // Check if the user details are correct
            if (user == null)
            {
                return BadRequest("Email or Password is incorrect");
            }

            // Create an authentication token for the user
            var token = await tokenHandler.CreateTokenAsync(user);

            // Return an 'OkObjectResult' containing the authentication token and user object
            return Ok(new { token, user });
        }


        /// <summary>
        /// HTTP POST endpoint handler for customer/user registration
        /// </summary>
        /// <param name="addUser"></param>
        /// <returns> The newly created user and their token</returns>
        [HttpPost("UserRegistration")]
        public async Task<IActionResult> RegisterUser([FromForm] AddUser addUser)
        {
            //Check if a user with the given email already exists in the database
            var userExists = await userRepository.GetByEmailAsync(addUser.Email);
            if (userExists != null)
            {
                return BadRequest("User already exists");
            }

            // Create a new 'UserProfile' object for customers
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

            // Register the new user in the database
            user = await userRepository.RegisterAsync(user);

            // Set the password and password salt properties of the user object to null for security
            user.Password = null;
            user.PasswordSalt = null;

            // Create an authentication token for the user
            var token = await tokenHandler.CreateTokenAsync(user);

            // Return an 'OkObjectResult' containing the authentication token and user object
            return Ok(new { token, user });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addUser"></param>
        /// <returns></returns>
        [HttpPost("VendorRegisteration")]
        public async Task<IActionResult> RegisterVendor([FromForm] AddVendor addUser)
        {
            //Check if a user with the given email already exists in the database
            var userExists = await userRepository.GetByEmailAsync(addUser.Email);
            if (userExists != null)
            {
                return BadRequest("User already exists");
            }

            // Create a new 'UserProfile' object for vendors
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

            // Register the new vendor in the database
            user = await userRepository.RegisterAsync(user);
           
            // Set the password and password salt properties of the user object to null for security
            user.Password = null;
            user.PasswordSalt = null;

            // Create an authentication token for the user
            var token = await tokenHandler.CreateTokenAsync(user);

            // Return an 'OkObjectResult' containing the authentication token and user object
            return Ok(new { token, user });
        }


        /// <summary>
        /// HTTP GET method to get a user by ID
        /// Auhorization: Admin only 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HTTP PUT endpoint to change a user's password
        /// Authorize the request (user must be logged in)
        /// </summary>
        /// <param name="updatePasswordRequest"></param>
        /// <returns> If successful, method will return "Password has been changed successfully.."</returns>
        [HttpPut("ChangePassword")]
        [ActionName("UpdatePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            // Get the user ID from the current HTTP context's user claims
            int userId = UserClaims.GetUserClaimID(HttpContext);

            // Verify that the current password provided in the 'updatePasswordRequest' object is correct 
            if (await userRepository.VerifyCurrentPasswordAsync(userId, updatePasswordRequest.CurrentPassword) == false)
            {
                return BadRequest("User is not found or current password is incorrect");
            }
            // Update the user's password in the database
            await userRepository.ChangePassAsync(userId, updatePasswordRequest.CurrentPassword);

            // Return an 'OkObjectResult'
            return Ok("Password has been changed successfully.");
        }

        /// <summary>
        /// HTTP POST Endpoint for sending a password reset request 
        /// </summary>
        /// <param name="forgetPassword"></param>
        /// <returns></returns>
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgetPasswordRequest([FromBody] ForgetPasswordDTO forgetPassword)
        {
            // Retrieve the user from the database
            var user = await userRepository.GetByEmailAsync(forgetPassword.Email);
            // If no user is found with the given email, return a 404 Not Found response with an appropriate message
            if (user == null)
            {
                return NotFound("No user found with this email: " + forgetPassword.Email);
            }

            // Create a password reset token for the user
            var token = await tokenHandler.CreateTokenAsync(user);
            var date = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow);

            // Add the password reset token to the user's account in the database 
            user = await userRepository.AddChangePassTokenAsync(user, token, date);

            // Generate a password reset link using the token
            var passwordResetLink = token;

            return Ok(token);
        }

        /// <summary>
        /// Resets the forgotten password through the generated token from the ForgetPassword endpoint
        /// </summary>
        /// 
        /// <param name="resetPassword">a request where the reset password parameters are set:
        /// - Token: the forget password token generated from email
        /// - NewPassword: the newly generated password 
        /// </param>
        /// 
        /// <returns type="User">
        /// The updated user with the generated password
        /// </returns>
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
