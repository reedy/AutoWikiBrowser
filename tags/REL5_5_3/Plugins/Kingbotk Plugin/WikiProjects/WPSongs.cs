using System.Windows.Forms;
using System.Xml;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;

namespace AutoWikiBrowser.Plugins.Kingbotk.WikiProjects
{
    internal sealed class WPSongs : PluginBase
    {
        private const string Prefix = "Songs";

        private const string PluginName = "WikiProject Songs";

        internal WPSongs() : base("WikiProjectSongs|WP Songs|Song|Songs|WPSongs|WikiProject Song")
        {
            // Specify alternate names only

            TemplateParameters[] @params = new TemplateParameters[-1 + 1];

            _ourSettingsControl = new GenericWithWorkgroups(PluginName, Prefix, true, @params);
        }

        // Settings:
        private readonly TabPage _ourTab = new TabPage(Prefix);

        private readonly GenericWithWorkgroups _ourSettingsControl;

        protected internal override string PluginShortName
        {
            get { return Prefix; }
        }

        protected override string PreferredTemplateName
        {
            get { return PluginName; }
        }

        protected internal override IGenericSettings GenericSettings
        {
            get { return _ourSettingsControl; }
        }

        // Initialisation:

        protected internal override void Initialise()
        {
            OurMenuItem = new ToolStripMenuItem("Songs Plugin");
            InitialiseBase();
            // must set menu item object first
            _ourTab.UseVisualStyleBackColor = true;
            _ourTab.Controls.Add(_ourSettingsControl);
        }

        // Article processing:
        protected override bool SkipIfContains()
        {
            return false;
        }

        protected override void ProcessArticleFinish()
        {
            StubClass();
        }

        protected override bool TemplateFound()
        {
            // Nothing to do here
            return false;
        }

        protected override string WriteTemplateHeader()
        {
            return "{{" + PluginName + WriteOutParameterToHeader("class");
        }

        //User interface:
        protected override void ShowHideOurObjects(bool visible)
        {
            PluginManager.ShowHidePluginTab(_ourTab, visible);
        }

        // XML settings:
        protected internal override void ReadXML(XmlTextReader reader)
        {
            Enabled = PluginManager.XMLReadBoolean(reader, Prefix + "Enabled", Enabled);
            _ourSettingsControl.ReadXML(reader);
        }

        protected internal override void Reset()
        {
            _ourSettingsControl.Reset();
        }

        protected internal override void WriteXML(XmlTextWriter writer)
        {
            writer.WriteAttributeString(Prefix + "Enabled", Enabled.ToString());
            _ourSettingsControl.WriteXML(writer);
        }

        // Misc:
        internal override bool HasReqPhotoParam
        {
            get { return false; }
        }

        internal override void ReqPhoto()
        {
        }
    }
}