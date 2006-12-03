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
using System.Xml;
using System.IO;
using System.Windows.Forms;
using WikiFunctions.Browser;
using System.Text.RegularExpressions;
using System.Reflection;

namespace WikiFunctions
{
    public enum LangCodeEnum { en, ca, da, de, eo, es, fi, fr, he, hu, it, ja, nl, no, mi, pl, pt, ru, simple, sk, sl, sv, ta, tj, ur, zh }
    public enum ProjectEnum { wikipedia, wiktionary, wikisource, wikiquote, wikiversity, wikibooks, wikinews, commons, meta, species, custom }

    /// <summary>
    /// Holds static variables, to allow functionality on different wikis.
    /// </summary>
    public static class Variables
    {
        static Variables()
        {
            SetProject(LangCodeEnum.en, ProjectEnum.wikipedia);

            enLangNamespaces[-2] = "Media:";
            enLangNamespaces[-1] = "Special:";
            enLangNamespaces[1] = "Talk:";
            enLangNamespaces[2] = "User:";
            enLangNamespaces[3] = "User talk:";
            enLangNamespaces[4] = "Project:";
            enLangNamespaces[5] = "Project talk:";
            enLangNamespaces[6] = "Image:";
            enLangNamespaces[7] = "Image talk:";
            enLangNamespaces[8] = "MediaWiki:";
            enLangNamespaces[9] = "MediaWiki talk:";
            enLangNamespaces[10] = "Template:";
            enLangNamespaces[11] = "Template talk:";
            enLangNamespaces[12] = "Help:";
            enLangNamespaces[13] = "Help talk:";
            enLangNamespaces[14] = "Category:";
            enLangNamespaces[15] = "Category talk:";
        }

        public static UserProperties User = new UserProperties();

        #region project and language settings

        /// <summary>
        /// Provides access to the en namespace keys
        /// </summary>
        public static Dictionary<int, string> enLangNamespaces = new Dictionary<int, string>(20);

        /// <summary>
        /// Provides access to the namespace keys
        /// </summary>
        public static Dictionary<int, string> Namespaces = new Dictionary<int, string>(24);

        /// <summary>
        /// Provides access to the namespace keys in a form so the first letter is case insensitive e.g. [Ww]ikipedia:
        /// </summary>
        public static Dictionary<int, string> NamespacesCaseInsensitive = new Dictionary<int, string>(24);

        /// <summary>
        /// Gets a URL of the site, e.g. "http://en.wikipedia.org/w/".
        /// </summary>
        public static string URLLong
        {
            get
            {
                return URL + URLEnd;
            }
        }

        /// <summary>
        /// Full project name, e.g. "Wikimedia Commons"
        /// </summary>
        public static string ProjectName;

        static string URLEnd = "/w/";

        static string strURL = "http://en.wikipedia.org";
        /// <summary>
        /// Gets a URL of the site, e.g. "http://en.wikipedia.org".
        /// </summary>
        public static string URL
        {
            get
            {
                return strURL;
            }
            private set { strURL = value; }
        }

        static ProjectEnum strproject = ProjectEnum.wikipedia;
        /// <summary>
        /// Gets a name of the project, e.g. "wikipedia".
        /// </summary>
        public static ProjectEnum Project
        {
            get { return strproject; }
        }

        static LangCodeEnum strlangcode = LangCodeEnum.en;
        /// <summary>
        /// Gets the language code, e.g. "en".
        /// </summary>
        public static LangCodeEnum LangCode
        {
            get { return strlangcode; }
        }

        static string strcustomproject = "";
        /// <summary>
        /// Gets script path of a custom project or empty string if standard project
        /// </summary>
        public static string CustomProject
        {
            get
            {
                return strcustomproject;
            }
        }

        static string strsummarytag = " using [[Project:AWB|AWB]]";
        /// <summary>
        /// Gets the tag to add to the edit summary, e.g. " using [[Wikipedia:AutoWikiBrowser|AWB]]".
        /// </summary>
        public static string SummaryTag
        {
            get { return strsummarytag; }
        }

        public static string Stub;
        public static string SectStub;

