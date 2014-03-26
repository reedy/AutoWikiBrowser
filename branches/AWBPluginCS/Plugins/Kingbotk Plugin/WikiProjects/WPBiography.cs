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

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{

	internal enum Living
	{
		Unknown,
		Living,
		Dead
	}

	internal sealed class WPBiography : PluginBase
	{

		internal WPBiography() : base("WPBiography|Wpbiography|WPBIO|WP Biography|WPbiography|Wikiproject Biography|WP Bio|Bio")
		{
			// Specify alternate names only

			OurSettingsControl = new BioWithWorkgroups(PluginName, Prefix, true, @params);
		}

		private const string PluginName = "WikiProject Biography";

		private const string Prefix = "Bio";
		private const string WorkgroupsGroups = "Work Groups";

		private const string OthersGroup = "Others";
		readonly TemplateParameters[] @params = {
			new TemplateParameters {
				StorageKey = "PolWG",
				Group = WorkgroupsGroups,
				ParamName = "Politician"
			},
			new TemplateParameters {
				StorageKey = "ArtsEntsWG",
				Group = WorkgroupsGroups,
				ParamName = "A&E"
			},
			new TemplateParameters {
				StorageKey = "FilmWG",
				Group = WorkgroupsGroups,
				ParamName = "Film Bio"
			},
			new TemplateParameters {
				StorageKey = "MilitaryWG",
				Group = WorkgroupsGroups,
				ParamName = "Military"
			},
			new TemplateParameters {
				StorageKey = "PeerageWG",
				Group = WorkgroupsGroups,
				ParamName = "Peerage"
			},
			new TemplateParameters {
				StorageKey = "RoyaltyWG",
				Group = WorkgroupsGroups,
				ParamName = "Royalty"
			},
			new TemplateParameters {
				StorageKey = "MusiciansWG",
				Group = WorkgroupsGroups,
				ParamName = "Musician"
			},
			new TemplateParameters {
				StorageKey = "ScienceWG",
				Group = WorkgroupsGroups,
				ParamName = "S&A"
			},
			new TemplateParameters {
				StorageKey = "SportWG",
				Group = WorkgroupsGroups,
				ParamName = "Sports"
			},
			new TemplateParameters {
				StorageKey = "LivingPerson",
				Group = OthersGroup,
				ParamName = "Living"
			},
			new TemplateParameters {
				StorageKey = "NotLivingPerson",
				Group = OthersGroup,
				ParamName = "Not Living"
			},
			new TemplateParameters {
				StorageKey = "ActivePol",
				Group = OthersGroup,
				ParamName = "Active Politician"
			}

		};
		// Settings:
		private readonly TabPage OurTab = new TabPage("Biography");

		private GenericWithWorkgroups OurSettingsControl;
		protected internal override string PluginShortName {
			get { return "Biography"; }
		}

		protected override string PreferredTemplateName {
			get { return PluginName; }
		}

		protected override void ImportanceParameter(Importance Importance)
		{
		}
		protected internal override IGenericSettings GenericSettings {
			get { return OurSettingsControl; }
		}

		internal override bool HasReqPhotoParam {
			get { return true; }
		}

		internal override void ReqPhoto()
		{
			AddNewParamWithAYesValue("needs-photo");
		}

		// Initialisation:
		protected internal override void Initialise()
		{
			OurMenuItem = new ToolStripMenuItem("Biography Plugin");
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
			Living Living = Living.Unknown;
			bool LivingAlreadyAddedToEditSummary = false;

			StubClass();

			var _with1 = OurSettingsControl;
			foreach (ListViewItem lvi in _with1.ListView1.Items) {
				if (lvi.Checked) {
					TemplateParameters tp = (TemplateParameters)lvi.Tag;

					if (tp.Group == WorkgroupsGroups) {
						string param = tp.ParamName.ToLower().Replace(" ", "-");
						AddAndLogNewParamWithAYesValue(param + "-work-group");
						//Probably needs some reformatting
						AddEmptyParam(param + "-priority");
					} else {
						switch (tp.ParamName) {
							case "Not Living":
								Living = Living.Dead;
								break;
							case "Living":
								Living = Living.Living;
								break;
							default:
								AddAndLogNewParamWithAYesValue(tp.ParamName.ToLower().Replace(" ", "-"));
								break;
						}
					}
				}
			}

			switch (Living) {
				case Living.Living:
					if (!Template.HasYesParamLowerOrTitleCase(true, "living")) {
						if (LivingAlreadyAddedToEditSummary) {
							AddNewParamWithAYesValue("living");
						} else {
							AddAndLogNewParamWithAYesValue("living");
						}
					}
					break;
				case Living.Dead:
					if (!Template.HasYesParamLowerOrTitleCase(false, "living")) {
						Template.NewOrReplaceTemplateParm("living", "no", article, true, false, false, "", PluginShortName, true);
					}
					break;
				case Living.Unknown:
					Template.NewOrReplaceTemplateParm("living", "", article, false, false, true, "", PluginShortName, true);
					break;
			}

			var _with2 = article;
			if (_with2.Namespace == Namespace.Talk && _with2.ProcessIt && !PluginManager.BotMode) {
				// Since we're dealing with talk pages, we want a listas= even if it's the same as the
				// article title without namespace (otherwise it sorts to namespace)
				Template.NewOrReplaceTemplateParm("listas", Tools.MakeHumanCatKey(article.FullArticleTitle, article.AlteredArticleText), article, true, false, true, "", PluginShortName);
			}
		}

		/// <summary>
		/// Send the template to the plugin for preinspection
		/// </summary>
		/// <returns>False if OK, TRUE IF BAD TAG</returns>
		protected override bool TemplateFound()
		{
			var _with3 = Template;
			if (_with3.Parameters.ContainsKey("importance")) {
				_with3.Parameters.Remove("importance");
				article.ArticleHasAMinorChange();
			}
			if (_with3.Parameters.ContainsKey("priority")) {
				string priorityValue = _with3.Parameters["priority"].Value;

				foreach (KeyValuePair<string, Templating.TemplateParametersObject> kvp in _with3.Parameters) {
					if (kvp.Key.Contains("-priority") && !string.IsNullOrEmpty(kvp.Value.Value)) {
						kvp.Value.Value = priorityValue;
					}
				}

				_with3.Parameters.Remove("priority");
				article.ArticleHasAMinorChange();
			}

			return false;
		}

		protected override string WriteTemplateHeader()
		{
			string res = "{{WikiProject Biography" + Environment.Newline;

			var _with4 = Template;
			if (_with4.Parameters.ContainsKey("living")) {
				_with4.Parameters["living"].Value = _with4.Parameters["living"].Value.ToLower();
				res += "|living=" + _with4.Parameters["living"].Value + ParameterBreak;

				_with4.Parameters.Remove("living");
				// we've written this parameter; if we leave it in the collection PluginBase.TemplateWritingAndPlacement() will write it again
			}
			if (article.Namespace == Namespace.Talk) {
				res += WriteOutParameterToHeader("class");
			}

			return res;
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
