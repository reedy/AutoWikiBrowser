/*
Autowikibrowser
Copyright (C) 2007 Mets501, Stephen Kennedy

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

using WikiFunctions;

namespace AutoWikiBrowser
{
    internal sealed partial class Help : WikiFunctions.Controls.Help
    {
        public Help()
        {
            Text = "AWB Help";
        }

        protected override string URL
        {
            get { return Tools.GetENLinkWithSimpleSkinAndLocalLanguage("Wikipedia:AutoWikiBrowser/User_manual"); }
        }

        public void ShowHelpEN(string article)
        { ShowHelp( Tools.GetENLinkWithSimpleSkinAndLocalLanguage(article)); }

        public void ShowHelp(string url)
        {
            ShowDialog();
            if (string.IsNullOrEmpty(url))
                Navigate();
            else
                Navigate(url);
        }

        private void Navigate()
        { Navigate(URL); }
    }
}

