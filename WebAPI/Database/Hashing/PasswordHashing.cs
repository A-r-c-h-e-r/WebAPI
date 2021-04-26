using System;
using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Database.Hashing
{
    public static class PasswordHashing
    {
        public static string GetPasswordHashing(string password)
        {
            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}