        /// <summary>
        /// Sets different language variables, such as namespaces. Default is english Wikipedia
        /// </summary>
        /// <param name="langCode">The language code, default is en</param>
        /// <param name="projectName">The project name default is Wikipedia</param>
        public static void SetProject(LangCodeEnum langCode, ProjectEnum projectName)
        {
            SetProject(langCode, projectName, "");
        }

        /// <summary>
        /// Sets different language variables, such as namespaces. Default is english Wikipedia
        /// </summary>
        /// <param name="langCode">The language code, default is en</param>
        /// <param name="projectName">The project name default is Wikipedia</param>
        /// <param name="customProject">Script path of a custom project or ""</param>
        public static void SetProject(LangCodeEnum langCode, ProjectEnum projectName, string customProject)
        {
            Namespaces.Clear();

            strproject = projectName;
            strlangcode = langCode;
            strcustomproject = customProject;

            ProjectName = "";
            URLEnd = "/w/";

            Stub = "[Ss]tub";

            SectStub = "[Ss]ect";

            if (Project == ProjectEnum.custom)
            {
                int x = customProject.IndexOf('/');

                if (x > 0)
                {
                    URLEnd = customProject.Substring(x, customProject.Length - x);
                    customProject = customProject.Substring(0, x);
                }

                URL = "http://" + CustomProject;
            }
            else
                URL = "http://" + LangCode + "." + Project + ".org";

            if (projectName == ProjectEnum.wikipedia)
            {
                //set language variables
                switch (langCode)
                {
                    case LangCodeEnum.en:
                        SetToEnglish();
                        break;

                    case LangCodeEnum.ca:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Especial:";
                        Namespaces[1] = "Discussió:";
                        Namespaces[2] = "Usuari:";
                        Namespaces[3] = "Usuari Discussió:";
                        Namespaces[4] = "Viquipèdia:";
                        Namespaces[5] = "Viquipèdia Discussió:";
                        Namespaces[6] = "Imatge:";
                        Namespaces[7] = "Imatge Discussió:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki Discussió:";
                        Namespaces[10] = "Plantilla:";
                        Namespaces[11] = "Plantilla Discussió:";
                        Namespaces[12] = "Ajuda:";
                        Namespaces[13] = "Ajuda Discussió:";
                        Namespaces[14] = "Categoria:";
                        Namespaces[15] = "Categoria Discussió:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Portal Discussió:";
                        Namespaces[102] = "Viquiprojecte:";
                        Namespaces[103] = "Viquiprojecte Discussió:";

                        strsummarytag = " [[Viquipèdia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.da:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Speciel:";
                        Namespaces[1] = "Diskussion:";
                        Namespaces[2] = "Bruger:";
                        Namespaces[3] = "Brugerdiskussion:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipedia-diskussion:";
                        Namespaces[6] = "Billede:";
                        Namespaces[7] = "Billedediskussion:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki-diskussion:";
                        Namespaces[10] = "Skabelon:";
                        Namespaces[11] = "Skabelondiskussion:";
                        Namespaces[12] = "Hjælp:";
                        Namespaces[13] = "Hjælp-diskussion:";
                        Namespaces[14] = "Kategori:";
                        Namespaces[15] = "Kategoridiskussion:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Portal diskussion:";

                        strsummarytag = " ved brug af [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.de:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Spezial:";
                        Namespaces[1] = "Diskussion:";
                        Namespaces[2] = "Benutzer:";
                        Namespaces[3] = "Benutzer Diskussion:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipedia Diskussion:";
                        Namespaces[6] = "Bild:";
                        Namespaces[7] = "Bild Diskussion:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki Diskussion:";
                        Namespaces[10] = "Vorlage:";
                        Namespaces[11] = "Vorlage Diskussion:";
                        Namespaces[12] = "Hilfe:";
                        Namespaces[13] = "Hilfe Diskussion:";
                        Namespaces[14] = "Kategorie:";
                        Namespaces[15] = "Kategorie Diskussion:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Portal Diskussion:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.es:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Especial:";
                        Namespaces[1] = "Discusión:";
                        Namespaces[2] = "Usuario:";
                        Namespaces[3] = "Usuario Discusión:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipedia Discusión:";
                        Namespaces[6] = "Imagen:";
                        Namespaces[7] = "Imagen Discusión:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki Discusión:";
                        Namespaces[10] = "Plantilla:";
                        Namespaces[11] = "Plantilla Discusión:";
                        Namespaces[12] = "Ayuda:";
                        Namespaces[13] = "Ayuda Discusión:";
                        Namespaces[14] = "Categoría:";
                        Namespaces[15] = "Categoría Discusión:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Portal Discusión:";
                        Namespaces[102] = "Wikiproyecto:";
                        Namespaces[103] = "Wikiproyecto Discusión:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.eo:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Speciala:";
                        Namespaces[1] = "Diskuto:";
                        Namespaces[2] = "Vikipediisto:";
                        Namespaces[3] = "Vikipediista diskuto:";
                        Namespaces[4] = "Vikipedio:";
                        Namespaces[5] = "Vikipedia diskuto:";
                        Namespaces[6] = "Dosiero:";
                        Namespaces[7] = "Dosiera diskuto:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki diskuto:";
                        Namespaces[10] = "Ŝablono:";
                        Namespaces[11] = "Ŝablona diskuto:";
                        Namespaces[12] = "Helpo:";
                        Namespaces[13] = "Helpa diskuto:";
                        Namespaces[14] = "Kategorio:";
                        Namespaces[15] = "Kategoria diskuto:";
                        Namespaces[100] = "Portalo:";
                        Namespaces[101] = "Portala diskuto:";

                        strsummarytag = " [[Vikipedio:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.fi:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Toiminnot:";
                        Namespaces[1] = "Keskustelu:";
                        Namespaces[2] = "Käyttäjä:";
                        Namespaces[3] = "Keskustelu käyttäjästä:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Keskustelu Wikipediasta:";
                        Namespaces[6] = "Kuva:";
                        Namespaces[7] = "Keskustelu kuvasta:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki talk:";
                        Namespaces[10] = "Malline:";
                        Namespaces[11] = "Keskustelu mallineesta:";
                        Namespaces[12] = "Ohje:";
                        Namespaces[13] = "Keskustelu ohjeesta:";
                        Namespaces[14] = "Luokka:";
                        Namespaces[15] = "Keskustelu luokasta:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.fr:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Special:";
                        Namespaces[1] = "Discuter:";
                        Namespaces[2] = "Utilisateur:";
                        Namespaces[3] = "Discussion Utilisateur:";
                        Namespaces[4] = "Wikipédia:";
                        Namespaces[5] = "Discussion Wikipédia:";
                        Namespaces[6] = "Image:";
                        Namespaces[7] = "Discussion Image:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "Discussion MediaWiki:";
                        Namespaces[10] = "Modèle:";
                        Namespaces[11] = "Discussion Modèle:";
                        Namespaces[12] = "Aide:";
                        Namespaces[13] = "Discussion Aide:";
                        Namespaces[14] = "Catégorie:";
                        Namespaces[15] = "Discussion Catégorie:";
                        Namespaces[100] = "Portail:";
                        Namespaces[101] = "Discussion Portail:";
                        Namespaces[102] = "Projet:";
                        Namespaces[103] = "Discussion Projet:";
                        Namespaces[104] = "Référence:";
                        Namespaces[105] = "Discussion Référence:";

                        strsummarytag = " [[Wikipédia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.hu:
                        Namespaces[-2] = "Média:";
                        Namespaces[-1] = "Speciális:";
                        Namespaces[1] = "Vita:";
                        Namespaces[2] = "User:";
                        Namespaces[3] = "User vita:";
                        Namespaces[4] = "Wikipédia:";
                        Namespaces[5] = "Wikipédia vita:";
                        Namespaces[6] = "Kép:";
                        Namespaces[7] = "Kép vita:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki vita:";
                        Namespaces[10] = "Sablon:";
                        Namespaces[11] = "Sablon vita:";
                        Namespaces[12] = "Segítség:";
                        Namespaces[13] = "Segítség vita:";
                        Namespaces[14] = "Kategória:";
                        Namespaces[15] = "Kategória vita:";

                        strsummarytag = " [[Wikipédia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.it:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Speciale:";
                        Namespaces[1] = "Discussione:";
                        Namespaces[2] = "Utente:";
                        Namespaces[3] = "Discussioni utente:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Discussioni Wikipedia:";
                        Namespaces[6] = "Immagine:";
                        Namespaces[7] = "Discussioni immagine:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "Discussioni MediaWiki:";
                        Namespaces[10] = "Template:";
                        Namespaces[11] = "Discussioni template:";
                        Namespaces[12] = "Aiuto:";
                        Namespaces[13] = "Discussioni aiuto:";
                        Namespaces[14] = "Categoria:";
                        Namespaces[15] = "Discussioni categoria:";
                        Namespaces[100] = "Portale:";
                        Namespaces[101] = "Discussioni portale:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.ja:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "特別:";
                        Namespaces[1] = "ノート:";
                        Namespaces[2] = "利用者:";
                        Namespaces[3] = "利用者‐会話:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipedia‐ノート:";
                        Namespaces[6] = "画像:";
                        Namespaces[7] = "画像‐ノート:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki‐ノート:";
                        Namespaces[10] = "Template:";
                        Namespaces[11] = "Template‐ノート:";
                        Namespaces[12] = "Help:";
                        Namespaces[13] = "Help‐ノート:";
                        Namespaces[14] = "Category:";
                        Namespaces[15] = "Category‐ノート:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Portal‐ノート:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.mi:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Special:";
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

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.nl:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Speciaal:";
                        Namespaces[1] = "Overleg:";
                        Namespaces[2] = "Gebruiker:";
                        Namespaces[3] = "Overleg gebruiker:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Overleg Wikipedia:";
                        Namespaces[6] = "Afbeelding:";
                        Namespaces[7] = "Overleg afbeelding:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "Overleg MediaWiki:";
                        Namespaces[10] = "Sjabloon:";
                        Namespaces[11] = "Overleg sjabloon:";
                        Namespaces[12] = "Help:";
                        Namespaces[13] = "Overleg help:";
                        Namespaces[14] = "Categorie:";
                        Namespaces[15] = "Overleg categorie:";
                        Namespaces[100] = "Portaal:";
                        Namespaces[101] = "Overleg portaal:";

                        strsummarytag = " met [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.no:
                        Namespaces[-2] = "Medium:";
                        Namespaces[-1] = "Spesial:";
                        Namespaces[1] = "Diskusjon:";
                        Namespaces[2] = "Bruker:";
                        Namespaces[3] = "Brukerdiskusjon:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipedia-diskusjon:";
                        Namespaces[6] = "Bilde:";
                        Namespaces[7] = "Bildediskusjon:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki-diskusjon:";
                        Namespaces[10] = "Mal:";
                        Namespaces[11] = "Maldiskusjon:";
                        Namespaces[12] = "Hjelp:";
                        Namespaces[13] = "Hjelpdiskusjon:";
                        Namespaces[14] = "Kategori:";
                        Namespaces[15] = "Kategoridiskusjon:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Portaldiskusjon:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.pl:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Specjalna:";
                        Namespaces[1] = "Dyskusja:";
                        Namespaces[2] = "Wikipedysta:";
                        Namespaces[3] = "Dyskusja Wikipedysty:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Dyskusja Wikipedii:";
                        Namespaces[6] = "Grafika:";
                        Namespaces[7] = "Dyskusja grafiki:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "Dyskusja MediaWiki:";
                        Namespaces[10] = "Szablon:";
                        Namespaces[11] = "Dyskusja szablonu:";
                        Namespaces[12] = "Pomoc:";
                        Namespaces[13] = "Dyskusja pomocy:";
                        Namespaces[14] = "Kategoria:";
                        Namespaces[15] = "Dyskusja kategorii:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Dyskusja portalu:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        SectStub = "[Ss]ek";
                        break;

                    case LangCodeEnum.pt:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Especial:";
                        Namespaces[1] = "Discussão:";
                        Namespaces[2] = "Usuário:";
                        Namespaces[3] = "Usuário Discussão:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipedia Discussão:";
                        Namespaces[6] = "Imagem:";
                        Namespaces[7] = "Imagem Discussão:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki Discussão:";
                        Namespaces[10] = "Predefinição:";
                        Namespaces[11] = "Predefinição Discussão:";
                        Namespaces[12] = "Ajuda:";
                        Namespaces[13] = "Ajuda Discussão:";
                        Namespaces[14] = "Categoria:";
                        Namespaces[15] = "Categoria Discussão:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Discussão Portal:";

                        strsummarytag = " utilizando [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.ru:
                        Namespaces[-2] = "Медиа:";
                        Namespaces[-1] = "Служебная:";
                        Namespaces[1] = "Обсуждение:";
                        Namespaces[2] = "Участник:";
                        Namespaces[3] = "Обсуждение участника:";
                        Namespaces[4] = "Википедия:";
                        Namespaces[5] = "Обсуждение Википедии:";
                        Namespaces[6] = "Изображение:";
                        Namespaces[7] = "Обсуждение изображения:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "Обсуждение MediaWiki:";
                        Namespaces[10] = "Шаблон:";
                        Namespaces[11] = "Обсуждение шаблона:";
                        Namespaces[12] = "Справка:";
                        Namespaces[13] = "Обсуждение справки:";
                        Namespaces[14] = "Категория:";
                        Namespaces[15] = "Обсуждение категории:";
                        Namespaces[100] = "Портал:";
                        Namespaces[101] = "Обсуждение портала:";

                        strsummarytag = " при помощи [[Википедия:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.simple:
                        SetToEnglish("Wikipedia:", "Wikipedia talk:");
                        ProjectName = "Simple English Wikipedia";
                        break;

                    case LangCodeEnum.sk:
                        Namespaces[-2] = "Médiá:";
                        Namespaces[-1] = "Špeciálne:";
                        Namespaces[1] = "Diskusia:";
                        Namespaces[2] = "Redaktor:";
                        Namespaces[3] = "Diskusia s redaktorom:";
                        Namespaces[4] = "Wikipédia:";
                        Namespaces[5] = "Diskusia k Wikipédii:";
                        Namespaces[6] = "Obrázok:";
                        Namespaces[7] = "Diskusia k obrázku:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "Diskusia k MediaWiki:";
                        Namespaces[10] = "Šablóna:";
                        Namespaces[11] = "Diskusia k šablóne:";
                        Namespaces[12] = "Pomoc:";
                        Namespaces[13] = "Diskusia k pomoci:";
                        Namespaces[14] = "Kategória:";
                        Namespaces[15] = "Diskusia ku kategórii:";
                        Namespaces[100] = "Portál:";
                        Namespaces[101] = "Diskusia k portálu:";

                        strsummarytag = " [[Wikipédia:AutoWikiBrowser|AWB]]";
                        break;

                    case LangCodeEnum.sl:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Posebno:";
                        Namespaces[1] = "Pogovor:";
                        Namespaces[2] = "Uporabnik:";
                        Namespaces[3] = "Uporabniški pogovor:";
                        Namespaces[4] = "Wikipedija:";
                        Namespaces[5] = "Pogovor o Wikipediji:";
                        Namespaces[6] = "Slika:";
                        Namespaces[7] = "Pogovor o sliki:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "Pogovor o MediaWiki:";
                        Namespaces[10] = "Predloga:";
                        Namespaces[11] = "Pogovor o predlogi:";
                        Namespaces[12] = "Pomoč:";
                        Namespaces[13] = "Pogovor o pomoči:";
                        Namespaces[14] = "Kategorija:";
                        Namespaces[15] = "Pogovor o kategoriji:";

                        strsummarytag = " [[Wikipedija:AutoWikiBrowser|AWB]]";
                        Stub = "(?:[Ss]tub|[Šš]krbina)";
                        break;

                    case LangCodeEnum.sv:
                        Namespaces[-2] = "Media:";
                        Namespaces[-1] = "Special:";
                        Namespaces[1] = "Diskussion:";
                        Namespaces[2] = "Användare:";
                        Namespaces[3] = "Användardiskussion:";
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipediadiskussion:";
                        Namespaces[6] = "Bild:";
                        Namespaces[7] = "Bilddiskussion:";
                        Namespaces[8] = "MediaWiki:";
                        Namespaces[9] = "MediaWiki diskussion:";
                        Namespaces[10] = "Mall:";
                        Namespaces[11] = "Malldiskussion:";
                        Namespaces[12] = "Hjälp:";
                        Namespaces[13] = "Hjälp diskussion:";
                        Namespaces[14] = "Kategori:";
                        Namespaces[15] = "Kategoridiskussion:";
                        Namespaces[100] = "Portal:";
                        Namespaces[101] = "Portaldiskussion:";

                        strsummarytag = " [[Wikipedia:AutoWikiBrowser|AWB]]";
                        break;

                    default:
                        Namespaces = LoadNamespaces(URL);
                        strsummarytag = " ([[Project:AWB|AWB]])";
                        break;
                }
            }
            else if (projectName == ProjectEnum.commons)
            {
                SetToEnglish("Commons:", "Commons talk:");
                Namespaces[100] = "Creator:";
                Namespaces[101] = "Creator talk:";
                URL = "http://commons.wikimedia.org";
                ProjectName = "Wikimedia Commons";
            }
            else if (projectName == ProjectEnum.meta)
            {
                SetToEnglish("Meta:", "Meta talk:");
                Namespaces[100] = "Hilfe";
                Namespaces[101] = "Hilfe Diskussion";
                Namespaces[102] = "Aide";
                Namespaces[103] = "Discussion Aide";
                Namespaces[104] = "Hjælp";
                Namespaces[105] = "Hjælp diskussion";
                Namespaces[106] = "Helpo";
                Namespaces[107] = "Helpa diskuto";
                Namespaces[108] = "Hjälp";
                Namespaces[109] = "Hjälp diskussion";
                Namespaces[110] = "Ayuda";
                Namespaces[111] = "Ayuda Discusión";
                Namespaces[112] = "Aiuto";
                Namespaces[113] = "Discussioni aiuto";
                Namespaces[114] = "ヘルプ";
                Namespaces[115] = "ヘルプ‐ノート";
                Namespaces[116] = "NL Help";
                Namespaces[117] = "Overleg help";
                Namespaces[118] = "Pomoc";
                Namespaces[119] = "Dyskusja pomocy";
                Namespaces[120] = "Ajuda";
                Namespaces[121] = "Ajuda Discussão";
                Namespaces[122] = "CA Ajuda";
                Namespaces[123] = "CA Ajuda Discussió";
                Namespaces[124] = "Hjelp";
                Namespaces[125] = "Hjelp diskusjon";
                Namespaces[126] = "帮助";
                Namespaces[127] = "帮助 对话";
                Namespaces[128] = "Помощь";
                Namespaces[129] = "Помощь Дискуссия";
                Namespaces[130] = "Pomoč";
                Namespaces[131] = "Pogovor o pomoči";
                URL = "http://meta.wikimedia.org";
            }
            else if (projectName == ProjectEnum.species)
            {
                SetToEnglish("Wikispecies:", "Wikispecies talk:");
                URL = "http://species.wikimedia.org";
            }
            else
            {
                Namespaces = LoadNamespaces(URL);
                strsummarytag = " ([[Project:AWB|AWB]])";
            }

            NamespacesCaseInsensitive.Clear();
            foreach (KeyValuePair<int, string> k in Namespaces)
            {
                //other languages can use the english syntax
                if (langCode != LangCodeEnum.en && enLangNamespaces.ContainsKey(k.Key))
                    NamespacesCaseInsensitive.Add(k.Key, "(?:" + Tools.CaseInsensitive(k.Value) + "|" + Tools.CaseInsensitive(enLangNamespaces[k.Key]).Replace(":", " ?:") + ")");
                else
                    NamespacesCaseInsensitive.Add(k.Key, Tools.CaseInsensitive(k.Value).Replace(":", " ?:"));
            }

            WikiRegexes.MakeLangSpecificRegexes();

            if (ProjectName == "") ProjectName = Namespaces[4].TrimEnd(':');

        }

