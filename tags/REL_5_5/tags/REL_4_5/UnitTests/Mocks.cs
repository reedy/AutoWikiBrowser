using WikiFunctions.Plugin;

namespace UnitTests
{
    class MockSkipOptions : ISkipOptions
    {
        public bool m_SkipNoUnicode;
        public bool SkipNoUnicode
        {
            get { return m_SkipNoUnicode; }
        }

        public bool m_SkipNoTag;
        public bool SkipNoTag
        {
            get { return m_SkipNoTag; }
        }

        public bool m_SkipNoHeaderError;
        public bool SkipNoHeaderError
        {
            get { return m_SkipNoHeaderError; }
        }

        public bool m_SkipNoBoldTitle;
        public bool SkipNoBoldTitle
        {
            get { return m_SkipNoBoldTitle; }
        }

        public bool m_SkipNoBulletedLink;
        public bool SkipNoBulletedLink
        {
            get { return m_SkipNoBulletedLink; }
        }

        public bool m_SkipNoBadLink;
        public bool SkipNoBadLink
        {
            get { return m_SkipNoBadLink; }
        }

        public bool m_SkipNoDefaultSortAdded;
        public bool SkipNoDefaultSortAdded
        {
            get { return m_SkipNoDefaultSortAdded; }
        }
    }
}
