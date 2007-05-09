using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace WikiFunctions
{
    public class WikiDiff
    {
        //char __declspec(dllexport) *wikidiff2_do_diff(const char *text1, const char *text2, int num_lines_context);
        [DllImport("wikidiff2.dll")]//, EntryPoint = "?wikidiff2_do_diff@@YAPADPBD0H@Z")]
        private static unsafe extern byte* wikidiff2_do_diff(byte* text1, byte* text2, int num_lines_context);

        //void __declspec(dllexport) wikidiff2_free(void *);
        [DllImport("wikidiff2.dll")]//, EntryPoint = "?wikidiff2_free@@YAXPAX@Z")]
        private static unsafe extern void wikidiff2_free(byte* p);

        static unsafe WikiDiff()
        {
            wikidiff2_free(null);
        }

        unsafe static string FromUTF8(byte* src)
        {
            Decoder d = Encoding.UTF8.GetDecoder();

            int nSrc = 0;
            for (; nSrc < 1024 * 1024; nSrc++) if (src[nSrc] == 0) break;
            nSrc++; //count = lastindex + 1

            int nDest = d.GetCharCount(src, nSrc, true);

            char[] chBuf = new char[nDest];
            string res;
            fixed (char* chOut = chBuf)
            {

                int bytesUsed;
                int charsUsed;
                bool completed;

                d.Convert(src, nSrc, chOut, nDest, true, out bytesUsed, out charsUsed, out completed);
                res = String.Copy(new String(chOut));
            }

            return res;
        }

        static byte[] ToUTF8(string s)
        {
            Encoder e = Encoding.UTF8.GetEncoder();

            char[] chars = s.ToCharArray();

            byte[] res = new byte[e.GetByteCount(chars, 0, s.Length, true)];

            e.GetBytes(chars, 0, s.Length, res, 0, true);

            return res;
        }

        static readonly Regex TdMagick = new Regex("<td class=\"(.*)\" id=\"(.*)\"\\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static string EnhanceDiff(string diff)
        {
            return TdMagick.Replace(diff, "<td class=\"$1\" id=\"$2\" " +
                //"onmouseover='document.all.$2.style.border=\"1px solid #ccccff\";' " +
                //"onmouseout='document.all.$2.style.border=\"1px solid white\";' " +
                "onclick='window.external.DiffClicked(\"$2\");' " +
                "ondblclick='window.external.DiffDblClicked(\"$2\");'>");
        }

        public static unsafe string GetDiff(string text1, string text2, int context)
        {
            byte[] buf1;
            if (text1 == "")
                buf1 = new byte[1];
            else
                buf1 = ToUTF8(text1);
            byte[] buf2 = ToUTF8(text2);

            string res;
            fixed (byte* p1 = buf1) fixed (byte* p2 = buf2)
            {
                byte* diff = wikidiff2_do_diff(p1, p2, context);
                if (diff == null) return "";
                res = FromUTF8(diff);
                wikidiff2_free(diff);
            }

            return EnhanceDiff(res);
        }

        public static string TableHeader()
        {
            return @"<table border='0' width='98%' cellpadding='0' cellspacing='4' class='diff'>
	<tr>
		<td colspan='2' width='50%' align='center' class='diff-otitle'><strong>Current revision</strong></td>
		<td colspan='2' width='50%' align='center' class='diff-ntitle'><strong>Your text</strong></td>
	</tr>
";
        }

        public static string DiffHead()
        {
            return @"<style type='text/css'>
td{
    border: 1px solid white;
}

table.diff, td.diff-otitle, td.diff-ntitle {
	background-color: white;
    border: 1px solid gray;
}
td.diff-addedline {
	background: #cfc;
	font-size: smaller;
}
td.diff-deletedline {
	background: #ffa;
	font-size: smaller;
}
td.diff-context {
	background: #eee;
	font-size: smaller;
}
.diffchange {
	color: red;
	font-weight: bold;
	text-decoration: none;
}

td.diff-deletedline span.diffchange {
    background-color: #FFD754; color:black;
}

td.diff-addedline span.diffchange {
    background-color: #73E5A1; color:black;
}

.d{
    overflow: auto;
}
</style>";
        }

        /// <summary>
        /// Use WikiDiff.WikiDiffVersion.FileVersion to get File Version
        /// </summary>
        public static System.Diagnostics.FileVersionInfo WikiDiffVersion
        {
           get { return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Wikidiff2.dll"); }
        }
    }
}
