/*
Copyright (C) 2008 Max Semenik

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
using System;

namespace WikiFunctions.API
{
    /// <summary>
    /// Base class for all API-related exceptions
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// The ApiEdit object that threw the exception
        /// </summary>
        public ApiEdit Editor
        { get; private set; }

        public ApiException(ApiEdit editor, string message)
            : base(message)
        {
            Editor = editor;
        }

        public ApiException(ApiEdit editor, string message, Exception innerException)
            : base(message, innerException)
        {
            Editor = editor;
        }
    }

    /// <summary>
    /// Thrown when Abort() function is called, or request is otherwise abruptly terminated
    /// </summary>
    public class ApiAbortedException : ApiException
    {
        public ApiAbortedException(ApiEdit editor)
            : base(editor, "API operation aborted")
        {
        }
    };

    /// <summary>
    /// Thrown when an API call returned an &lt;error> tag.
    /// See http://www.mediawiki.org/wiki/API:Errors for details
    /// </summary>
    public class ApiErrorException : ApiException
    {
        /// <summary>
        /// Short error code
        /// </summary>
        public string ErrorCode
        { get; private set; }

        /// <summary>
        /// Error message returned by API
        /// </summary>
        public string ApiErrorMessage
        { get; private set; }

        public ApiErrorException(ApiEdit editor, string errorCode, string errorMessage)
            : base(editor, "Bot API returned the following error: '" + errorMessage + "'")
        {
            ErrorCode = errorCode;
            ApiErrorMessage = errorMessage;
        }
    }


    /// <summary>
    /// Thrown when an operation is ended with result other than "Success"
    /// </summary>
    public class ApiOperationFailedException : ApiException
    {
        public ApiOperationFailedException(ApiEdit editor, string action, string result)
            : base(editor, "Operation '" + action + "' ended with result '" + result + "'.")
        {
            Action = action;
            Result = result;
        }

        public readonly string Action;
        public readonly string Result;
    };

    /// <summary>
    /// Thrown when an API call returns a zero-size reply. Most likely, this indicates a server internal error.
    /// </summary>
    public class ApiBlankException : ApiException
    {
        public ApiBlankException(ApiEdit editor)
            : base(editor, "The result returned by server was blank")
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApiBrokenXmlException : ApiException
    {
        public ApiBrokenXmlException(ApiEdit editor, string message)
            : base(editor, message)
        {
        }

        public ApiBrokenXmlException(ApiEdit editor, string message, Exception innerException)
            : base(editor, message, innerException)
        {
        }
        public ApiBrokenXmlException(ApiEdit editor, Exception innerException)
            : base(editor, "Error parsing data returned by server: " + innerException.Message , innerException)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApiLoginException : ApiException
    {
        public string StatusCode { get; private set; }

        public ApiLoginException(ApiEdit editor, string status)
            : base(editor, GetErrorMessage(status))
        {
            StatusCode = status;
        }

        protected static string GetErrorMessage(string code)
        {
            switch (code.ToLower())
            {
                case "noname":
                    return "You didn't set the lgname parameter";
                case "illegal":
                    return "You provided an illegal username";
                case "notexists":
                    return "The username you provided doesn't exist";
                case "emptypass":
                    return "You didn't set the lgpassword parameter or you left it empty";
                case "wrongpass":
                    return "The password you provided is incorrect";
                case "wrongpluginpass":
                    return
                        "The password you provided is incorrect. (an authentication plugin rather than MediaWiki itself rejected the password)";
                case "createblocked":
                    return
                        "The wiki tried to automatically create a new account for you, but your IP address has been blocked from account creation";
                case "throttled":
                    return "You've logged in too many times in a short time."; //see http://www.mediawiki.org/wiki/API:Login#Throttling
                default:
                    return code;
            }
        }
    }

    /// <summary>
    /// Thrown when servers refuse to perform operation due to overloading
    /// </summary>
    /// <remarks>http://www.mediawiki.org/wiki/Manual:Maxlag_parameter</remarks>
    public class ApiMaxlagException : ApiErrorException
    {
        public int Maxlag
        { get; private set; }

        public int RetryAfter
        { get; private set; }

        public ApiMaxlagException(ApiEdit editor, int maxlag, int retryAfter)
            : base(editor, "maxlag", "Maxlag exceeded by " + maxlag + " seconds, retry in " + retryAfter + " seconds")
        {
            Maxlag = maxlag;
            RetryAfter = retryAfter;
        }
    }

    /// <summary>
    /// Thrown when assertion in API call fails
    /// </summary>
    public class ApiAssertionException : ApiException
    {
        public ApiAssertionException(ApiEdit editor, string assertion)
            : base(editor, "Assertion '" + assertion + "' failed")
        {
        }
    }

    /// <summary>
    /// Thrown when an error occurs during asynchronous API operations
    /// </summary>
    public class ApiInvokeException : Exception
    {
        public ApiInvokeException(string message)
            : base(message)
        {
        }

        public ApiInvokeException(Exception innerException)
            : this("There was a problem with an asynchronous API call", innerException)
        {
        }

        public ApiInvokeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown when edit is blocked by SpamBlacklist extension
    /// </summary>
    public class ApiSpamlistException : ApiException
    {
        /// <summary>
        /// URL which triggered the blacklist
        /// </summary>
        public string URL
        { get; private set; }

        public ApiSpamlistException(ApiEdit editor, string url)
            : base(editor, "The link '" + url + "' is blocked by spam blacklist")
        {
            URL = url;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApiLoggedOffException : ApiException
    {
        public ApiLoggedOffException(ApiEdit editor)
            : base(editor, "You are currently logged off")
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApiCaptchaException : ApiException
    {
        public ApiCaptchaException(ApiEdit editor)
            : base(editor, "Captcha required")
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApiInterwikiException : ApiException
    {
        public ApiInterwikiException(ApiEdit editor)
            : base(editor, "Page title contains interwiki")
        {
        }
    }
}