        //User:MaxSem's code
        /// <summary>
        /// Loads namespaces from query.php
        /// </summary>
        /// <param name="url">URL of directory where scripts reside, e.g. "http://en.wikipedia.org".</param>
        /// <returns>Dictionary int=>string containing namespaces.</returns>
        public static Dictionary<int, string> LoadNamespaces(string URL)
        {
            Dictionary<int, string> ns = new Dictionary<int, string>();

            do// retry loop
            {
                try
                {
                    StringReader sr = new StringReader(Tools.GetHTML(URL + "/w/api.php?action=query&meta=siteinfo&siprop=general|namespaces&format=xml"));
                    XmlTextReader xml = new XmlTextReader(sr);
                    xml.MoveToContent();

                    while (xml.Read())
                    {
                        if (xml.IsStartElement() && xml.Name == "ns")
                        {
                            if (!xml.IsEmptyElement)
                            {
                                xml.MoveToAttribute("id");
                                int id = int.Parse(xml.GetAttribute("id"));
                                ns[id] = xml.ReadString() + ":";
                            }
                        }
                    }
                    break;
                }
                catch
                {
                    if (MessageBox.Show("An error occured while loading project information from the server. " +
                        "Please make sure that your internet connection works and such combination of project/language exist." +
                        "\r\nEnter the URL in the format \"en.wikipedia.org/w/\"",
                        "Error loading namespaces", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != DialogResult.Retry)
                    {
                        SetDefaults();

                        MessageBox.Show("Defaulting to the English Wikipedia settings.", "Project options",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ns.Clear(); // in case error was caused by XML parsing
                        foreach (KeyValuePair<int, string> p in Namespaces)
                        {
                            ns.Add(p.Key, p.Value);
                        }

                        break;
                    }
                }
            } while (true);

            return ns;
        }

        private static void SetDefaults()
        {
            strproject = ProjectEnum.wikipedia;
            strlangcode = LangCodeEnum.en;
            strsummarytag = " using [[Project:AWB|AWB]]";

            Namespaces.Clear();
            SetToEnglish();
        }

        private static void SetToEnglish()
        {
            SetToEnglish("Wikipedia:", "Wikipedia talk:");
            Namespaces[100] = "Portal:";
            Namespaces[101] = "Portal talk:";
        }

        private static void SetToEnglish(string Project, string ProjectTalk)
        {
            Namespaces[-2] = "Media:";
            Namespaces[-1] = "Special:";
            Namespaces[1] = "Talk:";
            Namespaces[2] = "User:";
            Namespaces[3] = "User talk:";
            Namespaces[4] = Project;
            Namespaces[5] = ProjectTalk;
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

            strsummarytag = " using [[Project:AWB|AWB]]";
        }
    }

    public enum WikiStatusResult { Error, NotLoggedIn, NotRegistered, OldVersion, Registered }

    public class UserProperties
    {
        public UserProperties()
        {
            webBrowserLogin.ScriptErrorsSuppressed = true;
        }

        /// <summary>
        /// Occurs when user name changes
        /// </summary>
        public event EventHandler UserNameChanged;

        /// <summary>
        /// Occurs when wiki status changes
        /// </summary>
        public event EventHandler WikiStatusChanged;

        /// <summary>
        /// Occurs when bot status changes
        /// </summary>
        public event EventHandler BotStatusChanged;

        /// <summary>
        /// Occurs when admin status changes
        /// </summary>
        public event EventHandler AdminStatusChanged;

        string strName = "";
        private bool bWikiStatus = false;
        bool bIsAdmin = false;
        bool bIsBot = false;
        bool bLoggedIn = false;

        public WebControl webBrowserLogin = new WebControl();

        /// <summary>
        /// Gets the user name
        /// </summary>
        public string Name
        {
            get { return strName; }
            set
            {
                if (strName != value)
                {
                    strName = value;

                    if (UserNameChanged != null)
                        UserNameChanged(null, null);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is enabled to use the software
        /// </summary>
        public bool WikiStatus
        {
            get { return bWikiStatus; }
            set
            {
                if (bWikiStatus != value)
                {
                    bWikiStatus = value;
                    if (WikiStatusChanged != null)
                        WikiStatusChanged(null, null);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether user is an admin
        /// </summary>
        public bool IsAdmin
        {
            get { return bIsAdmin; }
            set
            {
                if (bIsAdmin != value)
                {
                    bIsAdmin = value;

                    if (AdminStatusChanged != null)
                        AdminStatusChanged(null, null);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether user is a bot
        /// </summary>
        public bool IsBot
        {
            get { return bIsBot; }
            set
            {
                if (bIsBot != value)
                {
                    bIsBot = value;

                    if (BotStatusChanged != null)
                        BotStatusChanged(null, null);
                }
            }
        }

        public bool LoggedIn
        {
            get { return bLoggedIn; }
            set
            {
                bLoggedIn = value;
                if (bLoggedIn == false)
                    WikiStatus = false;
            }
        }

        /// <summary>
        /// Checks log in status, registered and version.
        /// </summary>
        public WikiStatusResult UpdateWikiStatus()
        {
            try
            {
                string strText = String.Empty;

                //load check page
                webBrowserLogin.Navigate(Variables.URLLong + "index.php?title=Project:AutoWikiBrowser/CheckPage&action=edit");
                //wait to load
                webBrowserLogin.Wait();

                strText = webBrowserLogin.GetArticleText();

                CheckPageText = strText;

                this.Name = webBrowserLogin.UserName();

                //see if we are logged in
                LoggedIn = webBrowserLogin.GetLogInStatus();
                if (!webBrowserLogin.GetLogInStatus())
                {
                    IsBot = false;
                    IsAdmin = false;
                    WikiStatus = false;
                    return WikiStatusResult.NotLoggedIn;
                }

                //see if there is a message
                Match m = Regex.Match(strText, "<!--Message:(.*?)-->");
                if (m.Success && m.Groups[1].Value.Trim().Length > 0)
                {
                    MessageBox.Show(m.Groups[1].Value, "Automated message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //don't require approval if checkpage does not exist.
                if (strText.Length < 1)
                {
                    WikiStatus = true;
                    IsBot = true;
                    return WikiStatusResult.Registered;
                }
                else if (strText.Contains("<!--All users enabled-->"))
                {//see if all users enabled
                    this.WikiStatus = true;
                    this.IsBot = true;
                    return WikiStatusResult.Registered;
                }
                else
                {
                    if (!m.Success)
                    {
                        IsBot = false;
                        IsAdmin = false;
                        WikiStatus = false;
                        return WikiStatusResult.Error;
                    }
                    //see if this version is enabled
                    else if (!strText.Contains(Assembly.GetExecutingAssembly().GetName().Version.ToString() + " enabled"))
                    {
                        IsBot = false;
                        IsAdmin = false;
                        WikiStatus = false;
                        return WikiStatusResult.OldVersion;
                    }
                    //see if we are allowed to use this softare
                    else
                    {
                        string strBotUsers = Tools.StringBetween(strText, "<!--enabledbots-->", "<!--enabledbotsends-->");
                        string strAdmins = Tools.StringBetween(strText, "<!--adminsbegins-->", "<!--adminsends-->");

                        if (this.Name.Length > 0 && strText.Contains("* " + Variables.User.Name + "\r\n"))
                        {
                            if (strBotUsers.Contains("* " + Variables.User.Name + "\r\n"))
                            {//enable botmode
                                this.IsBot = true;
                            }
                            if (strAdmins.Contains("* " + Variables.User.Name + "\r\n"))
                            {//enable admin features
                                this.IsAdmin = true;
                            }

                            this.WikiStatus = true;

                            return WikiStatusResult.Registered;
                        }
                        else
                        {
                            IsBot = false;
                            IsAdmin = false;
                            WikiStatus = false;
                            return WikiStatusResult.NotRegistered;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IsBot = false;
                IsAdmin = false;
                WikiStatus = false;
                MessageBox.Show(e.Message);
                return WikiStatusResult.Error;
            }
        }

        string strCheckPage = "";
        public string CheckPageText
        {
            get { return strCheckPage; }
            private set { strCheckPage = value; }
        }
    }
}

        #endregion
