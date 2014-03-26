using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions;
using WikiFunctions.Plugin;
internal sealed class WPSongs : PluginBase
{

	private const string Prefix = "Songs";

	private const string PluginName = "WikiProject Songs";
	internal WPSongs() : base("WikiProjectSongs|WP Songs|Song|Songs|WPSongs|WikiProject Song")
	{
		// Specify alternate names only

		TemplateParameters[] @params = new TemplateParameters[-1 + 1];

		OurSettingsControl = new GenericWithWorkgroups(PluginName, Prefix, true, @params);
	}

	// Settings:
	private readonly TabPage OurTab = new TabPage(PluginName);

	private GenericWithWorkgroups OurSettingsControl;
	protected internal override string PluginShortName {
		get { return Prefix; }
	}
	protected override string PreferredTemplateName {
		get { return PluginName; }
	}
	protected override void ImportanceParameter(Importance Importance)
	{
		// {{WikiProject Songs}} doesn't do importance
	}
	protected internal override IGenericSettings GenericSettings {
		get { return OurSettingsControl; }
	}

	// Initialisation:

	protected internal override void Initialise()
	{
		OurMenuItem = new ToolStripMenuItem("Songs Plugin");
		InitialiseBase();
		// must set menu item object first
		OurTab.UseVisualStyleBackColor = true;
		OurTab.Controls.Add(OurSettingsControl);
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
		return "{{WikiProject Songs" + WriteOutParameterToHeader("class");
	}

	//User interface:
	protected override void ShowHideOurObjects(bool Visible)
	{
		PluginManager.ShowHidePluginTab(OurTab, Visible);
	}

	// XML settings:
	protected internal override void ReadXML(XmlTextReader Reader)
	{
		bool blnNewVal = PluginManager.XMLReadBoolean(Reader, Prefix + "Enabled", Enabled);
		if (!(blnNewVal == Enabled))
			Enabled = blnNewVal;
		// Mustn't set if the same or we get extra tabs
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

	// Misc:
	internal override bool HasReqPhotoParam {
		get { return false; }
	}
	internal override void ReqPhoto()
	{
	}
}
