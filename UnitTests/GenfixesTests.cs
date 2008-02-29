// This file is only for tests that require more than one transformation functions at the same time, so
//don't add tests for separate functions here

using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using NUnit.Framework;
using WikiFunctions.Parse;
using WikiFunctions.Plugin;
using WikiFunctions.Logging;

namespace UnitTests
{
    #region Mock objects
    class MockSkipOptions : ISkipOptions
    {
        public bool m_SkipNoUnicode = false;
        public bool SkipNoUnicode
        {
            get { return m_SkipNoUnicode; }
        }

        public bool m_SkipNoTag = false;
        public bool SkipNoTag
        {
            get { return m_SkipNoTag; }
        }

        public bool m_SkipNoHeaderError = false;
        public bool SkipNoHeaderError
        {
            get { return m_SkipNoHeaderError; }
        }

        public bool m_SkipNoBoldTitle = false;
        public bool SkipNoBoldTitle
        {
            get { return m_SkipNoBoldTitle; }
        }

        public bool m_SkipNoBulletedLink = false;
        public bool SkipNoBulletedLink
        {
            get { return m_SkipNoBulletedLink; }
        }

        public bool m_SkipNoBadLink = false;
        public bool SkipNoBadLink
        {
            get { return m_SkipNoBadLink; }
        }

        public bool m_SkipNoDefaultSortAdded = false;
        public bool SkipNoDefaultSortAdded
        {
            get { return m_SkipNoDefaultSortAdded; }
        }
    }
    #endregion

    [TestFixture]
    public class GenfixesTests
    {
        #region Preparations
        Article a = new Article("Test");
        Parsers p = new Parsers();
        HideText h = new HideText();
        MockSkipOptions s = new MockSkipOptions();

        void GenFixes(bool replaceReferenceTags)
        {
            a.PerformGeneralFixes(p, h, s, replaceReferenceTags);
        }

        void GenFixes()
        {
            GenFixes(true);
        }

        [SetUp]
        public void SetUp()
        {
            Globals.UnitTestMode = true;
            WikiRegexes.MakeLangSpecificRegexes();
            a.InitialiseLogListener();
        }
        #endregion

        string ArticleText
        {
            get { return a.ArticleText; }
            set
            {
                a.AWBChangeArticleText("unit testing", value, true);
                a.OriginalArticleText = value;
            }
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_Underscore_removal_in_URL.27s_in_wikilinks
        public void UndersoreRemovalInExternalLink()
        {
            // just in case...
            ArticleText = "test http://some_link test";
            GenFixes();

            Assert.AreEqual(a.OriginalArticleText, ArticleText);
            
            ArticleText = "[http://some_link]";
            GenFixes();

            Assert.AreEqual(a.OriginalArticleText, ArticleText);

            ArticleText = "[[http://some_link]]";
            GenFixes();

            Assert.AreEqual("[http://some_link]", ArticleText);
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_Underscore_removal_in_URL.27s_in_wikilinks
        public void ExternalLinksInImageCaptions()
        {
            ArticleText = "[[Image:foo.jpg|Some http://some_crap.com]]";
            GenFixes();

            Assert.AreEqual(a.OriginalArticleText, ArticleText);

            ArticleText = "[[Image:foo.jpg|Some [http://some_crap.com]]]";
            GenFixes();

            Assert.AreEqual(a.OriginalArticleText, ArticleText);

            ArticleText = "[[Image:foo.jpg|Some [[http://some_crap.com]]]]";
            GenFixes();

            // not performing a full comparison due to a bug that should be tested elsewhere
            StringAssert.StartsWith("[[Image:foo.jpg|Some [http://some_crap.com]]]", ArticleText);
        }
    }
}
