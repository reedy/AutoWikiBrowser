/*
Copyright (C) 2008 Sam Reed, Stephen Kennedy

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions
{
    public static class RegistryUtils
    {
        private const string KeyPrefix = "Software\\AutoWikiBrowser\\";
        private static Computer myComputer = new Computer();

        public static string GetAWBCurrentUserSubKey(string keyNameSuffix, object defaultValue)
        {
            return myComputer.Registry.CurrentUser.GetValue(keyNameSuffix, defaultValue).ToString();
        }
    }

    namespace Encryption
    {
        public class EncryptionUtils
        {
            internal readonly string IV16Chars;
            internal readonly string PassPhrase;
            internal readonly string Salt;

            public EncryptionUtils(string InitVector, string PassPhrase, string Salt)
            {
                this.IV16Chars = InitVector;
                this.PassPhrase = PassPhrase;
                this.Salt = Salt;
            }

            /// <summary>
            /// Encrypts a string using the specified Pass Key and Salt
            /// </summary>
            /// <param name="text">String to be encrypted</param>
            /// <returns>Encrypted String</returns>
            public string Encrypt(string text)
            {
                try
                {
                    if (!string.IsNullOrEmpty(text))
                        return RijndaelSimple.Encrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
                    else
                        return text;
                }
                catch { return text; }
            }

            /// <summary>
            /// Decrypts a string
            /// </summary>
            /// <param name="text">String to be decrypted</param>
            /// <returns>Decrypted String</returns>
            public string Decrypt(string text)
            {
                try
                {
                    if (!string.IsNullOrEmpty(text))
                        return Encryption.RijndaelSimple.Decrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
                    else
                        return text;
                }
                catch { return text; }
            }

            public string DecryptAWBCurrentUserSubKey(string keyNameSuffix, object defaultValue)
            { return Decrypt(RegistryUtils.GetAWBCurrentUserSubKey(keyNameSuffix, defaultValue)); }
        }
    }
}
