﻿using System.Security.Cryptography;
using System.Text;

namespace dz_asp_mvc_db.Classes
{
    public static class HashClass
    {
        public static string ToSHA256(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
