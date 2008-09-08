/////////////////////////////////////////////////////////////////////////////////////////////////
/// DON'T MODIFY SvnInfo.cs, it is automatically generated from SvnInfo.template.cs, which you 
/// should edit. This system is used for embedding the SVN revision number into the binaries.
/// For details, see http://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-subwcrev.html
/// It relies on TortoiseSVN to be installed at default location, "C:\Program Files\TortoiseSVN"
/// If it's not, create a batch file C:\Program Files\TortoiseSVN\bin\SubWCRev.bat,
/// consisting of single line:
/// path_to_SubWCRev.exe_on_your_system %1 %2 %3 %4 %5 %6 %7 %8 %9
/// Otherwise, create a Directory Junction/similar - http://www.howtogeek.com/howto/windows-vista/using-symlinks-in-windows-vista/
//////////////////////////////////////////////////////////////////////////////////////////////////
using System;

namespace WikiFunctions
{
    public static partial class Variables
    {
        private const string m_Revision = "$WCREV$ ($WCDATE$)";
    }
}
