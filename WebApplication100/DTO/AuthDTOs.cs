namespace WebApplication100.DTO
{
    public class AuthDTOs
    {
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class AddUser
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string Email { get; set; }
            public string? Password { get; set; }
            public int? Phone { get; set; }
        }

        public class AddVendor
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string Email { get; set; }
            public string? Password { get; set; }
            public int? Phone { get; set; }
        }


        public class UpdatePasswordRequest
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class ForgetPasswordDTO
        {
            public string Email { get; set; }
        }

        public class ResetPasswordDTO
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }

    }
}
