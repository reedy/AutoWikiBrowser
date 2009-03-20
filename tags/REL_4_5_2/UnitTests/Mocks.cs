using WikiFunctions.Plugin;

namespace UnitTests
{
    class MockSkipOptions : ISkipOptions
    {
        public bool SkipNoUnicode
        { get; set; }

        public bool SkipNoTag
        { get; set; }

        public bool SkipNoHeaderError
        { get; set; }

        public bool SkipNoBoldTitle
        { get; set; }

        public bool SkipNoBulletedLink
        { get; set; }

        public bool SkipNoBadLink
        { get; set; }

        public bool SkipNoDefaultSortAdded
        { get; set; }
    }
}
