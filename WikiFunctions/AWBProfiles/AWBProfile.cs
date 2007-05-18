using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions.AWBProfiles
{
    struct Profile
    {
        public string username, password, defaultsettings, notes;
    }

    class AWBProfile
    {
        private const string RegKey = "Software\\AutoWikiBrowser\\Profiles";
        private const string PassPhrase = "";
        private const string Salt = "";
        private const string IV16Chars = "";

        protected string mUsername;
        private string mPassword; // or store in RAM encrypted

        public string Username
        {
            //get { }
            set { }
        }

        public string Password
        {
            set { }
        }

        public string Encrypt(string text)
        {
            return Encryption.RijndaelSimple.Encrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
        }

        public string Decrypt(string text)
        {
            return Encryption.RijndaelSimple.Decrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
        }

        public List<Profile> GetProfiles()
        {
            List<Profile> profiles = new List<Profile>();

            return profiles;
        }

        public void SaveProfile(Profile profile)
        {
        }
    }
}
