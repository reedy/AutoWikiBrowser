using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using My;
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
//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{

	internal sealed class WPNovels : PluginBase
	{

		// Settings:
		private readonly TabPage OurTab = new TabPage(Prefix);

		private GenericWithWorkgroups OurSettingsControl;
		private const string Prefix = "Novels";

		private const string PluginName = "WikiProject Novels";
		internal WPNovels() : base("Novels|WPNovels")
		{
			// Specify alternate names only

			OurSettingsControl = new GenericWithWorkgroups(PluginName, Prefix, true, @params);
		}

		readonly TemplateParameters[] @params = {
			new TemplateParameters {
				StorageKey = "CrimeWG",
				Group = "",
				ParamName = "Crime"
			},
			new TemplateParameters {
				StorageKey = "ShortStoryWG",
				Group = "",
				ParamName = "Short Story"
			},
			new TemplateParameters {
				StorageKey = "SFWG",
				Group = "",
				ParamName = "SF"
			},
			new TemplateParameters {
				StorageKey = "AusWG",
				Group = "",
				ParamName = "Australian"
			},
			new TemplateParameters {
				StorageKey = "FantWG",
				Group = "",
				ParamName = "Fantasy"
			},
			new TemplateParameters {
				StorageKey = "19thWG",
				Group = "",
				ParamName = "19thC"
			},
			new TemplateParameters {
				StorageKey = "NarniaWG",
				Group = "",
				ParamName = "Narnia"
			},
			new TemplateParameters {
				StorageKey = "LemonyWG",
				Group = "",
				ParamName = "Lemony Snicket"
			},
			new TemplateParameters {
				StorageKey = "ShannaraWG",
				Group = "",
				ParamName = "Shannara"
			},
			new TemplateParameters {
				StorageKey = "SwordWG",
				Group = "",
				ParamName = "Sword of Truth"
			},
			new TemplateParameters {
				StorageKey = "TwilightWG",
				Group = "",
				ParamName = "Twilight"
			},
			new TemplateParameters {
				StorageKey = "OldPeerReview",
				Group = "",
				ParamName = "Old Peer Review"
			}

		};
		protected internal override string PluginShortName {
			get { return Prefix; }
		}
		protected override string PreferredTemplateName {
			get { return PluginName; }
		}

		protected override void ImportanceParameter(Importance Importance)
		{
			Template.NewOrReplaceTemplateParm("importance", Importance.ToString(), article, false, false);
		}
		protected internal override IGenericSettings GenericSettings {
			get { return OurSettingsControl; }
		}

		internal override bool HasReqPhotoParam {
			get { return true; }
		}
		internal override void ReqPhoto()
		{
			AddNewParamWithAYesValue("needs-infobox-cover");
		}

		// Initialisation:
		protected internal override void Initialise()
		{
			OurMenuItem = new ToolStripMenuItem("Novels Plugin");
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
					AddAndLogNewParamWithAYesValue(param + "-task-force");
					//Probably needs some reformatting
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
	}
}
