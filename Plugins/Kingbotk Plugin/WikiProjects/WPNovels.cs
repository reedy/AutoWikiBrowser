using System.Windows.Forms;
using System.Xml;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.WikiProjects
{
    internal sealed class WPNovels : PluginBase
    {
        // Settings:
        private readonly TabPage _ourTab = new TabPage(Prefix);

        private readonly GenericWithWorkgroups _ourSettingsControl;
        private const string Prefix = "Novels";

        private const string PluginName = "WikiProject Novels";

        internal WPNovels() : base("Novel|Novels|NovelsWikiProject|Novels WikiProject|WPNovels|WPNOVEL|")
        {
            // Specify alternate names only

            _ourSettingsControl = new GenericWithWorkgroups(PluginName, Prefix, true, _params);
        }

        private readonly TemplateParameters[] _params =
        {
            new TemplateParameters
            {
                StorageKey = "CrimeWG",
                Group = "",
                ParamName = "Crime"
            },
            new TemplateParameters
            {
                StorageKey = "ShortStoryWG",
                Group = "",
                ParamName = "Short Story"
            },
            new TemplateParameters
            {
                StorageKey = "SFWG",
                Group = "",
                ParamName = "SF"
            },
            new TemplateParameters
            {
                StorageKey = "AusWG",
                Group = "",
                ParamName = "Australian"
            },
            new TemplateParameters
            {
                StorageKey = "FantWG",
                Group = "",
                ParamName = "Fantasy"
            },
            new TemplateParameters
            {
                StorageKey = "19thWG",
                Group = "",
                ParamName = "19thC"
            },
            new TemplateParameters
            {
                StorageKey = "NarniaWG",
                Group = "",
                ParamName = "Narnia"
            },
            new TemplateParameters
            {
                StorageKey = "LemonyWG",
                Group = "",
                ParamName = "Lemony Snicket"
            },
            new TemplateParameters
            {
                StorageKey = "ShannaraWG",
                Group = "",
                ParamName = "Shannara"
            },
            new TemplateParameters
            {
                StorageKey = "SwordWG",
                Group = "",
                ParamName = "Sword of Truth"
            },
            new TemplateParameters
            {
                StorageKey = "TwilightWG",
                Group = "",
                ParamName = "Twilight"
            },
            new TemplateParameters
            {
                StorageKey = "OldPeerReview",
                Group = "",
                ParamName = "Old Peer Review"
            }
        };

        protected internal override string PluginShortName
        {
            get { return Prefix; }
        }

        protected override string PreferredTemplateName
        {
            get { return PluginName; }
        }

        protected override void ImportanceParameter(Importance importance)
        {
            Template.NewOrReplaceTemplateParm("importance", importance.ToString(), TheArticle, false, false);
        }

        protected internal override IGenericSettings GenericSettings
        {
            get { return _ourSettingsControl; }
        }

        internal override bool HasReqPhotoParam
        {
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
        protected override void ShowHideOurObjects(bool visible)
        {
            PluginManager.ShowHidePluginTab(_ourTab, visible);
        }

        // XML settings:
        protected internal override void ReadXML(XmlTextReader reader)
        {
            bool blnNewVal = PluginManager.XMLReadBoolean(reader, Prefix + "Enabled", Enabled);
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (blnNewVal != Enabled)
                Enabled = blnNewVal;
            // Mustn't set if the same or we get extra tabs
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
    }
}