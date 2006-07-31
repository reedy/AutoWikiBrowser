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
    public enum LangCodeEnum { en, ca, de, es, fr, it, nl, mi, pl, pt, sv, ru, sl }
    public enum ProjectEnum { wikipedia, wiktionary, wikisource, commons, meta, species }

    /// <summary>
    /// Holds static variables, to allow functionality on different wikis.
    /// </summary>
    public static class Variables
    {
        #region project and language settings

        /// <summary>
        /// Provides access to the namespace keys
        /// </summary>
        public static Dictionary<int, string> Namespaces = new Dictionary<int, string>(24);
        
        /// <summary>
        /// Gets a URL of the site, e.g. "http://en.wikipedia.org".
        /// </summary>
        public static string URL
        {
            get
            {
                if (Project == "commons")
                    return "http://commons.wikimedia.org/w/";
                else if (Project == "meta")
                    return "http://meta.wikimedia.org/w/";
                else if (Project == "species")
                    return "http://species.wikimedia.org/w/";
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
                else if (Project == "meta")
                    return "http://meta.wikimedia.org";
                else if (Project == "species")
                    return "http://species.wikimedia.org";
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

        static string strtalkns = " talk:";
        /// <summary>
        /// Gets the user talk namespace, e.g. " talk:".
        /// </summary>
        /// 
        public static string TalkNS
        {
            get { return strtalkns; }
        }
        
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
        public static void SetProject(LangCodeEnum langCode, ProjectEnum projectName)
        {
            strproject = projectName.ToString();

            //set language variables
            switch (langCode)
            {
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

                    strlangcode = "ca";
                    strtalkns = " discussió:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "de";
                    strtalkns = " Diskussion:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;

                case LangCodeEnum.en:
                    Namespaces[-2] = "Media:";
                    Namespaces[-1] = "Special:";
                    Namespaces[1] = "Talk:";
                    Namespaces[2] = "User:";
                    Namespaces[3] = "User talk:";

                    if (strproject == "wikisource")
                    {
                        Namespaces[4] = "Wikisource:";
                        Namespaces[5] = "Wikisource talk:";
                    }
                    else if (strproject == "meta")
                    {
                        Namespaces[4] = "Meta:";
                        Namespaces[5] = "Meta talk:";
                    }
                    else if (strproject == "species")
                    {
                        Namespaces[4] = "Wikispecies:";
                        Namespaces[5] = "Wikispecies talk:";
                    }
                    else if (strproject == "commons")
                    {
                        Namespaces[4] = "Commons:";
                        Namespaces[5] = "Commons talk:";
                    }
                    else if (strproject == "wiktionary")
                    {
                        Namespaces[4] = "Wiktionary:";
                        Namespaces[5] = "Wiktionary talk:";
                    }
                    else
                    {
                        Namespaces[4] = "Wikipedia:";
                        Namespaces[5] = "Wikipedia talk:";
                    }

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
                    strtalkns = " talk:";
                    strsummarytag = " using [[WP:AWB|AWB]]";
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

                    strlangcode = "es";
                    strtalkns = " Discusión:";
                    strsummarytag = " [[en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "fr";
                    //strtalkns = "Discussion Utilisateur:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "it";
                    //strtalkns = "Discussion Utilisateur:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "mi";
                    strtalkns = " talk:";
                    strsummarytag = "";
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

                    strlangcode = "nl";
                    strtalkns = " overleg:";
                    strsummarytag = " met [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "pl";
                    //strtalkns = "";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "pt";
                    strtalkns = " discussão:";
                    strsummarytag = " utilizando [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "ru";
                    strtalkns = "Обсуждение:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "sl";
                    strtalkns = " pogovor:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
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

                    strlangcode = "sv";
                    strtalkns = " talk:";
                    strsummarytag = " [[:en:Wikipedia:AutoWikiBrowser|AWB]]";
                    break;                
            }
        }
    }
}
        #endregion
