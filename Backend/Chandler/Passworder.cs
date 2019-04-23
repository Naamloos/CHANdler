using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Chandler
{
    public static class Passworder
    {
        public static (string hash, int cycles) GenerateHash(string input, string salt, int cycles = 0)
        {
            var hmac = SHA256.Create("SHA256");
            byte[] buffer = Encoding.UTF8.GetBytes($"{salt}{input}{salt}");
            if(cycles == 0)
                cycles = new Random().Next(32767, 65535);

            for (int i = 0; i < cycles; i++)
            {
                buffer = hmac.ComputeHash(buffer);
            }

            return (BitConverter.ToString(buffer).Replace("-", ""), cycles);
        }

        public static bool CompareHash(string input, string salt, string hash, int cycles)
        {
            var inputhash = GenerateHash(input, salt, cycles);

            return inputhash.hash == hash;
        }

        public static string GenerateSalt()
        {
            byte[] output = new byte[64];

            new Random().NextBytes(output);

            return Encoding.UTF8.GetString(output);
        }
    }
}
