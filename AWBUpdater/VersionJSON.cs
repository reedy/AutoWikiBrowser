using System.Collections.Generic;

namespace AWBUpdater
{
    public class Enabledversion
    {
        public string version { get; set; }
        public string dotnetversion { get; set; }
        public bool dev { get; set; }
    }

    public class RootObject
    {
        public string updaterversion { get; set; }
        public List<Enabledversion> enabledversions { get; set; }
        public List<object> messages { get; set; }
        public List<string> globalusers { get; set; }
        public List<object> badnames { get; set; }
    }
}
