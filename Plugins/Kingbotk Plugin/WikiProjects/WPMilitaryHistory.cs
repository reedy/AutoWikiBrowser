/*
Copyright (C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
Copyright (C) 2008 Sam Reed (Reedy) http://www.reedyboy.net/

This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System.Windows.Forms;
using System.Xml;

namespace AutoWikiBrowser.Plugins.Kingbotk.WikiProjects
{
    internal sealed class WPMilitaryHistory : PluginBase
    {
        private const string PluginName = "WikiProject Military history";
        private const string TemplateName = "WikiProject Military history";

        // Initialisation:
        internal WPMilitaryHistory() : base("WikiProject Military History|MILHIST|Milhist|WP Military History|WikiProject MILHIST|WPMILHIST|WPMilhist|MilHist|Mil Hist")
        {
            // Specify alternate names only

            _ourSettingsControl = new GenericWithWorkgroups(TemplateName, PluginName, false, _params);
        }

        // Settings:
        private readonly TabPage _ourTab = new TabPage("MilHist");

        private readonly GenericWithWorkgroups _ourSettingsControl;

        protected internal override string PluginShortName
        {
            get { return "Military History"; }
        }

        protected override string PreferredTemplateName
        {
            get { return TemplateName; }
        }

        protected internal override IGenericSettings GenericSettings
        {
            get { return _ourSettingsControl; }
        }

        internal override bool HasReqPhotoParam
        {
            get { return false; }
        }

        internal override void ReqPhoto()
        {
        }

        private const string PeriodsAndConflictsGroup = "Periods and Conflicts";
        private const string GeneralGroup = "General Task Forces";

        private const string NationsGroup = "Nations and Regions";

        private readonly TemplateParameters[] _params =
        {
            new TemplateParameters
            {
                StorageKey = "ACW",
                Group = PeriodsAndConflictsGroup,
                ParamName = "ACW"
            },
            new TemplateParameters
            {
                StorageKey = "ARW",
                Group = PeriodsAndConflictsGroup,
                ParamName = "ARW"
            },
            new TemplateParameters
            {
                StorageKey = "Classic",
                Group = PeriodsAndConflictsGroup,
                ParamName = "Classical"
            },
            new TemplateParameters
            {
                StorageKey = "Crusades",
                Group = PeriodsAndConflictsGroup,
                ParamName = "Crusades"
            },
            new TemplateParameters
            {
                StorageKey = "EarlyModern",
                Group = PeriodsAndConflictsGroup,
                ParamName = "Early-Modern"
            },
            new TemplateParameters
            {
                StorageKey = "Medieval",
                Group = PeriodsAndConflictsGroup,
                ParamName = "Medieval"
            },
            new TemplateParameters
            {
                StorageKey = "Muslim",
                Group = PeriodsAndConflictsGroup,
                ParamName = "Muslim"
            },
            new TemplateParameters
            {
                StorageKey = "Napol",
                Group = PeriodsAndConflictsGroup,
                ParamName = "Napoleonic"
            },
            new TemplateParameters
            {
                StorageKey = "PostCW",
                Group = PeriodsAndConflictsGroup,
                ParamName = "Post-Cold-War"
            },
            new TemplateParameters
            {
                StorageKey = "WWI",
                Group = PeriodsAndConflictsGroup,
                ParamName = "WWI"
            },
            new TemplateParameters
            {
                StorageKey = "WWII",
                Group = PeriodsAndConflictsGroup,
                ParamName = "WWII"
            },
            new TemplateParameters
            {
                StorageKey = "Air",
                Group = GeneralGroup,
                ParamName = "Aviation"
            },
            new TemplateParameters
            {
                StorageKey = "Biography",
                Group = GeneralGroup,
                ParamName = "Biography"
            },
            {
                StorageKey = "CultTradHer",
                Group = GeneralGroup,
                ParamName = "Culture"
            },
            new TemplateParameters
            {
                StorageKey = "Films",
                Group = GeneralGroup,
                ParamName = "Films"
            },
            new TemplateParameters
            {
                StorageKey = "Fortifications",
                Group = GeneralGroup,
                ParamName = "Fortifications"
            },
            new TemplateParameters
            {
                StorageKey = "Historiography",
                Group = GeneralGroup,
                ParamName = "Historiography"
            },
            new TemplateParameters
            {
                StorageKey = "Intel",
                Group = GeneralGroup,
                ParamName = "Intel"
            },
            new TemplateParameters
            {
                StorageKey = "LandVech",
                Group = GeneralGroup,
                ParamName = "Land Vehicles"
            },
            {
                StorageKey = "Culture",
                Group = GeneralGroup,
                ParamName = "Culture"
            },
            new TemplateParameters
            {
                StorageKey = "Logandmed",
                Group = GeneralGroup,
                ParamName = "Logistics"
            },
            new TemplateParameters
            {
                StorageKey = "Memorial",
                Group = GeneralGroup,
                ParamName = "Memorials"
            },
            new TemplateParameters
            {
                StorageKey = "National",
                Group = GeneralGroup,
                ParamName = "Nationals"
            },
            new TemplateParameters
            {
                StorageKey = "Science",
                Group = GeneralGroup,
                ParamName = "Science"
            },
            new TemplateParameters
            {
                StorageKey = "Tech",
                Group = GeneralGroup,
                ParamName = "Technology"
            },
            new TemplateParameters
            {
                StorageKey = "Weapon",
                Group = GeneralGroup,
                ParamName = "Weaponry"
            },
            new TemplateParameters
            {
                StorageKey = "NTF",
                Group = NationsGroup,
                ParamName = "No Task Force"
            },
            new TemplateParameters
            {
                StorageKey = "African",
                Group = NationsGroup,
                ParamName = "Africa"
            },
            new TemplateParameters
            {
                StorageKey = "Asian",
                Group = NationsGroup,
                ParamName = "Asian"
            },
            new TemplateParameters
            {
                StorageKey = "Aus",
                Group = NationsGroup,
                ParamName = "Australia"
            },
            new TemplateParameters
            {
                StorageKey = "Balkan",
                Group = NationsGroup,
                ParamName = "Balkan"
            },
            new TemplateParameters
            {
                StorageKey = "Baltic",
                Group = NationsGroup,
                ParamName = "Baltic"
            },
            new TemplateParameters
            {
                StorageKey = "Brit",
                Group = NationsGroup,
                ParamName = "British"
            },
            new TemplateParameters
            {
                StorageKey = "Byzan",
                Group = NationsGroup,
                ParamName = "Byzantine"
            },
            {
                StorageKey = "Canuck",
                Group = NationsGroup,
                ParamName = "Canadian"
            },
            new TemplateParameters
            {
                StorageKey = "China",
                Group = NationsGroup,
                ParamName = "Chinese"
            },
            new TemplateParameters
            {
                StorageKey = "Dutch",
                Group = NationsGroup,
                ParamName = "Dutch"
            },
            new TemplateParameters
            {
                StorageKey = "European",
                Group = NationsGroup,
                ParamName = "European"
            },
            new TemplateParameters
            {
                StorageKey = "French",
                Group = NationsGroup,
                ParamName = "French"
            },
            new TemplateParameters
            {
                StorageKey = "German",
                Group = NationsGroup,
                ParamName = "German"
            },
            new TemplateParameters
            {
                StorageKey = "India",
                Group = NationsGroup,
                ParamName = "Indian"
            },
            new TemplateParameters
            {
                StorageKey = "Italy",
                Group = NationsGroup,
                ParamName = "Italian"
            },
            new TemplateParameters
            {
                StorageKey = "Japan",
                Group = NationsGroup,
                ParamName = "Japanese"
            },
            new TemplateParameters
            {
                StorageKey = "Korean",
                Group = NationsGroup,
                ParamName = "Korean"
            },
            new TemplateParameters
            {
                StorageKey = "Lebanese",
                Group = NationsGroup,
                ParamName = "Lebanese"
            },
            new TemplateParameters
            {
                StorageKey = "MidEast",
                Group = NationsGroup,
                ParamName = "Middle-Eastern"
            },
            new TemplateParameters
            {
                StorageKey = "NZ",
                Group = NationsGroup,
                ParamName = "New Zealand"
            },
            new TemplateParameters
            {
                StorageKey = "Nordic",
                Group = NationsGroup,
                ParamName = "Nordic"
            },
            new TemplateParameters
            {
                StorageKey = "NorthAm",
                Group = NationsGroup,
                ParamName = "North-American"
            },
            new TemplateParameters
            {
                StorageKey = "Ottoman",
                Group = NationsGroup,
                ParamName = "Ottoman"
            },
            new TemplateParameters
            {
                StorageKey = "Poland",
                Group = NationsGroup,
                ParamName = "Polish"
            },
            new TemplateParameters
            {
                StorageKey = "Romanian",
                Group = NationsGroup,
                ParamName = "Romanian"
            },
            new TemplateParameters
            {
                StorageKey = "Russian",
                Group = NationsGroup,
                ParamName = "Russian"
            },
            new TemplateParameters
            {
                StorageKey = "Spanish",
                Group = NationsGroup,
                ParamName = "Spanish"
            },
            new TemplateParameters
            {
                StorageKey = "SAmerican",
                Group = NationsGroup,
                ParamName = "S American"
            },
            new TemplateParameters
            {
                StorageKey = "SEAsian",
                Group = NationsGroup,
                ParamName = "SE Asian"
            },
            new TemplateParameters
            {
                StorageKey = "Taiwanese",
                Group = NationsGroup,
                ParamName = "Taiwanese"
            },
            new TemplateParameters
            {
                StorageKey = "ThreeKingdoms",
                Group = NationsGroup,
                ParamName = "Three-Kingdoms"
            },
            new TemplateParameters
            {
                StorageKey = "US",
                Group = NationsGroup,
                ParamName = "US"
            }
        };

        protected internal override void Initialise()
        {
            OurMenuItem = new ToolStripMenuItem("Military History Plugin");
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
            foreach (ListViewItem lvi in _ourSettingsControl.ListView1.Items)
            {
                if (lvi.Checked)
                {
                    TemplateParameters tp = (TemplateParameters) lvi.Tag;
                    AddAndLogNewParamWithAYesValue(tp.ParamName.ToLower().Replace(" ", "-"));
                    // Probably needs some reformatting
                }
            }
            if (Template.Parameters.ContainsKey("importance"))
            {
                Template.Parameters.Remove("importance");
                TheArticle.ArticleHasAMajorChange();
            }

            if (Template.Parameters.ContainsKey("auto"))
            {
                Template.Parameters.Remove("auto");
                TheArticle.ArticleHasAMajorChange();
            }
        }

        private const string MedievalTaskForce = "Medieval-task-force";

        protected override bool TemplateFound()
        {
            const string conMiddleAges = "Middle-Ages-task-force";

            if (Template.Parameters.ContainsKey(conMiddleAges))
            {
                if (Template.Parameters[conMiddleAges].Value.ToLower() == "yes")
                {
                    Template.NewOrReplaceTemplateParm(MedievalTaskForce, "yes", TheArticle, false, false, false, "");
                    TheArticle.DoneReplacement(conMiddleAges, MedievalTaskForce);
                }

                Template.Parameters.Remove(conMiddleAges);
                TheArticle.ArticleHasAMinorChange();
            }

            return false;
        }

        protected override string WriteTemplateHeader()
        {
            return "{{" + PluginName + WriteOutParameterToHeader("class");
        }

        // User interface:
        protected override void ShowHideOurObjects(bool visible)
        {
            PluginManager.ShowHidePluginTab(_ourTab, visible);
        }

        // XML settings:
        protected internal override void ReadXML(XmlTextReader reader)
        {
            bool blnNewVal = PluginManager.XMLReadBoolean(reader, PluginName + "Enabled", Enabled);
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (blnNewVal != Enabled)
            {
                Enabled = blnNewVal;
                // Mustn't set if the same or we get extra tabs
            }

            _ourSettingsControl.ReadXML(reader);
        }

        protected internal override void Reset()
        {
            _ourSettingsControl.Reset();
        }

        protected internal override void WriteXML(XmlTextWriter writer)
        {
            writer.WriteAttributeString(PluginName + "Enabled", Enabled.ToString());
            _ourSettingsControl.WriteXML(writer);
        }
    }
}
