namespace WebApplication100.DTO
{
    public class UserDTO
    {

        public class UserProfileDTO
        {
            public int UserProfileId { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }

            public DateTime? CreatedTime { get; set; }
            public int? Phone { get; set; }
            public string? RoleName { get; set; }
        }
    }
}
