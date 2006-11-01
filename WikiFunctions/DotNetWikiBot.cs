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
