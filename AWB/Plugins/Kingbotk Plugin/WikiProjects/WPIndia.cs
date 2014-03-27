using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;

using System.Windows.Forms;
using System.Xml;

internal sealed class WPIndia : PluginBase
{

	// Settings:
	private readonly TabPage OurTab = new TabPage("India");

	private readonly GenericWithWorkgroups OurSettingsControl;
	private const string Prefix = "Ind";

	const string PluginName = "WikiProject India";
	const string GeographyGroup = "Geography";

	const string OthersGroup = "Others";
	readonly TemplateParameters[] @params = {
		new TemplateParameters {
			StorageKey = "Geography",
			Group = GeographyGroup,
			ParamName = "Geography"
		},
		new TemplateParameters {
			StorageKey = "Maps",
			Group = GeographyGroup,
			ParamName = "Maps"
		},
		new TemplateParameters {
			StorageKey = "Cities",
			Group = GeographyGroup,
			ParamName = "Cities"
		},
		new TemplateParameters {
			StorageKey = "Chennai",
			Group = GeographyGroup,
			ParamName = "Chennai"
		},
		new TemplateParameters {
			StorageKey = "Mumbai",
			Group = GeographyGroup,
			ParamName = "Mumbai"
		},
		new TemplateParameters {
			StorageKey = "Hyderabad",
			Group = GeographyGroup,
			ParamName = "Hyderabad"
		},
		new TemplateParameters {
			StorageKey = "Districts",
			Group = GeographyGroup,
			ParamName = "Districts"
		},
		new TemplateParameters {
			StorageKey = "States",
			Group = GeographyGroup,
			ParamName = "States"
		},
		new TemplateParameters {
			StorageKey = "Andhra",
			Group = GeographyGroup,
			ParamName = "Andhra Pradesh"
		},
		new TemplateParameters {
			StorageKey = "Arunachal",
			Group = GeographyGroup,
			ParamName = "Arunachal Pradesh"
		},
		new TemplateParameters {
			StorageKey = "Assam",
			Group = GeographyGroup,
			ParamName = "Assam"
		},
		new TemplateParameters {
			StorageKey = "Bengal",
			Group = GeographyGroup,
			ParamName = "Bengal"
		},
		new TemplateParameters {
			StorageKey = "Bihar",
			Group = GeographyGroup,
			ParamName = "Bihar"
		},
		new TemplateParameters {
			StorageKey = "Chhattisgarh",
			Group = GeographyGroup,
			ParamName = "Chhattisgarh"
		},
		new TemplateParameters {
			StorageKey = "Goa",
			Group = GeographyGroup,
			ParamName = "Goa"
		},
		new TemplateParameters {
			StorageKey = "Gujarat",
			Group = GeographyGroup,
			ParamName = "Gujarat"
		},
		new TemplateParameters {
			StorageKey = "Haryana",
			Group = GeographyGroup,
			ParamName = "Haryana"
		},
		new TemplateParameters {
			StorageKey = "Himachal",
			Group = GeographyGroup,
			ParamName = "Himachal Pradesh"
		},
		new TemplateParameters {
			StorageKey = "JandK",
			Group = GeographyGroup,
			ParamName = "Jammu and Kashmir"
		},
		new TemplateParameters {
			StorageKey = "Jharkhand",
			Group = GeographyGroup,
			ParamName = "Jharkhand"
		},
		new TemplateParameters {
			StorageKey = "Karnataka",
			Group = GeographyGroup,
			ParamName = "Karnataka"
		},
		new TemplateParameters {
			StorageKey = "Kerala",
			Group = GeographyGroup,
			ParamName = "Kerala"
		},
		new TemplateParameters {
			StorageKey = "Madhya",
			Group = GeographyGroup,
			ParamName = "Madhya Pradesh"
		},
		new TemplateParameters {
			StorageKey = "Maharashtra",
			Group = GeographyGroup,
			ParamName = "Maharashtra"
		},
		new TemplateParameters {
			StorageKey = "Manipur",
			Group = GeographyGroup,
			ParamName = "Manipur"
		},
		new TemplateParameters {
			StorageKey = "Meghalaya",
			Group = GeographyGroup,
			ParamName = "Meghalaya"
		},
		new TemplateParameters {
			StorageKey = "Mizoram",
			Group = GeographyGroup,
			ParamName = "Mizoram"
		},
		new TemplateParameters {
			StorageKey = "Nagaland",
			Group = GeographyGroup,
			ParamName = "Nagaland"
		},
		new TemplateParameters {
			StorageKey = "Orissa",
			Group = GeographyGroup,
			ParamName = "Orissa"
		},
		new TemplateParameters {
			StorageKey = "Punjab",
			Group = GeographyGroup,
			ParamName = "Punjab"
		},
		new TemplateParameters {
			StorageKey = "Rajasthan",
			Group = GeographyGroup,
			ParamName = "Rajasthan"
		},
		new TemplateParameters {
			StorageKey = "Sikkim",
			Group = GeographyGroup,
			ParamName = "Sikkim"
		},
		new TemplateParameters {
			StorageKey = "Tamilnadu",
			Group = GeographyGroup,
			ParamName = "Tamil Nadu"
		},
		new TemplateParameters {
			StorageKey = "Tripura",
			Group = GeographyGroup,
			ParamName = "Tripura"
		},
		new TemplateParameters {
			StorageKey = "Uttar",
			Group = GeographyGroup,
			ParamName = "Uttar Pradesh"
		},
		new TemplateParameters {
			StorageKey = "Uttarakand",
			Group = GeographyGroup,
			ParamName = "Uttarakand"
		},
		new TemplateParameters {
			StorageKey = "Andaman",
			Group = GeographyGroup,
			ParamName = "Andaman and Nicobar Island"
		},
		new TemplateParameters {
			StorageKey = "Chandigarh",
			Group = GeographyGroup,
			ParamName = "Chandigarh"
		},
		new TemplateParameters {
			StorageKey = "Dadra",
			Group = GeographyGroup,
			ParamName = "Dadra and Nagar Haveli"
		},
		new TemplateParameters {
			StorageKey = "Daman",
			Group = GeographyGroup,
			ParamName = "Daman and Diu"
		},
		new TemplateParameters {
			StorageKey = "Delhi",
			Group = GeographyGroup,
			ParamName = "Delhi"
		},
		new TemplateParameters {
			StorageKey = "Lakshadweep",
			Group = GeographyGroup,
			ParamName = "Lakshadweep"
		},
		new TemplateParameters {
			StorageKey = "Puducherry",
			Group = GeographyGroup,
			ParamName = "Puducherry"
		},
		new TemplateParameters {
			StorageKey = "Cinema",
			Group = OthersGroup,
			ParamName = "Cinema"
		},
		new TemplateParameters {
			StorageKey = "History",
			Group = OthersGroup,
			ParamName = "History"
		},
		new TemplateParameters {
			StorageKey = "Literature",
			Group = OthersGroup,
			ParamName = "Literature"
		},
		new TemplateParameters {
			StorageKey = "Politics",
			Group = OthersGroup,
			ParamName = "Politics"
		},
		new TemplateParameters {
			StorageKey = "ProtectedAreas",
			Group = OthersGroup,
			ParamName = "Protected Areas"
		},
		new TemplateParameters {
			StorageKey = "Tamil",
			Group = OthersGroup,
			ParamName = "Tamil"
		},
		new TemplateParameters {
			StorageKey = "Tele",
			Group = OthersGroup,
			ParamName = "Television"
		}

	};
	internal WPIndia() : base("")
	{
		// Specify alternate names only

		OurSettingsControl = new GenericWithWorkgroups(PluginName, Prefix, true, @params);
	}

