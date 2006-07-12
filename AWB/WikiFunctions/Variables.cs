/*
WikiFunctions
Copyright (C) 2006 Martin Richards

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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions
{
    /// <summary>
    /// Holds static variables, to allow functionality on different wikis.
    /// </summary>
    public static class Variables
    {
        #region project and language settings

        public static Dictionary<int, string> Namespaces = new Dictionary<int, string>(20);

        /// <summary>
        /// Gets a URL of the site, e.g. "http://en.wikipedia.org".
        /// </summary>
        public static string URL
        {
            get
            {
                if (Project == "commons")
                    return "http://commons.wikimedia.org/w/";
                else
                    return "http://" + LangCode + "." + Project + ".org/w/";
            }
        }

        public static string URLShort
        {
            get
            {
                if (Project == "commons")
                    return "http://commons.wikimedia.org";
                else
                    return "http://" + LangCode + "." + Project + ".org";
            }
        }

        static string strproject = "wikipedia";
        /// <summary>
        /// Gets a name of the project, e.g. "wikipedia".
        /// </summary>
        public static string Project
        {
            get { return strproject; }
        }

        static string strlangcode = "en";
        /// <summary>
        /// Gets the language code, e.g. "en".
        /// </summary>
        public static string LangCode
        {
            get { return strlangcode; }
        }




        /*


                static string strcategoryns = "Category:";
                /// <summary>
                /// Gets the category namespace, e.g. "Category:".
                /// </summary>
                public static string CategoryNS
                {
                    get { return strcategoryns; }
                }

                static string strtemplatens = "Template:";
                /// <summary>
                /// Gets the template namespace, e.g. "Template:".
                /// </summary>
                public static string TemplateNS
                {
                    get { return strtemplatens; }
                }

                static string strprojectns = "Wikipedia:";
                /// <summary>
                /// Gets the wikipedia namespace, e.g. "Wikipedia:".
                /// </summary>
                public static string ProjectNS
                {
                    get { return strprojectns; }
                }

                static string struserns = "User:";
                /// <summary>
                /// Gets the user namespace, e.g. "User:".
                /// </summary>
                public static string UserNS
                {
                    get { return struserns; }
                }

                static string strimagens = "Image:";
                /// <summary>
                /// Gets the image namespace, e.g. "Image:".
                /// </summary>
                public static string ImageNS
                {
                    get { return strimagens; }
                }

                static string strtalkns = " talk:";
                /// <summary>
                /// Gets the user talk namespace, e.g. " talk:".
                /// </summary>
                public static string TalkNS
                {
                    get { return strtalkns; }
                }

                static string strarticletalkns = "Talk:";
                /// <summary>
                /// Gets the talk namespace, e.g. "Talk:".
                /// </summary>
                public static string ArticleTalkNS
                {
                    get { return strarticletalkns; }
                }

                static string strspecialns = "Special:";
                /// <summary>
                /// Gets the special namespace, e.g. "Special:".
                /// </summary>
                public static string SpecialNS
                {
                    get { return strspecialns; }
                }

                static string strportalns = "Portal:";
                /// <summary>
                /// Gets the portal namespace, e.g. "Portal:".
                /// </summary>
                public static string PortalNS
                {
                    get { return strportalns; }
                }

                static string strmediawikins = "MediaWiki:";
                /// <summary>
                /// Gets the mediawiki namespace, e.g. "MediaWiki:".
                /// </summary>
                public static string MediaWikiNS
                {
                    get { return strmediawikins; }
                }



                */





        static string strsummarytag = " using [[Wikipedia:AutoWikiBrowser|AWB]]";
        /// <summary>
        /// Gets the tag to add to the edit summary, e.g. " using [[Wikipedia:AutoWikiBrowser|AWB]]".
        /// </summary>
        public static string SummaryTag
        {
            get { return strsummarytag; }
        }

        /// <summary>
        /// Sets different language variables, such as namespaces. Default is english Wikipedia
        /// </summary>
        /// <param name="langCode">The language code, default is en</param>
        /// <param name="projectName">The project name default is Wikipedia</param>
        public static void SetProject(string langCode, string projectName)
        {

            strproject = projectName;

            //set language variables
            switch (langCode)
            {
                case "fr": // done by Adrian Buehlmann 2006-02-28
                    strlangcode = "fr";
                    strcategoryns = "Catégorie:";
                    strtemplatens = "Modèle:";
                    strprojectns = "Wikipédia:";
                    struserns = "Utilisateur:";
                    strimagens = "Image:";
                    strtalkns = "Discussion Utilisateur:";
                    strarticletalkns = "Discuter:";
                    strspecialns = "Special:";
                    strportalns = "Portail:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                case "de": // done by Adrian Buehlmann 2006-02-28 (with help from Marc aka Genesis)
                    strlangcode = "de";
                    strcategoryns = "Kategorie:";
                    strtemplatens = "Vorlage:";
                    strprojectns = "Wikipedia:";
                    struserns = "Benutzer:";
                    strimagens = "Bild:";
                    strtalkns = " Diskussion:";
                    strarticletalkns = "Diskussion:";
                    strspecialns = "Spezial:";
                    strportalns = "Portal:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                case "sv":
                    strlangcode = "sv";
                    strcategoryns = "Kategori:";
                    strtemplatens = "Template:";
                    strprojectns = "Wikipedia:";
                    struserns = "User:";
                    strimagens = "Image:";
                    strtalkns = " talk:";
                    strarticletalkns = "Talk:";
                    strspecialns = "Special:";
                    strportalns = "Portal:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                case "ca": //by :ca:Usuari:Joanjoc (catalan wiki sysop)
                    strlangcode = "ca";
                    strcategoryns = "Categoria:";
                    strtemplatens = "Template:";
                    strprojectns = "Viquipèdia:";
                    struserns = "Usuari:";
                    strimagens = "Imatge:";
                    strtalkns = " discussió:";
                    strarticletalkns = "Discussió:";
                    strspecialns = "Especial:";
                    strportalns = "Portal:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                case "ru": // done by Max Semenik 2006-04-16 (with help from Swix)
                    strlangcode = "ru";
                    strcategoryns = "Категория:";
                    strtemplatens = "Шаблон:";
                    strprojectns = "Википедия:";
                    struserns = "Участник:";
                    strimagens = "Изображение:";
                    strtalkns = "Обсуждение:";
                    strarticletalkns = "Обсуждение:";
                    strspecialns = "Служебная:";
                    strportalns = "Портал:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                /*
         <ns id="-2">Media</ns>
  <ns id="-1">Special</ns>
  <ns id="0" />
  <ns id="1">Talk</ns>
  <ns id="2">User</ns>
  <ns id="3">User talk</ns>
  <ns id="4">Wikipedia</ns>
  <ns id="5">Wikipedia talk</ns>
  <ns id="6">Image</ns>
  <ns id="7">Image talk</ns>
  <ns id="8">MediaWiki</ns>
  <ns id="9">MediaWiki talk</ns>
  <ns id="10">Template</ns>
  <ns id="11">Template talk</ns>
  <ns id="12">Help</ns>
  <ns id="13">Help talk</ns>
  <ns id="14">Category</ns>
  <ns id="15">Category talk</ns>
  <ns id="100">Portal</ns>
  <ns id="101">Portal talk</ns>
         */

                case "en":
                    Namespaces[-2] = "Media:";
                    Namespaces[-1] = "Special:";
                    //Namespaces[0] = "";
                    Namespaces[1] = "Talk:";
                    Namespaces[2] = "User:";
                    Namespaces[3] = "User talk:";
                    Namespaces[4] = "Wikipedia:";
                    Namespaces[5] = "Wikipedia talk:";
                    Namespaces[6] = "Image:";
                    Namespaces[7] = "Image talk:";
                    Namespaces[8] = "MediaWiki:";
                    Namespaces[9] = "MediaWiki talk:";
                    Namespaces[10] = "Template:";
                    Namespaces[11] = "Template talk:";
                    Namespaces[12] = "Help:";
                    Namespaces[13] = "Help talk:";
                    Namespaces[14] = "Category:";
                    Namespaces[15] = "Category talk:";
                    Namespaces[100] = "Portal:";
                    Namespaces[101] = "Portal talk:";

                    strlangcode = "en";
                    strsummarytag = " using [[Wikipedia:AutoWikiBrowser|AWB]]";

                    break;

                case "mi":
                    strlangcode = "mi";
                    strcategoryns = "Category:";
                    strtemplatens = "Template:";
                    strprojectns = Tools.TurnFirstToUpper(projectName) + ":";
                    struserns = "User:";
                    strimagens = "Image:";
                    strtalkns = " talk:";
                    strarticletalkns = "Talk:";
                    strspecialns = "Special:";
                    strportalns = "Portal:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = "";
                    break;

                case "sl":
                    strlangcode = "sl";
                    strcategoryns = "Kategorija:";
                    strtemplatens = "Predloga:";
                    strprojectns = "Wikipedija:";
                    struserns = "Uporabnik:";
                    strimagens = "Slika:";
                    strtalkns = " pogovor:";
                    strarticletalkns = "Pogovor:";
                    strspecialns = "Posebno:";
                    strportalns = "Portal:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                case "nl":
                    strlangcode = "nl";
                    strcategoryns = "Categorie:";
                    strtemplatens = "Sjabloon:";
                    strprojectns = "Wikipedia:";
                    struserns = "Gebruiker:";
                    strimagens = "Afbeelding:";
                    strtalkns = " overleg:";
                    strarticletalkns = "Overleg:";
                    strspecialns = "Speciaal:";
                    strportalns = "Portaal:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " met [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                case "pt":
                    strlangcode = "pt";
                    strcategoryns = "Categoria:";
                    strtemplatens = "Predefinição:";
                    strprojectns = "Wikipedia:";
                    struserns = "Usuário:";
                    strimagens = "Imagem:";
                    strtalkns = " discussão:";
                    strarticletalkns = "Discussão:";
                    strspecialns = "Especial:";
                    strportalns = "Portal:";
                    strmediawikins = "MediaWiki:";
                    strsummarytag = " utilizando [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;
            }
        }
    }
}
        #endregion
