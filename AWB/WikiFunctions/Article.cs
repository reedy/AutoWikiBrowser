/*
Autowikibrowser
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
        [XmlAttribute]
        protected int mNameSpaceKey;
        protected string mName;

        [XmlIgnore]
        protected string mEditSummary = "";
        protected AWBLogListener mAWBLogListener;
        protected string mArticleText = "";
        protected string mOriginalArticleText = "";

        protected string mArticleTextSentToPlugin; // Todo: Not sure if we can just pass the Article object's text or not
        protected string mPluginEditSummary;
        protected bool mPluginSkip;

        public Article()
        { }
        
        public Article(string mName)
        {
            this.mName = mName;
            this.mNameSpaceKey = Tools.CalculateNS(mName);
            this.mEditSummary = "";
        }

        public Article(string mName, int mNameSpaceKey)
        {
            this.mName = mName;
            this.mNameSpaceKey = mNameSpaceKey;
            this.mEditSummary = "";
        }

        public AWBLogListener InitialiseLogListener()
        {
            mAWBLogListener = new Logging.AWBLogListener(this.mName);
            return mAWBLogListener;
        }

        public AWBLogListener LogListener
        { get { return mAWBLogListener; } set { mAWBLogListener = value; } }

        public string URLEncodedName
        {
            get { return Tools.WikiEncode(mName); }
        }

        public string Name
        { get { return mName; } set { mName = value; } }

        public string ArticleText
        { get { return mArticleText; } } //set { mArticleText = value; } } // set using methods

        public string OriginalArticleText
        { get { return mOriginalArticleText; } set { mOriginalArticleText = value; mArticleText = value; } }

        public string EditSummary
        { get { return mEditSummary; } set { mEditSummary = value; } }

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
                mEditSummary += " " + mPluginEditSummary.Trim();
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
                this.AWBChangeArticleText("Unicodification", strTemp, false);
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
                this.AWBChangeArticleText("Image replacement", strTemp, false);
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
            string strTemp, testText = mArticleText;

            strTemp = findAndReplace.MultipleFindAndReplce(mArticleText, mName, ref mEditSummary);
            strTemp = replaceSpecial.ApplyRules(strTemp, mName);
            strTemp = substTemplates.SubstituteTemplates(strTemp, mName); // TODO: Possible bug, this was "articleTitle" not "Name"

            if (SkipWhenNoFAR && (testText == strTemp)) // NoChange
                mAWBLogListener.AWBSkipped("No Find And Replace Changes");
            else
                this.AWBChangeArticleText("Find and replace", strTemp, false);
        }

        public void PerformTypoFixes(RegExTypoFix RegexTypos, bool SkipIfNoRegexTypo)
        {
            bool NoChange;
            string strTemp = RegexTypos.PerformTypoFixes(mArticleText, out NoChange, out mPluginEditSummary);

            if (NoChange && SkipIfNoRegexTypo)
                mAWBLogListener.AWBSkipped("No Typo Fixes");
            else
                this.AWBChangeArticleText(mPluginEditSummary, strTemp, false);
                AppendPluginEditSummary();
        }

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

        string ProcessArticleEventArgs.ArticleText
        { get { return mArticleTextSentToPlugin; } }

        string ProcessArticleEventArgs.ArticleTitle
        { get { return mName; } }

        string ProcessArticleEventArgs.EditSummary // this is temp edit summary field, sent from plugin
        { get { return mPluginEditSummary; } set { mPluginEditSummary = value.Trim(); } }

        public int NameSpaceKey
        { get { return mNameSpaceKey; } }

        bool ProcessArticleEventArgs.Skip
        { get { return mPluginSkip; } set { mPluginSkip = value; } }

        #endregion
    }

    public interface ProcessArticleEventArgs
    {
        string ArticleText { get; }
        string ArticleTitle { get; }
        string EditSummary { get; set; }
        int NameSpaceKey { get; }
        IMyTraceListener AWBLogItem { get; }
        bool Skip { get; set; }
    }
}
