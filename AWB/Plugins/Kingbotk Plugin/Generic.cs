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

        private readonly string mName;
        // Enums:
        internal enum ImportanceSettingEnum
        {
            Imp,
            Pri,
            None
        }

        #region "Parameter Names"

        private string conTemplateNameParm
        {
            get { return mName + "GenericTemplateName"; }
        }

        private string conTemplateAlternateNamesYNParm
        {
            get { return mName + "GenericTemplateAlternateNamesYN"; }
        }

        private string conTemplateAlternateNamesParm
        {
            get { return mName + "GenericTemplateAlternateNames"; }
        }

        private string conTemplateImportanceParm
        {
            get { return mName + "GenericTemplateImp"; }
        }

        private string conTemplateAutoStubYNParm
        {
            get { return mName + "GenericTemplateAutoStubYN"; }
        }

        private string conSkipRegexYN
        {
            get { return mName + "GenericSkipRegexYN"; }
        }

        private string conSkipRegex
        {
            get { return mName + "GenericSkipRegex"; }
        }

        private string conAutoStubParm
        {
            get { return mName + "GenericAutoStub"; }
        }

        private string conStubClassParm
        {
            get { return mName + "GenericStubClass"; }
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

        internal GenericTemplateSettings(string OurPluginName)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            mName = OurPluginName;
        }

        internal void Goodbye()
        {
        }

        #region "XML interface"

        public void ReadXML(XmlTextReader Reader)
        {
            AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub);
            StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass);
            TemplateName = PluginManager.XMLReadString(Reader, conTemplateNameParm, TemplateName);
            HasAlternateNames = PluginManager.XMLReadBoolean(Reader, conTemplateAlternateNamesYNParm, HasAlternateNames);
            AlternateNames = PluginManager.XMLReadString(Reader, conTemplateAlternateNamesParm, AlternateNames);
            ImportanceSetting =
                (ImportanceSettingEnum)
                    ImportanceSettingEnum.Parse(typeof (ImportanceSettingEnum),
                        PluginManager.XMLReadString(Reader, conTemplateImportanceParm, ImportanceSetting.ToString()),
                        true);
            AutoStubYN = PluginManager.XMLReadBoolean(Reader, conTemplateAutoStubYNParm, AutoStubYN);
            SkipRegexYN = PluginManager.XMLReadBoolean(Reader, conSkipRegexYN, SkipRegexYN);
            SkipRegex = PluginManager.XMLReadString(Reader, conSkipRegex, SkipRegex);
        }

        public void WriteXML(XmlTextWriter Writer)
        {
            var _with1 = Writer;
            _with1.WriteAttributeString(conTemplateNameParm, TemplateName);
            _with1.WriteAttributeString(conAutoStubParm, AutoStub.ToString());
            _with1.WriteAttributeString(conStubClassParm, StubClass.ToString());
            _with1.WriteAttributeString(conTemplateAlternateNamesYNParm, HasAlternateNames.ToString());
            _with1.WriteAttributeString(conTemplateAlternateNamesParm, AlternateNames);
            _with1.WriteAttributeString(conTemplateImportanceParm, ImportanceSetting.ToString());
            _with1.WriteAttributeString(conTemplateAutoStubYNParm, AutoStubYN.ToString());
            _with1.WriteAttributeString(conSkipRegexYN, SkipRegexYN.ToString());
            _with1.WriteAttributeString(conSkipRegex, SkipRegex);
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
        private TabPage OurTab;
        private GenericTemplateSettings OurSettingsControl;
        private ToolStripMenuItem DeleteMeMenuItem = new ToolStripMenuItem("Delete");
        // Settings:
        private readonly string OurName;

        private string conEnabled
        {
            get { return OurName + "GenericEnabled"; }
        }

        // Regex:

        private Regex SkipRegex;
        // Initialisation:
        internal GenericTemplatePlugin(string MyName) : base(true)
        {
            OurSettingsControl = new GenericTemplateSettings(MyName);
            OurTab = new TabPage(MyName);
            OurName = MyName;

            // Keep track of changing configuration by suscribing to various events:
            OurSettingsControl.SkipRegexCheckBox.CheckedChanged += SkipRegexChanged;
            OurSettingsControl.SkipRegexTextBox.TextChanged += SkipRegexChanged;
            OurSettingsControl.TemplateNameTextBox.TextChanged += TemplateNamesChanged;
            OurSettingsControl.HasAlternateNamesCheckBox.CheckedChanged += TemplateNamesChanged;
            OurSettingsControl.AlternateNamesTextBox.TextChanged += TemplateNamesChanged;
            //AddHandler OurSettingsControl.AlternateNamesTextBox.EnabledChanged, AddressOf TemplateNamesChanged ' CheckedChanged should covert this
            OurSettingsControl.PropertiesButton.Click += PropertiesButtonClick;
            OurSettingsControl.GetRedirectsButton.Click += GetRedirectsButtonClick;
        }

        protected internal override void Initialise()
        {
            OurMenuItem = new ToolStripMenuItem(PluginShortName);
            InitialiseBase();
            // must set menu item object first
            OurTab.UseVisualStyleBackColor = true;
            OurSettingsControl.Reset();
            OurTab.Controls.Add(OurSettingsControl);
            DeleteMeMenuItem.ToolTipText = "Delete the " + PluginShortName + " plugin";
            DeleteMeMenuItem.Click += DeleteMeMenuItem_Click;
            OurMenuItem.DropDownItems.Add(DeleteMeMenuItem);
        }

        // Properties:
        protected internal override bool IAmReady
        {
            get
            {
                if (string.IsNullOrEmpty(OurSettingsControl.TemplateName))
                    return false;
                if (MainRegex == null)
                    return false;
                if (SecondChanceRegex == null)
                    return false;
                // else:
                return base.IAmReady;
            }
        }

        protected internal override bool IAmGeneric
        {
            get { return true; }
        }

        protected internal override string PluginShortName
        {
            get { return "Generic (" + OurName + ")"; }
        }

        protected override string PreferredTemplateName
        {
            get { return OurSettingsControl.TemplateName; }
        }

        protected override void ImportanceParameter(Importance Importance)
        {
            switch (OurSettingsControl.ImportanceSetting)
            {
                case GenericTemplateSettings.ImportanceSettingEnum.Imp:
                    Template.NewOrReplaceTemplateParm("importance", Importance.ToString(), TheArticle, false, false);
                    break;
                case GenericTemplateSettings.ImportanceSettingEnum.Pri:
                    Template.NewOrReplaceTemplateParm("priority", Importance.ToString(), TheArticle, false, false);
                    break;
                    // Case GenericTemplateSettings.ImportanceSettingEnum.None ' do nothing
            }
        }

        protected internal override IGenericSettings GenericSettings
        {
            get { return OurSettingsControl; }
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

            switch (OurSettingsControl.ImportanceSetting)
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
            PluginManager.ShowHidePluginTab(OurTab, visible);
        }

        // XML settings:
        protected internal override void ReadXML(XmlTextReader Reader)
        {
            bool blnNewVal = PluginManager.XMLReadBoolean(Reader, conEnabled, Enabled);
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (blnNewVal != Enabled)
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
            Writer.WriteAttributeString(conEnabled, Enabled.ToString());
            OurSettingsControl.WriteXML(Writer);
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
            get { return OurName; }
        }

        // Settings control event handlers:
        private void SkipRegexChanged(object sender, EventArgs e)
        {
            if (OurSettingsControl.SkipRegexYN == false || string.IsNullOrEmpty(OurSettingsControl.SkipRegex))
            {
                SkipRegex = null;
            }
            else
            {
                SkipRegex = new Regex(OurSettingsControl.SkipRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }

        private void PropertiesButtonClick(object sender, EventArgs e)
        {
            using (GenericTemplatePropertiesForm frm = new GenericTemplatePropertiesForm())
            {
                frm.AmIReadyLabel.Text = IAmReady
                    ? "Generic Template Plugin is ready"
                    : "Generic Template Plugin is not properly configured";

                GenericTemplatePropertiesForm.DoRegexTextBox(frm.MainRegexTextBox, MainRegex);
                GenericTemplatePropertiesForm.DoRegexTextBox(frm.PreferredTemplateNameRegexTextBox,
                    PreferredTemplateNameRegex);
                GenericTemplatePropertiesForm.DoRegexTextBox(frm.SecondChanceRegexTextBox, SecondChanceRegex);
                GenericTemplatePropertiesForm.DoRegexTextBox(frm.SkipRegexTextBox, SkipRegex);

                frm.HasAltNamesLabel.Text += HasAlternateNames.ToString();

                var _with3 = OurSettingsControl;
                frm.NameLabel.Text += _with3.TemplateName;

                if (_with3.SkipRegexYN)
                {
                    if (string.IsNullOrEmpty(_with3.SkipRegex))
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

                switch (_with3.ImportanceSetting)
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

                if (_with3.AutoStubYN)
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
                MessageBox.Show("Delete the " + OurName + " plugin?", "Delete?", MessageBoxButtons.YesNo,
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
                    OurSettingsControl.AlternateNames =
                        ConvertRedirectsToString(GetRedirects(OurSettingsControl.TemplateName));
                    OurSettingsControl.HasAlternateNamesCheckBox.Checked =
                        !(string.IsNullOrEmpty(OurSettingsControl.AlternateNames));
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
                    OurSettingsControl.SkipRegexCheckBox.CheckedChanged -= SkipRegexChanged;
                    OurSettingsControl.SkipRegexTextBox.TextChanged -= SkipRegexChanged;
                    OurSettingsControl.TemplateNameTextBox.TextChanged -= TemplateNamesChanged;
                    OurSettingsControl.HasAlternateNamesCheckBox.CheckedChanged -= TemplateNamesChanged;
                    OurSettingsControl.AlternateNamesTextBox.TextChanged -= TemplateNamesChanged;
                    OurSettingsControl.PropertiesButton.Click -= PropertiesButtonClick;
                    ShowHideOurObjects(false);

                    OurTab.Dispose();

                    OurSettingsControl.Goodbye();
                    OurSettingsControl.Dispose();

                    PluginManager.AWBForm.PluginsToolStripMenuItem.DropDownItems.Remove(OurMenuItem);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                // Perform cleanup that has to be executed in either case:
                OurTab = null;
                OurMenuItem = null;
                TheArticle = null;
                Template = null;
                MainRegex = null;
                SecondChanceRegex = null;
                PreferredTemplateNameRegex = null;
                OurTab = null;
                OurSettingsControl = null;
                DeleteMeMenuItem = null;
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
            var _with4 = OurSettingsControl;
            if (string.IsNullOrEmpty(_with4.TemplateName))
            {
                MainRegex = null;
                PreferredTemplateNameRegex = null;
                SecondChanceRegex = null;
            }
            else
            {
                GotNewAlternateNamesString(_with4.HasAlternateNames ? _with4.AlternateNames : "");
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