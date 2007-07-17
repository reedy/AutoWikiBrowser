/*

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
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace WikiFunctions
{
    public class DotNetWikiBot
    {
        public DotNetWikiBot(string URL, string User, string Password)
        {
            LoadDotNetWikiBotLibrary();
            Loggin(URL, User, Password);
        }

        //See http://sourceforge.net/projects/dotnetwikibot/        
        object site;
        Type pageT;
        Type siteT;

        private void LoadDotNetWikiBotLibrary()
        {
            try
            {
                if (!File.Exists("DotNetWikiBot.dll"))
                    throw new Exception("DotNetWikiBot.dll not found.");

                Assembly asm = Assembly.Load("DotNetWikiBot");
                
                siteT = asm.GetType("DotNetWikiBot.Site");
                pageT = asm.GetType("DotNetWikiBot.Page");                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Loggin(string URL, string User, string Password)
        {
            try
            {
                site = Activator.CreateInstance(siteT, URL, User, Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private object MakePage(string Title)
        {
            object page;
            page = Activator.CreateInstance(pageT, site, Title);

            return page;
        }

        public void MovePage(string ArticleTitle, string NewTitle, string Summary)
        {
            object page = MakePage(ArticleTitle);
            MethodInfo mi;

            object[] paramArray = new object[2];
            paramArray[0] = NewTitle;
            paramArray[1] = "Test";
            mi = pageT.GetMethod("RenameTo");
            mi.Invoke(page, paramArray);
        }

        public void DeletePage(string ArticleTitle, string Summary)
        {
            object page = MakePage(ArticleTitle);
            MethodInfo mi;

            object[] paramArray = new object[1];
            paramArray[0] = Summary;

            mi = pageT.GetMethod("Delete");
            mi.Invoke(page, paramArray);
        }
    
    }
}
