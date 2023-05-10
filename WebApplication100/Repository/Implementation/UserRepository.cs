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

        /// <summary>
        ///  Asynchronously adds a password reset token and creation date to the specified user's account in the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<UserProfile> AddChangePassTokenAsync(UserProfile user, string token, DateTime date)
        {
            // Set the user's 'ForgetPassword' property to the specified token
            user.ForgetPassword = token;
            user.ForgetCreatedAt = date;

            // Save the changes to the database 
            await assignmentDBContext.SaveChangesAsync();

            // Return the updated 'UserProfile' object
            return user;
        }

        /// <summary>
        /// Authenticates a user by checking if their email and password match those stored in the database
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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

            // Verify if the provided password matches the user's stored password
            if (!VerifyPassword(password, user.Password, user.PasswordSalt))
            {
                return null;
            }
            // Set the user's password and password salt properties to null for security reasons
            user.Password = null;
            user.PasswordSalt = null;

            // Return the authenticated user object
            return user;
        }

        /// <summary>
        /// Changes the password for the specfied user in the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<UserProfile> ChangePassAsync(int? userId, string newPassword)
        {
            // checks if the user id has the correct current password
            var user = await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.UserProfileId == userId);

            if (user == null)
            {
                return null;
            }

            // Hash and salt the new password using the 'CreatePasswordHash' method
            string passwordHash, passwordSalt;
            CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            // Set the user's password and password salt properties to the newly hashed and salted values
            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;
            // Save the changes to the database
            await assignmentDBContext.SaveChangesAsync();

            // Set the user's password, password salt, and forget password properties to null for security reasons
            user.Password = null;
            user.PasswordSalt = null;
            user.ForgetPassword = null;

            // Return the updated 'UserProfile' object
            return user;

        }

        /// <summary>
        /// Resets the specified user's password in the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<UserProfile> ForgetPasswordAsync(UserProfile user, string password)
        {
            // Hash and salt the new password using the 'CreatePasswordHash' method
            string passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);


            // Set the user's password and password salt properties to the newly hashed and salted values
            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Save the changes 
            await assignmentDBContext.SaveChangesAsync();

            // Set the user's password, password salt, and forget password properties to null for security reasons
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

        /// <summary>
        /// Method used to add a new user to the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserProfile> RegisterAsync(UserProfile user)
        {
            user.UserProfileId = new int();

            // hashes and salts password
            string passwordHash, passwordSalt;
            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);


            // Set the user's password and password salt properties to the newly hashed and salted values
            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;

            // add the new user
            await assignmentDBContext.AddAsync(user);

            // Save the changes 
            await assignmentDBContext.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Verify if the specified user's current password matches the password stored in the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPassword"></param>
        /// <returns></returns>
        public async Task<bool> VerifyCurrentPasswordAsync(int userId, string currentPassword)
        {
            // Retrieve the user with the specified user ID from the database
            var user = await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.UserProfileId == userId);
            // checks if user can be found
            if (user == null)
            {
                return false;
            }

            // Verify if the user's password and password salt properties are not null, and if the provided current password matches the user's stored password
            if (user.Password == null || user.PasswordSalt == null)
            {
                return false;
            }
            if (!VerifyPassword(currentPassword, user.Password, user.PasswordSalt))
            {
                return false;
            }

            // Return true if the verification is successful
            return true;
        }



        /// <summary>
        /// Generates a salted and hashed password using the provided password and outputs the hashed password and salt as strings
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHashString"></param>
        /// <param name="passwordSaltString"></param>
        private void CreatePasswordHash(string password, out string passwordHashString, out string passwordSaltString)
        {
            byte[] passwordSalt, passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // Generate a random salt using the HMACSHA512 algorithm's key
                passwordSalt = hmac.Key;

                // Compute the hash of the provided password using the HMACSHA512 algorithm and the salt
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // convert password salt and hash byte for database storage
            passwordSaltString = Convert.ToBase64String(passwordSalt);
            passwordHashString = Convert.ToBase64String(passwordHash);
        }


        /// <summary>
        ///  Verifies if the provided password matches the specified hashed and salted password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        private bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            // convert to bytes for comparison
            byte[] passwordHashByte = Convert.FromBase64String(passwordHash);
            byte[] passwordSaltByte = Convert.FromBase64String(passwordSalt);

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSaltByte))
            {
                // Compute the hash of the provided password using the HMACSHA512 algorithm and the salt
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));


                // Loop through the byte array to compare the computed hash with the stored hash
                for (int i = 0; i < computedHash.Length; i++)
                {
                    // if mismatch
                    if (computedHash[i] != passwordHashByte[i])
                    {
                        return false;
                    }
                }
            }
            // If no mismatches, return true
            return true;
        }

        public async Task<UserProfile> GetByTokenAsync(string token)
        {
            return await assignmentDBContext.UserProfiles.FirstOrDefaultAsync(x => x.ForgetPassword == token);
        }


    }
}
