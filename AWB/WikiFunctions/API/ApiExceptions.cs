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
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions.API
{
    /// <summary>
    /// Base class for all API-related exceptions
    /// </summary>
    public class ApiException : Exception
    {
        ApiEdit m_Editor;

        /// <summary>
        /// The ApiEdit object that threw the exception
        /// </summary>
        public ApiEdit Editor
        { get { return m_Editor; } }

        public ApiException(ApiEdit editor, string message)
            : base(message)
        {
            m_Editor = editor;
        }

        public ApiException(ApiEdit editor, string message, Exception innerException)
            : base(message, innerException)
        {
            m_Editor = editor;
        }
    }

    /// <summary>
    /// Thrown when an API call returned an <error> tag.
    /// See http://www.mediawiki.org/wiki/API:Errors for details
    /// </summary>
    public class ApiErrorException : ApiException
    {
        string m_ErrorCode;

        /// <summary>
        /// Short error code
        /// </summary>
        public string ErrorCode
        { get { return m_ErrorCode; } }

        string m_ApiErrorMessage;

        /// <summary>
        /// Error message returned by API
        /// </summary>
        public string ApiErrorMessage
        { get { return m_ApiErrorMessage; } }

        public ApiErrorException(ApiEdit editor, string errorCode, string errorMessage)
            : base(editor, "Bot API returned the following error: '" + errorMessage + "'")
        {
            m_ErrorCode = errorCode;
            m_ApiErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Thrown when an API call returned zero-size reply. Most likely, this indicates a server internal error.
    /// </summary>
    public class ApiBlankException : ApiException
    {
        public ApiBlankException(ApiEdit editor)
            : base(editor, "The result returned by server was blank")
        {
        }
    }

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
            : base(editor, "Error parsing data returned by server." , innerException)
        {
        }
    }

    public class ApiLoginException : ApiException
    {
        string m_StatusCode;
        public string StatusCode
        { get { return m_StatusCode; } }

        public ApiLoginException(ApiEdit editor, string status)
            :base(editor, GetErrorMessage(status))
        {
            m_StatusCode = status;
        }

        //TODO:
        protected static string GetErrorMessage(string code)
        {
            return code;
        }
    }
}