	protected internal override string PluginShortName {
		get { return "India"; }
	}

	protected override void ImportanceParameter(Importance Importance)
	{
		Template.NewOrReplaceTemplateParm("importance", Importance.ToString(), article, false, false);
	}
	protected internal override IGenericSettings GenericSettings {
		get { return OurSettingsControl; }
	}

	protected override string PreferredTemplateName {
		get { return PluginName; }
	}
	internal override bool HasReqPhotoParam {
		get { return true; }
	}
	internal override void ReqPhoto()
	{
		AddNewParamWithAYesValue("image-needed");
	}

	// Initialisation:
	protected internal override void Initialise()
	{
		OurMenuItem = new ToolStripMenuItem("India Plugin");
		InitialiseBase();
		// must set menu item object first
		OurTab.UseVisualStyleBackColor = true;
		OurTab.Controls.Add(OurSettingsControl);
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

		var _with1 = OurSettingsControl;
		foreach (ListViewItem lvi in _with1.ListView1.Items) {
			if (lvi.Checked) {
				TemplateParameters tp = (TemplateParameters)lvi.Tag;
				string param = tp.ParamName.ToLower().Replace(" ", "-");
				AddAndLogNewParamWithAYesValue(param);
				AddEmptyParam(param + "-importance");
			}
		}
	}
	protected override bool TemplateFound()
	{
	    return false;
	}

	protected override string WriteTemplateHeader()
	{
		return "{{" + PluginName + WriteOutParameterToHeader("class") + WriteOutParameterToHeader("importance");
	}

	//User interface:
	protected override void ShowHideOurObjects(bool Visible)
	{
		PluginManager.ShowHidePluginTab(OurTab, Visible);
	}

	// XML settings:
	protected internal override void ReadXML(XmlTextReader Reader)
	{
        Enabled = PluginManager.XMLReadBoolean(Reader, Prefix + "Enabled", Enabled);
		OurSettingsControl.ReadXML(Reader);
	}
	protected internal override void Reset()
	{
		OurSettingsControl.Reset();
	}
	protected internal override void WriteXML(XmlTextWriter Writer)
	{
		Writer.WriteAttributeString(Prefix + "Enabled", Enabled.ToString());
		OurSettingsControl.WriteXML(Writer);
	}
}
