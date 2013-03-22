using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using WikiFunctions.Plugin;

namespace UnitTests
{
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
}
