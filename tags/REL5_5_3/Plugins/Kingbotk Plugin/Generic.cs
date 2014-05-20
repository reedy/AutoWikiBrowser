using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{
    internal sealed partial class GenericTemplateSettings : IGenericSettings
    {
        // Our name:

        private readonly string _name;
        // Enums:
        internal enum ImportanceSettingEnum
        {
            Imp,
            Pri,
            None
        }

        #region "Parameter Names"

        private string TemplateNameParm
        {
            get { return _name + "GenericTemplateName"; }
        }

        private string TemplateAlternateNamesYNParm
        {
            get { return _name + "GenericTemplateAlternateNamesYN"; }
        }

        private string TemplateAlternateNamesParm
        {
            get { return _name + "GenericTemplateAlternateNames"; }
        }

        private string TemplateImportanceParm
        {
            get { return _name + "GenericTemplateImp"; }
        }

        private string TemplateAutoStubYNParm
        {
            get { return _name + "GenericTemplateAutoStubYN"; }
        }

        private string conSkipRegexYN
        {
            get { return _name + "GenericSkipRegexYN"; }
        }

        private string conSkipRegex
        {
            get { return _name + "GenericSkipRegex"; }
        }

        private string AutoStubParm
        {
            get { return _name + "GenericAutoStub"; }
        }

        private string StubClassParm
        {
            get { return _name + "GenericStubClass"; }
        }

        #endregion

        #region "Properties"

        internal string TemplateName
        {
            get { return Tools.GetTemplateName(TemplateNameTextBox.Text); }
            set { TemplateNameTextBox.Text = value; }
        }

        internal bool HasAlternateNames
        {
            get { return HasAlternateNamesCheckBox.Checked; }
            set { HasAlternateNamesCheckBox.Checked = value; }
        }

        internal string AlternateNames
        {
            get { return AlternateNamesTextBox.Text.Trim(new[] {Convert.ToChar("|"), Convert.ToChar(" ")}); }
            set { AlternateNamesTextBox.Text = value; }
        }

        internal ImportanceSettingEnum ImportanceSetting
        {
            get
            {
                if (ImportanceCheckedListBox.CheckedIndices.Count == 0)
                {
                    return ImportanceSettingEnum.None;
                }
                return (ImportanceSettingEnum) ImportanceCheckedListBox.CheckedIndices[0];
            }
            set { ImportanceCheckedListBox.SetItemChecked((int) value, true); }
        }

        internal string SkipRegex
        {
            get { return SkipRegexTextBox.Text.Trim(); }
            set { SkipRegexTextBox.Text = value; }
        }

        internal bool SkipRegexYN
        {
            get { return SkipRegexCheckBox.Checked; }
            set { SkipRegexCheckBox.Checked = value; }
        }

        internal bool AutoStubYN
        {
            get { return AutoStubSupportYNCheckBox.Checked; }
            set { AutoStubSupportYNCheckBox.Checked = value; }
        }

        public bool StubClass
        {
            get { return StubClassCheckBox.Checked; }
            set { StubClassCheckBox.Checked = value; }
        }

        public bool AutoStub
        {
            get { return (AutoStubCheckBox.Checked & AutoStubSupportYNCheckBox.Checked); }
            set { AutoStubCheckBox.Checked = AutoStubYN && value; }
        }

        public bool StubClassModeAllowed
        {
            set { StubClassCheckBox.Enabled = value; }
        }

        #endregion

        // Initialisation and goodbye:

        internal GenericTemplateSettings(string ourPluginName)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            _name = ourPluginName;
        }

        internal void Goodbye()
        {
        }

        #region "XML interface"

        public void ReadXML(XmlTextReader reader)
        {
            AutoStub = PluginManager.XMLReadBoolean(reader, AutoStubParm, AutoStub);
            StubClass = PluginManager.XMLReadBoolean(reader, StubClassParm, StubClass);
            TemplateName = PluginManager.XMLReadString(reader, TemplateNameParm, TemplateName);
            HasAlternateNames = PluginManager.XMLReadBoolean(reader, TemplateAlternateNamesYNParm, HasAlternateNames);
            AlternateNames = PluginManager.XMLReadString(reader, TemplateAlternateNamesParm, AlternateNames);
            ImportanceSetting =
                (ImportanceSettingEnum)
                    ImportanceSettingEnum.Parse(typeof (ImportanceSettingEnum),
                        PluginManager.XMLReadString(reader, TemplateImportanceParm, ImportanceSetting.ToString()),
                        true);
            AutoStubYN = PluginManager.XMLReadBoolean(reader, TemplateAutoStubYNParm, AutoStubYN);
            SkipRegexYN = PluginManager.XMLReadBoolean(reader, conSkipRegexYN, SkipRegexYN);
            SkipRegex = PluginManager.XMLReadString(reader, conSkipRegex, SkipRegex);
        }

        public void WriteXML(XmlTextWriter writer)
        {
            writer.WriteAttributeString(TemplateNameParm, TemplateName);
            writer.WriteAttributeString(AutoStubParm, AutoStub.ToString());
            writer.WriteAttributeString(StubClassParm, StubClass.ToString());
            writer.WriteAttributeString(TemplateAlternateNamesYNParm, HasAlternateNames.ToString());
            writer.WriteAttributeString(TemplateAlternateNamesParm, AlternateNames);
            writer.WriteAttributeString(TemplateImportanceParm, ImportanceSetting.ToString());
            writer.WriteAttributeString(TemplateAutoStubYNParm, AutoStubYN.ToString());
            writer.WriteAttributeString(conSkipRegexYN, SkipRegexYN.ToString());
            writer.WriteAttributeString(conSkipRegex, SkipRegex);
        }

        internal void Reset()
        {
            TemplateName = "";
            AutoStub = false;
            StubClass = false;
            HasAlternateNames = false;
            AlternateNames = "";
            ImportanceSetting = ImportanceSettingEnum.None;
            AutoStubYN = false;
            SkipRegexYN = false;
            SkipRegex = "";
        }

        void IGenericSettings.XMLReset()
        {
            Reset();
        }

        #endregion

        #region "Event handlers"

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenENArticleInBrowser("Kingbotk/Plugin/Generic_WikiProject_templates", true);
        }

        private void ImportanceCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            AssessmentForm.AllowOnlyOneCheckedItem(sender, e);
        }

        private void HasAlternateNamesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AlternateNamesTextBox.Enabled = HasAlternateNamesCheckBox.Checked;
        }

        private void AutoStubSupportYNCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoStubSupportYNCheckBox.Checked)
            {
                AutoStubCheckBox.Enabled = true;
            }
            else
            {
                AutoStubCheckBox.Checked = false;
                AutoStubCheckBox.Enabled = false;
            }
        }

        private void SkipRegexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SkipRegexTextBox.Enabled = SkipRegexCheckBox.Checked;
        }

        private void TemplateNameTextBox_TextChanged(object sender, EventArgs e)
        {
            GetRedirectsButton.Enabled = !(string.IsNullOrEmpty(TemplateName));
        }

        #endregion
    }

    internal sealed class GenericTemplatePlugin : PluginBase, IGenericTemplatePlugin, IDisposable
    {
        // Objects:
        private TabPage _ourTab;
        private GenericTemplateSettings _ourSettingsControl;
        private ToolStripMenuItem _deleteMeMenuItem = new ToolStripMenuItem("Delete");
        // Settings:
        private readonly string _ourName;

        private string conEnabled
        {
            get { return _ourName + "GenericEnabled"; }
        }

        // Regex:

        private Regex SkipRegex;
        // Initialisation:
        internal GenericTemplatePlugin(string myName) : base(true)
        {
            _ourSettingsControl = new GenericTemplateSettings(myName);
            _ourTab = new TabPage(myName);
            _ourName = myName;

            // Keep track of changing configuration by suscribing to various events:
            _ourSettingsControl.SkipRegexCheckBox.CheckedChanged += SkipRegexChanged;
            _ourSettingsControl.SkipRegexTextBox.TextChanged += SkipRegexChanged;
            _ourSettingsControl.TemplateNameTextBox.TextChanged += TemplateNamesChanged;
            _ourSettingsControl.HasAlternateNamesCheckBox.CheckedChanged += TemplateNamesChanged;
            _ourSettingsControl.AlternateNamesTextBox.TextChanged += TemplateNamesChanged;
            //AddHandler OurSettingsControl.AlternateNamesTextBox.EnabledChanged, AddressOf TemplateNamesChanged ' CheckedChanged should covert this
            _ourSettingsControl.PropertiesButton.Click += PropertiesButtonClick;
            _ourSettingsControl.GetRedirectsButton.Click += GetRedirectsButtonClick;
        }

        protected internal override void Initialise()
        {
            OurMenuItem = new ToolStripMenuItem(PluginShortName);
            InitialiseBase();
            // must set menu item object first
            _ourTab.UseVisualStyleBackColor = true;
            _ourSettingsControl.Reset();
            _ourTab.Controls.Add(_ourSettingsControl);
            _deleteMeMenuItem.ToolTipText = "Delete the " + PluginShortName + " plugin";
            _deleteMeMenuItem.Click += DeleteMeMenuItem_Click;
            OurMenuItem.DropDownItems.Add(_deleteMeMenuItem);
        }

        // Properties:
        protected internal override bool AmReady
        {
            get
            {
                if (string.IsNullOrEmpty(_ourSettingsControl.TemplateName))
                    return false;
                if (MainRegex == null)
                    return false;
                if (SecondChanceRegex == null)
                    return false;
                // else:
                return base.AmReady;
            }
        }

        protected internal override bool AmGeneric
        {
            get { return true; }
        }

        protected internal override string PluginShortName
        {
            get { return "Generic (" + _ourName + ")"; }
        }

        protected override string PreferredTemplateName
        {
            get { return _ourSettingsControl.TemplateName; }
        }

        protected override void ImportanceParameter(Importance importance)
        {
            switch (_ourSettingsControl.ImportanceSetting)
            {
                case GenericTemplateSettings.ImportanceSettingEnum.Imp:
                    Template.NewOrReplaceTemplateParm("importance", importance.ToString(), TheArticle, false, false);
                    break;
                case GenericTemplateSettings.ImportanceSettingEnum.Pri:
                    Template.NewOrReplaceTemplateParm("priority", importance.ToString(), TheArticle, false, false);
                    break;
                    // Case GenericTemplateSettings.ImportanceSettingEnum.None ' do nothing
            }
        }

        protected internal override IGenericSettings GenericSettings
        {
            get { return _ourSettingsControl; }
        }

        // Article processing:
        protected override bool SkipIfContains()
        {
            if ((SkipRegex != null))
            {
                try
                {
                    return (SkipRegex.Matches(TheArticle.AlteredArticleText).Count > 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error processing skip regular expression: " + Environment.NewLine + Environment.NewLine + ex,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    PluginManager.StopAWB();
                }
            }
            return false;
        }

        protected override bool TemplateFound()
        {
            // Nothing to do here
            return false;
        }

        protected override string WriteTemplateHeader()
        {
            string res = "{{" + PreferredTemplateName + WriteOutParameterToHeader("class");

            switch (_ourSettingsControl.ImportanceSetting)
            {
                case GenericTemplateSettings.ImportanceSettingEnum.Imp:
                    res += WriteOutParameterToHeader("importance");
                    break;
                case GenericTemplateSettings.ImportanceSettingEnum.Pri:
                    res += WriteOutParameterToHeader("priority");
                    break;
            }

            return res;
        }

        protected override void ProcessArticleFinish()
        {
            StubClass();
        }

        //User interface:
        protected override void ShowHideOurObjects(bool visible)
        {
            PluginManager.ShowHidePluginTab(_ourTab, visible);
        }

        // XML settings:
        protected internal override void ReadXML(XmlTextReader reader)
        {
            bool blnNewVal = PluginManager.XMLReadBoolean(reader, conEnabled, Enabled);
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
            writer.WriteAttributeString(conEnabled, Enabled.ToString());
            _ourSettingsControl.WriteXML(writer);
        }

        //' These do nothing because generic templates already have a AlternateNames XML property
        internal override void ReadXMLRedirects(XmlTextReader reader)
        {
        }

        internal override void WriteXMLRedirects(XmlTextWriter writer)
        {
        }

        // Our interface:
        public void Goodbye()
        {
            Dispose();
        }

        public string GenericTemplateKey
        {
            get { return _ourName; }
        }

        // Settings control event handlers:
        private void SkipRegexChanged(object sender, EventArgs e)
        {
            if (_ourSettingsControl.SkipRegexYN == false || string.IsNullOrEmpty(_ourSettingsControl.SkipRegex))
            {
                SkipRegex = null;
            }
            else
            {
                SkipRegex = new Regex(_ourSettingsControl.SkipRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }

        private void PropertiesButtonClick(object sender, EventArgs e)
        {
            using (GenericTemplatePropertiesForm frm = new GenericTemplatePropertiesForm())
            {
                frm.AmIReadyLabel.Text = AmReady
                    ? "Generic Template Plugin is ready"
                    : "Generic Template Plugin is not properly configured";

                GenericTemplatePropertiesForm.DoRegexTextBox(frm.MainRegexTextBox, MainRegex);
                GenericTemplatePropertiesForm.DoRegexTextBox(frm.PreferredTemplateNameRegexTextBox,
                    PreferredTemplateNameRegex);
                GenericTemplatePropertiesForm.DoRegexTextBox(frm.SecondChanceRegexTextBox, SecondChanceRegex);
                GenericTemplatePropertiesForm.DoRegexTextBox(frm.SkipRegexTextBox, SkipRegex);

                frm.HasAltNamesLabel.Text += HasAlternateNames.ToString();

                frm.NameLabel.Text += _ourSettingsControl.TemplateName;

                if (_ourSettingsControl.SkipRegexYN)
                {
                    if (string.IsNullOrEmpty(_ourSettingsControl.SkipRegex))
                    {
                        frm.SkipLabel.Text += bool.FalseString;
                    }
                    else
                    {
                        frm.SkipLabel.Text += bool.TrueString;
                    }
                }
                else
                {
                    frm.SkipLabel.Text += bool.FalseString;
                }

                switch (_ourSettingsControl.ImportanceSetting)
                {
                    case GenericTemplateSettings.ImportanceSettingEnum.Imp:
                        frm.ImportanceLabel.Text += "importance=";
                        break;
                    case GenericTemplateSettings.ImportanceSettingEnum.None:
                        frm.ImportanceLabel.Text += "<none>";
                        break;
                    case GenericTemplateSettings.ImportanceSettingEnum.Pri:
                        frm.ImportanceLabel.Text += "priority=";
                        break;
                }

                if (_ourSettingsControl.AutoStubYN)
                {
                    frm.AutoStubLabel.Text += "auto=yes";
                }
                else
                {
                    frm.AutoStubLabel.Text += "<none>";
                }

                frm.ShowDialog();
            }
        }

        private void DeleteMeMenuItem_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Delete the " + _ourName + " plugin?", "Delete?", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                PluginManager.DeleteGenericPlugin(this, this);
            }
        }

        private void GetRedirectsButtonClick(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Get the redirects from Wikipedia? Note: This may take a while.", "Get from Wikipedia?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
                DialogResult.Yes)
            {
                try
                {
                    _ourSettingsControl.AlternateNames =
                        ConvertRedirectsToString(GetRedirects(_ourSettingsControl.TemplateName));
                    _ourSettingsControl.HasAlternateNamesCheckBox.Checked =
                        !(string.IsNullOrEmpty(_ourSettingsControl.AlternateNames));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Whoops, we caught an error when trying to get the redirects from Wikipedia." +
                                    Environment.NewLine + Environment.NewLine + "The error was:" + ex.Message +
                                    Environment.NewLine + Environment.NewLine +
                                    "Depending on the error you might want to " +
                                    "try again by repressing Get. If this shouldn't have happened please report it to the authors.");
                }
            }
        }

        // Misc:
        internal override bool HasReqPhotoParam
        {
            get { return false; }
        }

        internal override void ReqPhoto()
        {
        }

        #region "IDisposable"

        public void Dispose()
        {
            Dispose(true);
        }

        // To detect redundant calls
        private bool disposed;
        // This procedure is where the actual cleanup occurs
        private void Dispose(bool disposing)
        {
            try
            {
                // Exit now if the object has already been disposed
                if (disposed)
                    return;

                if (disposing)
                {
                    // The object is being disposed, not finalized.
                    // It is safe to access other objects (other than the mybase object)
                    // only from inside this block
                    _ourSettingsControl.SkipRegexCheckBox.CheckedChanged -= SkipRegexChanged;
                    _ourSettingsControl.SkipRegexTextBox.TextChanged -= SkipRegexChanged;
                    _ourSettingsControl.TemplateNameTextBox.TextChanged -= TemplateNamesChanged;
                    _ourSettingsControl.HasAlternateNamesCheckBox.CheckedChanged -= TemplateNamesChanged;
                    _ourSettingsControl.AlternateNamesTextBox.TextChanged -= TemplateNamesChanged;
                    _ourSettingsControl.PropertiesButton.Click -= PropertiesButtonClick;
                    ShowHideOurObjects(false);

                    _ourTab.Dispose();

                    _ourSettingsControl.Goodbye();
                    _ourSettingsControl.Dispose();

                    PluginManager.AWBForm.PluginsToolStripMenuItem.DropDownItems.Remove(OurMenuItem);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                // Perform cleanup that has to be executed in either case:
                _ourTab = null;
                OurMenuItem = null;
                TheArticle = null;
                Template = null;
                MainRegex = null;
                SecondChanceRegex = null;
                PreferredTemplateNameRegex = null;
                _ourTab = null;
                _ourSettingsControl = null;
                _deleteMeMenuItem = null;
                SkipRegex = null;

                // Remember that this object has been disposed of:
                disposed = true;
            }
        }

        #endregion

        /// <summary>
        /// This is called when the contents of TemplateNameTextBox or AlternateNamesTextBox changes, or when HasAlternateNamesCheckBox is (un)checked
        /// </summary>
        private void TemplateNamesChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_ourSettingsControl.TemplateName))
            {
                MainRegex = null;
                PreferredTemplateNameRegex = null;
                SecondChanceRegex = null;
            }
            else
            {
                GotNewAlternateNamesString(_ourSettingsControl.HasAlternateNames
                    ? _ourSettingsControl.AlternateNames
                    : "");
            }
        }
    }
}

namespace AutoWikiBrowser.Plugins.Kingbotk
{
    internal interface IGenericTemplatePlugin
    {
        void Goodbye();
        string GenericTemplateKey { get; }
    }
}