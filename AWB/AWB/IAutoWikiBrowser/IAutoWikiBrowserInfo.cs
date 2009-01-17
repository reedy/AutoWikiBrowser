/*
Copyright (C) 2008 Stephen Kennedy <steve@sdk-software.com>

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
using WikiFunctions;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        Version IAutoWikiBrowserInfo.AWBVersion { get { return Program.Version; } }
        Version IAutoWikiBrowserInfo.WikiFunctionsVersion { get { return Tools.Version; } }
        string IAutoWikiBrowserInfo.AWBVersionString { get { return Program.VersionString; } }
        string IAutoWikiBrowserInfo.WikiFunctionsVersionString { get { return Tools.VersionString; } }
        string IAutoWikiBrowserInfo.WikiDiffVersionString { get { return "(internal)"; } }
        LangCodeEnum IAutoWikiBrowserInfo.LangCode { get { return Variables.LangCode; } }
        ProjectEnum IAutoWikiBrowserInfo.Project { get { return Variables.Project; } }

        bool IAutoWikiBrowserInfo.Privacy { get { return Properties.Settings.Default.Privacy; } }
        bool IAutoWikiBrowserInfo.Shutdown { get { return Shutdown; } }
    }
}
