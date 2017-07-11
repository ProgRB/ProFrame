using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProFrame
{
    public class ProtectString
    {
        private static byte[] s_entropy = { 8, 7, 15, 15 };
        public static byte[] Protect(string stringData)
        {
            try
            {
                return ProtectedData.Protect(System.Text.Encoding.Unicode.GetBytes(stringData), s_entropy, DataProtectionScope.LocalMachine);
            }
            catch { };
            return null;
        }
        public static string Unprotect(byte[] data)
        {
            try
            {
                return System.Text.Encoding.Unicode.GetString(ProtectedData.Unprotect(data, s_entropy, DataProtectionScope.LocalMachine));
            }
            catch { };
            return null;
        }
    }
}
