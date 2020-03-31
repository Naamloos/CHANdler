using System;
using System.Security.Cryptography;
using System.Text;

namespace Chandler
{
    /// <summary>
    /// Static class for hashing passwords
    /// </summary>
    public static class Passworder
    {
        private static Random Rand { get; set; } = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Generate a hash
        /// </summary>
        /// <param name="input">Text Input</param>
        /// <param name="salt">Hash Salt</param>
        /// <param name="cycles">Number of cycles to go through</param>
        /// <returns>Generated Hash</returns>
        public static (string hash, int cycles) GenerateHash(string input, string salt, int cycles = 0)
        {
            var hmac = SHA512.Create("SHA512");
            var buffer = Encoding.UTF8.GetBytes($"{salt}{input}{salt}");
            if(cycles == 0) cycles = new Random().Next(32767, 568372);
            for (int i = 0; i < cycles; i++) buffer = hmac.ComputeHash(buffer);
            var bitval = BitConverter.ToString(buffer).Replace("-", "");
            return (bitval, cycles);
        }

        /// <summary>
        /// Compares text input to the given hash
        /// </summary>
        /// <param name="input">Text Input</param>
        /// <param name="salt">Hash Salt</param>
        /// <param name="cycles">Hash Cycles</param>
        /// <param name="hash">Hash to compare to</param>
        /// <returns>True, if text matches hash</returns>
        public static bool HashAndCompare(string input, string salt, int cycles, string hash) 
            => GenerateHash(input, salt, cycles).hash == hash;

        /// <summary>
        /// Generate a salt for hashing
        /// </summary>
        /// <returns>Salt</returns>
        public static string GenerateSalt()
        {
            var output = new byte[Rand.Next(64, 255)];
            Rand.NextBytes(output);
            return Encoding.UTF8.GetString(output);
        }
    }
}
