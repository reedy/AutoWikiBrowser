/*
Autowikibrowser
Copyright (C) 2006 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Xml.Serialization;
using WikiFunctions.Logging;
using System.Text.RegularExpressions;
using WikiFunctions.Plugin;
using WikiFunctions.Options;
using WikiFunctions.Parse;

namespace WikiFunctions
{
    public class Article : ProcessArticleEventArgs
    {
        protected int mNameSpaceKey;
        protected string mName;
        protected string mEditSummary = "";
        protected string mSavedSummary = "";
        protected AWBLogListener mAWBLogListener;
        protected string mArticleText = "";
        protected string mOriginalArticleText = "";
        protected string mPluginEditSummary;
        protected bool mPluginSkip;

        public Article()
        { }
        
        public Article(string mName)
        {
            this.mName = mName;
            this.mNameSpaceKey = Tools.CalculateNS(mName);
            this.EditSummary = "";
        }

        public Article(string mName, int mNameSpaceKey)
        {
            this.mName = mName;
            this.mNameSpaceKey = mNameSpaceKey;
            this.EditSummary = "";
        }

        public AWBLogListener InitialiseLogListener()
        {
            mAWBLogListener = new Logging.AWBLogListener(this.mName);
            return mAWBLogListener;
        }


        [XmlIgnore]
        public AWBLogListener LogListener
        { get { return mAWBLogListener; } set { mAWBLogListener = value; } }

        public string URLEncodedName
        {
            get { return Tools.WikiEncode(mName); }
        }

        public string Name
        { get { return mName; } set { mName = value; } }
        
        [XmlIgnore]
        public string ArticleText
        { get { return mArticleText.Trim(); } } // deliberately readonly; set using methods

        [XmlIgnore]
        public string OriginalArticleText
        { get { return mOriginalArticleText.Trim(); } set { mOriginalArticleText = value; mArticleText = value; } }

        [XmlIgnore]
        public string EditSummary
        { get { return mEditSummary; } set { mEditSummary = value; } }

        [XmlIgnore]
        public string SavedSummary
        { get { return mSavedSummary; } }

        public void SaveSummary()
        { mSavedSummary = 
            mEditSummary; // EditSummary gets reset by MainForm.txtEdit_TextChanged before it's used, I don't know why
        }

        public bool IsStub()
        {
            if (mArticleText.Contains("stub}}") | mArticleText.Length < 1500)
            { return true; }
            else
            { return false; }
        }

        public bool IsInUse()
        { return Regex.IsMatch(mArticleText, "\\{\\{[Ii]nuse"); }

        public bool SkipIfContains(string strFind, bool Regexe, bool caseSensitive, bool DoesContain)
        {
            if (strFind.Length > 0)
            {
                RegexOptions RegOptions;

                if (caseSensitive)
                    RegOptions = RegexOptions.None;
                else
                    RegOptions = RegexOptions.IgnoreCase;

                strFind = Tools.ApplyKeyWords(this.Name, strFind);

                if (!Regexe)
                    strFind = Regex.Escape(strFind);

                if (Regex.IsMatch(this.OriginalArticleText, strFind, RegOptions))
                    return DoesContain;
                else
                    return !DoesContain;
            }
            else
                return false;
        }

        public void AWBSkip(string reason)
        { mAWBLogListener.AWBSkipped(reason); }

        public bool SkipArticle
        { get { return mAWBLogListener.Skipped; } }

        public void ChangeArticleText(string changedBy, string reason, string newText, bool checkIfChanged)
        {
            if (checkIfChanged && newText == mArticleText) return;

            mArticleText = newText;
            mAWBLogListener.WriteLine(reason, changedBy);  
        }

        public void AWBChangeArticleText(string reason, string newText, bool checkIfChanged)
        {
            ChangeArticleText("AWB", reason, newText, checkIfChanged);
        }

        public void PluginChangeArticleText(string newText)
        // plugins should set their own log entry using the object passed in ProcessArticle()
        {
            mArticleText = newText;
        }

        public void AppendPluginEditSummary()
        {
            if (mPluginEditSummary.Length > 0)
            {
                EditSummary += " " + mPluginEditSummary.Trim();
                mPluginEditSummary = "";
            }
        }

        public void SendPageToPlugin(IAWBPlugin plugin, IAutoWikiBrowser sender)
        {
            string strTemp = plugin.ProcessArticle(sender, this);

            if (!mPluginSkip)
            {
                this.PluginChangeArticleText(strTemp);
                this.AppendPluginEditSummary();
            }
        }

        public void Unicodify(bool SkipNoUnicode, Parsers parsers)
        {
            bool NoChange;
            string strTemp = parsers.Unicodify(mArticleText, out NoChange);

            if (SkipNoUnicode && NoChange)
                mAWBLogListener.AWBSkipped("No Unicodification");
            else if (!NoChange)
                this.AWBChangeArticleText("Article Unicodified", strTemp, false);
        }

        public void UpdateImages(ImageReplaceOptions option, Parsers parsers,
            string ImageReplaceText, string ImageWithText, bool SkipNoImgChange)
        {
            bool NoChange=false; string strTemp="";

            switch(option)
            {
                case ImageReplaceOptions.NoAction:
                    return;

                case ImageReplaceOptions.Replace:
                    strTemp = parsers.ReplaceImage(ImageReplaceText, ImageWithText, mArticleText, out NoChange);
                    break;

                case ImageReplaceOptions.Remove:
                    strTemp  = parsers.RemoveImage(ImageReplaceText, mArticleText, false, ImageWithText, out NoChange);
                    break;

                case ImageReplaceOptions.Comment:
                    strTemp = parsers.RemoveImage(ImageReplaceText, mArticleText, true, ImageWithText, out NoChange);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (NoChange && SkipNoImgChange)
                mAWBLogListener.AWBSkipped("No Image Changed");
            else if (!NoChange)
                this.AWBChangeArticleText("Image replacement applied", strTemp, false);
        }

        public void Categorisation(CategorisationOptions option, Parsers parsers,
            bool SkipNoCatChange, string NewCategoryText, string NewCategoryText2)
        {
            bool NoChange = false; string strTemp = "", action = "";

            switch (option)
            {
                case CategorisationOptions.NoAction:
                    return;

                case CategorisationOptions.AddCat:
                    if (NewCategoryText.Length < 1) return;
                    strTemp = parsers.AddCategory(NewCategoryText, mArticleText, mName);
                    action = "Added " + NewCategoryText;
                    break;

                case CategorisationOptions.ReCat:
                    if (NewCategoryText.Length < 1 || NewCategoryText2.Length < 1) return;
                    strTemp = parsers.ReCategoriser(NewCategoryText, NewCategoryText2, mArticleText, out NoChange);
                    break;

                case CategorisationOptions.RemoveCat:
                    if (NewCategoryText.Length < 1) return;
                    strTemp = parsers.RemoveCategory(NewCategoryText, mArticleText, out NoChange);
                    action = "Removed " + NewCategoryText;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (NoChange && SkipNoCatChange)
                mAWBLogListener.AWBSkipped("No Category Changed");
            else if (!NoChange)
                this.AWBChangeArticleText(action, strTemp, false);
        }

        public void Categorisation(CategorisationOptions option, Parsers parsers,
            bool SkipNoCatChange, string NewCategoryText)
        { Categorisation(option, parsers, SkipNoCatChange, NewCategoryText, ""); }

        public void PerformFindAndReplace(FindandReplace findAndReplace, SubstTemplates substTemplates,
            WikiFunctions.MWB.ReplaceSpecial replaceSpecial, bool SkipWhenNoFAR)
        {
            string strTemp, testText = mArticleText, tmpEditSummary="";

            strTemp = findAndReplace.MultipleFindAndReplce(mArticleText, mName, ref tmpEditSummary);
            strTemp = replaceSpecial.ApplyRules(strTemp, mName);
            strTemp = substTemplates.SubstituteTemplates(strTemp, mName); // TODO: Possible bug, this was "articleTitle" not "Name"

            if (SkipWhenNoFAR && (testText == strTemp)) // NoChange
                mAWBLogListener.AWBSkipped("No Find And Replace Changes");
            else
            {
                this.AWBChangeArticleText("Find and replace applied" + tmpEditSummary, strTemp, false);
                EditSummary += tmpEditSummary;
            }
        }

        public void PerformTypoFixes(RegExTypoFix RegexTypos, bool SkipIfNoRegexTypo)
        {
            bool NoChange;
            string strTemp = RegexTypos.PerformTypoFixes(mArticleText, out NoChange, out mPluginEditSummary);

            if (NoChange && SkipIfNoRegexTypo)
                mAWBLogListener.AWBSkipped("No typo fixes");
            else if (!NoChange)
            {
                this.AWBChangeArticleText(mPluginEditSummary, strTemp, false);
                AppendPluginEditSummary();
            }
        }

        public void AutoTag(Parsers parsers, bool SkipNoAutoTag)
        {
            bool NoChange; string tmpEditSummary="";
            string strTemp = parsers.Tagger(mArticleText, mName, out NoChange, ref tmpEditSummary);

            if (SkipNoAutoTag && SkipArticle)
                mAWBLogListener.AWBSkipped("No Tag changed");
            else if (!NoChange)
            {
                this.AWBChangeArticleText("Auto tagger changes applied" + tmpEditSummary, strTemp, false);
                EditSummary += tmpEditSummary;
            }
        }

        public void FixHeaderErrors(Parsers parsers, LangCodeEnum LangCode, bool SkipNoHeaderError)
        {
            if (LangCode == LangCodeEnum.en)
            {
                bool NoChange;
                string strTemp = parsers.Conversions(mArticleText);
                strTemp = parsers.FixDates(strTemp);
                strTemp = parsers.LivingPeople(strTemp, out NoChange);
                strTemp = parsers.ChangeToDefaultSort(strTemp, mName);
                strTemp = parsers.FixHeadings(strTemp, mName, out NoChange);
                if (SkipNoHeaderError && NoChange)
                    mAWBLogListener.AWBSkipped("No header errors");
                else if (!NoChange)
                    this.AWBChangeArticleText("Fixed header errors", strTemp, true);
            }
        }

        public void FixLinks(Parsers parsers, bool SkipNoBadLink)
        {
            bool NoChange;
            string strTemp = parsers.FixLinks(mArticleText, out NoChange);
            if (NoChange && SkipArticle)
                mAWBLogListener.AWBSkipped("No bad links");
            else if (!NoChange)
                this.AWBChangeArticleText("Fixed links", strTemp, false);
        }

        public void BulletExternalLinks(Parsers parsers, bool SkipNoBulletedLink)
        {
            bool NoChange;
            string strTemp = parsers.BulletExternalLinks(mArticleText, out NoChange);
            if (SkipNoBulletedLink && NoChange)
                mAWBLogListener.AWBSkipped("No missing bulleted links");
            else if (!NoChange)
                this.AWBChangeArticleText("Bulleted external links", strTemp, false);
        }

        public void EmboldenTitles(Parsers parsers, bool SkipNoBoldTitle)
        {
            bool NoChange;
            string strTemp = parsers.BoldTitle(mArticleText, mName, out NoChange);
            if (SkipNoBoldTitle && NoChange)
               mAWBLogListener.AWBSkipped("No Titles to embolden");
            else if (!NoChange)
                this.AWBChangeArticleText("Emboldened titles", strTemp, false);
        }

        /// <summary>
        /// Disambiguate
        /// </summary>
        /// <returns>True if OK to proceed, false to abort</returns>
        public bool Disambiguate(string DabLinkText, string[] DabVariantsLines, bool BotMode, int ContextChar,
            bool SkipNoDab)
        {
            bool NoChange;
            AutoWikiBrowser.DabForm df = new AutoWikiBrowser.DabForm();
            string strTemp = df.Disambiguate(mArticleText, mName, DabLinkText,
                DabVariantsLines, ContextChar, BotMode, out NoChange);

            if (df.Abort) return false;

            if (NoChange && SkipNoDab)
                mAWBLogListener.AWBSkipped("No disambiguation");
            else if (!NoChange)
                this.AWBChangeArticleText("Disambiguated " + DabLinkText, strTemp, false);

            return true;
        }

        public void HideText(HideText RemoveText)
        { mArticleText = RemoveText.Hide(mArticleText); }

        public void UnHideText(HideText RemoveText)
        { mArticleText = RemoveText.AddBack(mArticleText); }

        public bool CanDoGeneralFixes
        { get { return (NameSpaceKey == 0 || NameSpaceKey == 14 || Name.Contains("Sandbox")); } }

        #region Overrides

        public override string ToString()
        {
            return mName;
        }

        public override int GetHashCode()
        {
            return mName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Article)) return false;
            return mName == (obj as Article).mName;
            /*
            if (obj.GetHashCode() == this.GetHashCode())
                return true;
            else
                return false;
            */
        }

        #endregion

        #region ProcessArticleEventArgs

        IMyTraceListener ProcessArticleEventArgs.AWBLogItem
        { get { return mAWBLogListener; } }

        string ProcessArticleEventArgs.ArticleTitle
        { get { return mName; } }

        string ProcessArticleEventArgs.EditSummary // this is temp edit summary field, sent from plugin
        { get { return mPluginEditSummary; } set { mPluginEditSummary = value.Trim(); } }

        [XmlAttribute]
        public int NameSpaceKey
        { get { return mNameSpaceKey; } set { mNameSpaceKey = value; } }

        bool ProcessArticleEventArgs.Skip
        { get { return mPluginSkip; } set { mPluginSkip = value; } }

        #endregion
    }
}
