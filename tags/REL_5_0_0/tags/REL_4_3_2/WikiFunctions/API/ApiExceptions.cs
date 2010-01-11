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
    public class ApiException : Exception
    {
        ApiEdit m_Editor;
        public ApiEdit Editor
        { get { return m_Editor; } }

        public ApiException(ApiEdit editor, string message)
            : base(message)
        {
            m_Editor = editor;
        }
    }

    public class ApiNotSupportedException
    {
    }
}
