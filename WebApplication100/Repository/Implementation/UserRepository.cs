using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;

namespace WebApplication100.Repository.Implementation
{
    public class UserRepository: IUserRepository
    {
        private readonly AssignmentDBContext assignmentDBContext;
        public UserRepository(AssignmentDBContext assignmentDBContext)
        {
            this.assignmentDBContext = assignmentDBContext;
        }

        public async Task<UserProfile> AddChangePassTokenAsync(UserProfile user, string token, DateTime date)
        {
            user.ForgetPassword = token;
            user.ForgetCreatedAt = date;

            await assignmentDBContext.SaveChangesAsync();

            return user;
        }

        public async Task<UserProfile> AuthenticateAsync(string email, string password)
        {
            var user = await assignmentDBContext.UserProfiles
                          .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

            if (user == null)
            {

                return null;
            }

            // check if password is correct
            if (user.Password == null || user.PasswordSalt == null)
            {
                return null;
            }
            if (!VerifyPassword(password, user.Password, user.PasswordSalt))
            {
                return null;
            }

            user.Password = null;
            user.PasswordSalt = null;
            return user;
        }

        public async Task<UserProfile> ChangePassAsync(int? userId, string newPassword)
        {
            // checks if the user id has the correct current password
            var user = await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.UserProfileId == userId);

            if (user == null)
            {
                return null;
            }

            // hashes and salts new password
            string passwordHash, passwordSalt;
            CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;
            await assignmentDBContext.SaveChangesAsync();

            user.Password = null;
            user.PasswordSalt = null;
            user.ForgetPassword = null;
            return user;

        }


        public async Task<UserProfile> ForgetPasswordAsync(UserProfile user, string password)
        {
            // hashes and salts new password
            string passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;

            await assignmentDBContext.SaveChangesAsync();

            user.Password = null;
            user.PasswordSalt = null;
            user.ForgetPassword = null;

            return user;
        }

        public async Task<UserProfile> GetByEmailAsync(string email)
        {
            return await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<UserProfile> GetByUserId(int? userId)
        {
            return await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.UserProfileId == userId);

        }

        public async Task<UserProfile> RegisterAsync(UserProfile user)
        {
            user.UserProfileId = new int();

            // hashes and salts password
            string passwordHash, passwordSalt;
            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;

            await assignmentDBContext.AddAsync(user);
            await assignmentDBContext.SaveChangesAsync();

            return user;
        }

        public async Task<bool> VerifyCurrentPasswordAsync(int userId, string currentPassword)
        {
            var user = await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.UserProfileId == userId);
            // checks if user can be found
            if (user == null)
            {
                return false;
            }

            // verifies password
            if (user.Password == null || user.PasswordSalt == null)
            {
                return false;
            }
            if (!VerifyPassword(currentPassword, user.Password, user.PasswordSalt))
            {
                return false;
            }

            return true;
        }

        private void CreatePasswordHash(string password, out string passwordHashString, out string passwordSaltString)
        {
            byte[] passwordSalt, passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // get salt
                passwordSalt = hmac.Key;
                // create hash
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // convert to string for database storage
            passwordSaltString = Convert.ToBase64String(passwordSalt);
            passwordHashString = Convert.ToBase64String(passwordHash);
        }


        private bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            // convert to bytes for comparison
            byte[] passwordHashByte = Convert.FromBase64String(passwordHash);
            byte[] passwordSaltByte = Convert.FromBase64String(passwordSalt);

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSaltByte))
            {
                // create hash using password salt
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                // loop through the byte array
                for (int i = 0; i < computedHash.Length; i++)
                {
                    // if mismatch
                    if (computedHash[i] != passwordHashByte[i])
                    {
                        return false;
                    }
                }
            }
            // if no mismatches 
            return true;
        }

        public async Task<UserProfile> GetByTokenAsync(string token)
        {
            return await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.ForgetPassword == token);
        }


    }
}
