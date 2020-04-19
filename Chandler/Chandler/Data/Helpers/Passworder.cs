using BCrypt.Net;

namespace Chandler.Data
{
    /// <summary>
    /// Static class for hashing passwords
    /// </summary>
    public static class Passworder
    {
        /// <summary>
        /// Generate a hash
        /// </summary>
        /// <param name="input">Text Input</param>
        /// <param name="pepper">Unique string known only to the server</param>
        /// <returns>Generated Hash</returns>
        public static (string Hash, string Salt) GenerateHash(string input, string pepper)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var hash = BCrypt.Net.BCrypt.EnhancedHashPassword($"{salt}{pepper}{input}", 15, HashType.SHA512);
            return (hash, salt);
        }

        /// <summary>
        /// Compares text input to the given hash
        /// </summary>
        /// <param name="input">Text Input</param>
        /// <param name="salt">Hash Salt</param>
        /// <param name="pepper">Unique string known only to the server</param>
        /// <param name="hash">Hash to compare to</param>
        /// <returns>True, if text matches hash</returns>
        public static bool VerifyPassword(string input, string hash, string salt, string pepper)
            => BCrypt.Net.BCrypt.EnhancedVerify($"{salt}{pepper}{input}", hash, HashType.SHA512);
    }
}
