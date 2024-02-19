using System;

namespace WikiFunctions
{
    class UriChangedException : Exception
    {
        public string Header => "Incorrect Request Scheme?";

        public UriChangedException(string reqUri, string resUri)
            : base(ReadableMessage(reqUri, resUri))
        {
        }

        private static string ReadableMessage(string reqUri, string resUri)
        {
            string extendedMessage = reqUri == "http"
                ? "Most websites now use https://, did you accidentally select http://?"
                : "It seems this website uses http://, but you selected https://.";

            return $"Request was made to {reqUri}, but response was from {resUri}!\r\n\r\n" + extendedMessage +
                   $"\r\n\r\nTry selecting {resUri}:// when setting up the custom project and try again.";
        }
    }
}
