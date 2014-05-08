using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using System.Windows.Forms;
using System.Xml;

internal sealed class WPAustralia : PluginBase
{
    internal WPAustralia() : base("")
    {
        // Specify alternate names only

        _ourSettingsControl = new GenericWithWorkgroups(PluginName, Prefix, true, _params);
    }

    private const string Prefix = "Aus";

    private const string PluginName = "WikiProject Australia";
    private const string PlacesGroup = "Places";
    private const string SportsGroup = "Sports";

    private const string OtherGroup = "Other topics";

    private readonly TemplateParameters[] _params =
    {
        new TemplateParameters
        {
            StorageKey = "Place",
            Group = PlacesGroup,
            ParamName = "place"
        },
        new TemplateParameters
        {
            StorageKey = "AAT",
            Group = PlacesGroup,
            ParamName = "AAT"
        },
        new TemplateParameters
        {
            StorageKey = "Adel",
            Group = PlacesGroup,
            ParamName = "Adelaide"
        },
        new TemplateParameters
        {
            StorageKey = "Bris",
            Group = PlacesGroup,
            ParamName = "Brisbane"
        },
        new TemplateParameters
        {
            StorageKey = "Canb",
            Group = PlacesGroup,
            ParamName = "Canberra"
        },
        new TemplateParameters
        {
            StorageKey = "Gee",
            Group = PlacesGroup,
            ParamName = "Geelong"
        },
        new TemplateParameters
        {
            StorageKey = "Melb",
            Group = PlacesGroup,
            ParamName = "Melbourne"
        },
        new TemplateParameters
        {
            StorageKey = "Perth",
            Group = PlacesGroup,
            ParamName = "Perth"
        },
        new TemplateParameters
        {
            StorageKey = "Sydney",
            Group = PlacesGroup,
            ParamName = "Sydney"
        },
        new TemplateParameters
        {
            StorageKey = "River",
            Group = PlacesGroup,
            ParamName = "Riverina"
        },
        new TemplateParameters
        {
            StorageKey = "TAS",
            Group = PlacesGroup,
            ParamName = "TAS"
        },
        new TemplateParameters
        {
            StorageKey = "Sport",
            Group = SportsGroup,
            ParamName = "sports"
        },
        new TemplateParameters
        {
            StorageKey = "AFL",
            Group = SportsGroup,
            ParamName = "afl"
        },
        new TemplateParameters
        {
            StorageKey = "NBL",
            Group = SportsGroup,
            ParamName = "nbl"
        },
        new TemplateParameters
        {
            StorageKey = "NRL",
            Group = SportsGroup,
            ParamName = "nrl"
        },
        new TemplateParameters
        {
            StorageKey = "V8",
            Group = SportsGroup,
            ParamName = "v8"
        },
        new TemplateParameters
        {
            StorageKey = "Crime",
            Group = OtherGroup,
            ParamName = "crime"
        },
        new TemplateParameters
        {
            StorageKey = "Law",
            Group = OtherGroup,
            ParamName = "law"
        },
        new TemplateParameters
        {
            StorageKey = "Military",
            Group = OtherGroup,
            ParamName = "military"
        },
        new TemplateParameters
        {
            StorageKey = "Music",
            Group = OtherGroup,
            ParamName = "music"
        },
        new TemplateParameters
        {
            StorageKey = "Politics",
            Group = OtherGroup,
            ParamName = "politics"
        }
    };

    // Settings:
    private readonly TabPage _ourTab = new TabPage("Australia");

    private readonly GenericWithWorkgroups _ourSettingsControl;

    protected internal override string PluginShortName
    {
        get { return "Australia"; }
    }

    protected override void ImportanceParameter(Importance importance)
    {
        Template.NewOrReplaceTemplateParm("importance", importance.ToString(), TheArticle, false, false);
    }

    protected internal override IGenericSettings GenericSettings
    {
        get { return _ourSettingsControl; }
    }

    protected override string PreferredTemplateName
    {
        get { return PluginName; }
    }

    // Initialisation:

    protected internal override void Initialise()
    {
        OurMenuItem = new ToolStripMenuItem("Australia Plugin");
        InitialiseBase();
        // must set menu item object first
        _ourTab.UseVisualStyleBackColor = true;
        _ourTab.Controls.Add(_ourSettingsControl);
    }

    // Article processing:
    protected override bool SkipIfContains()
    {
        // None
        return false;
    }

    protected override void ProcessArticleFinish()
    {
        StubClass();
        foreach (ListViewItem lvi in _ourSettingsControl.ListView1.Items)
        {
            if (lvi.Checked)
            {
                TemplateParameters tp = (TemplateParameters) lvi.Tag;
                AddAndLogNewParamWithAYesValue(tp.ParamName.ToLower());
                //Probably needs some reformatting
            }
        }
    }

    protected override bool TemplateFound()
    {
        if (CheckForDoublyNamedParameters("V8", "v8"))
            return true;
        // tag is bad, exit
        if (CheckForDoublyNamedParameters("nbl", "NBL"))
            return true;

        return false;
    }

    protected override string WriteTemplateHeader()
    {
        return "{{" + PluginName + WriteOutParameterToHeader("class") + WriteOutParameterToHeader("importance");
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

    // Not implemented:
    internal override bool HasReqPhotoParam
    {
        get { return false; }
    }

    internal override void ReqPhoto()
    {
    }
}