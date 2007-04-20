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

namespace WikiFunctions
{
    public class Article : ProcessArticleEventArgs
    {
        protected AWBLogListener mAWBLogListener;

        public Article()
        { }
        
        public Article(string Name)
        {
            this.Name = Name;
            this.NameSpaceKey = Tools.CalculateNS(Name);
            this.EditSummary = "";
        }

        public Article(string Name, int NameSpaceKey)
        {
            this.Name = Name;
            this.NameSpaceKey = NameSpaceKey;
            this.EditSummary = "";
        }

        public AWBLogListener InitialiseLogListener()
        {
            mAWBLogListener = new Logging.AWBLogListener(this.Name);
            return mAWBLogListener;
        }

        public AWBLogListener LogListener
        { get { return mAWBLogListener; } set { mAWBLogListener = value; } }

        [XmlAttribute]
        public int NameSpaceKey;
        public string Name;

        [XmlIgnore]
        public string EditSummary = "";

        //public string ArticleText;
        //public string OriginalText;

        public string URLEncodedName
        {
            get { return Tools.WikiEncode(Name); }
        }

        #region Overrides

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Article)) return false;
            return Name == (obj as Article).Name;
            /*
            if (obj.GetHashCode() == this.GetHashCode())
                return true;
            else
                return false;
            */
        }

        #endregion

        #region ProcessArticleEventArgs
        protected string mPluginArticleText; // Todo: Not sure if we can just pass the Article object's text or not
        protected string mPluginEditSummary;
        protected bool mPluginSkip;

        IMyTraceListener ProcessArticleEventArgs.AWBLogItem
        { get { return mAWBLogListener; } }

        string ProcessArticleEventArgs.ArticleText
        { get { return mPluginArticleText; } }

        string ProcessArticleEventArgs.ArticleTitle
        { get { return Name; } }

        string ProcessArticleEventArgs.EditSummary
        { get { return mPluginEditSummary; } set { mPluginEditSummary = value.Trim(); } }

        int ProcessArticleEventArgs.Namespace
        { get { return NameSpaceKey; } }

        bool ProcessArticleEventArgs.Skip
        { get { return mPluginSkip; } set { mPluginSkip = value; } }

        public ProcessArticleEventArgs GetProcessArticleEventArgs(string articletext)
        {
            mPluginArticleText = articletext;
            return this;
        }

        #endregion
    }

    public interface ProcessArticleEventArgs
    {
        string ArticleText { get; }
        string ArticleTitle { get; }
        string EditSummary { get; set; }
        int Namespace { get; }
        IMyTraceListener AWBLogItem { get; }
        bool Skip { get; set; }
    }
}
