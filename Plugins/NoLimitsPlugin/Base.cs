using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.NoLimitsPlugin
{
    internal class Base : IAWBBasePlugin
    {
        static internal IAutoWikiBrowser AWB;

        public void Initialise(IAutoWikiBrowser sender)
        {
            AWB = sender;
        }

        public string Name
        {
            get { return "No Limits Plugin"; }
        }

        public string WikiName
        {
            get { return "[[WP:AWB|" + Name + "]]"; }
        }

        public void LoadSettings(object[] prefs)
        {
        }

        public object[] SaveSettings()
        {
            return new object[0];
        }
    }
}
