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
using WikiFunctions.Logging.Uploader;
using WikiFunctions.Plugin;
//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

// Manual assessment:
using WikiFunctions.API;

namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
{
	internal sealed class Assessments : IDisposable
	{


		internal const string conMe = "Wikipedia Assessments Plugin";
		// Objects:
		private List<CheckBox> AWBCleanupCheckboxes = new List<CheckBox>();
		private PluginSettingsControl PluginSettings;

		private StateClass State = new StateClass();
		// New:
		internal Assessments(PluginSettingsControl vPluginSettings)
		{
			PluginSettings = vPluginSettings;

			// Get a reference to the cleanup checkboxes:
			foreach (Control ctl in PluginManager.AWBForm.OptionsTab.Controls["groupBox6"].Controls) {
				if (object.ReferenceEquals(ctl.GetType(), typeof(CheckBox)))
					AWBCleanupCheckboxes.Add((CheckBox)ctl);
			}
			ToggleAWBCleanup(PluginSettings.Cleanup);

			PluginSettings.CleanupCheckBox.CheckedChanged += CleanupCheckBox_CheckedChanged;
			PluginManager.AWBForm.TheSession.StateChanged += EditorStatusChanged;
			PluginManager.AWBForm.SaveButton.Click += Save_Click;
			PluginManager.AWBForm.SkipButton.Click += Skip_Click;
		}

		#region "IDisposable"
			// To detect redundant calls
		private bool disposed;

		// This procedure is where the actual cleanup occurs
		private void Dispose(bool disposing)
		{
			// Exit now if the object has already been disposed
			if (disposed)
				return;

			if (disposing) {
				// The object is being disposed, not finalized.
				// It is safe to access other objects (other than the mybase object)
				// only from inside this block
				PluginSettings.CleanupCheckBox.CheckedChanged -= CleanupCheckBox_CheckedChanged;
				PluginManager.AWBForm.TheSession.StateChanged -= EditorStatusChanged;
				PluginManager.AWBForm.SaveButton.Click -= Save_Click;
				PluginManager.AWBForm.SkipButton.Click -= Skip_Click;
			}

			// Perform cleanup that has to be executed in either case:
			AWBCleanupCheckboxes = null;
			PluginSettings = null;
			State = null;

			// Remember that this object has been disposed of:
			disposed = true;
		}

		internal void Dispose()
		{
			Debug.WriteLine("Disposing of AssessmentClass object");
			// Execute the code that does the cleanup.
			Dispose(true);
			// Let the CLR know that Finalize doesn't have to be called.
			GC.SuppressFinalize(this);
		}

		protected override void Finalize()
		{
			base.Finalize();
			Debug.WriteLine("Finalizing AssessmentClass object");
			// Execute the code that does the cleanup.
			Dispose(false);
		}
		#endregion

		// Friend methods:
		internal void Reset()
		{
			ToggleAWBCleanup(false);

			var _with1 = PluginManager.AWBForm.ListMaker;
			if (_with1.Count > 1 && State.IsNextPage(_with1.Item(1).Name)) {
				_with1.RemoveAt(1);
			} else if (_with1.Count > 0 && State.IsNextPage(_with1.Item(0).Name)) {
				_with1.RemoveAt(0);
			}

			State = new StateClass();
		}
		internal void ProcessMainSpaceArticle(string ArticleTitle)
		{
			if (State.NextArticleShouldBeTalk) {
				IsThisABug("a talk page");
			}

			State.NextTalkPageExpected = ArticleTitle;

			var _with2 = PluginManager.AWBForm.ListMaker;
			if (_with2.Count < 2) {
				_with2.Insert(1, State.NextTalkPageExpected);
			} else if (!State.IsNextPage(_with2.Item(1).Name)) {
				_with2.Insert(1, State.NextTalkPageExpected);
			}

			State.NextEventShouldBeMainSpace = true;
			State.NextArticleShouldBeTalk = true;
		}
		internal bool ProcessTalkPage(Article TheArticle, PluginSettingsControl pluginSettings, ref bool ReqPhoto)
		{
			bool WeAddedAReqPhotoParam = false;
			bool returnVal = false;

			if (!State.NextArticleShouldBeTalk) {
				IsThisABug("an article");
			} else if (!State.IsNextPage(TheArticle.FullArticleTitle)) {
				IsThisABug(State.NextTalkPageExpected);
			} else {
				State.NextArticleShouldBeTalk = false;

				PluginManager.StatusText.Text = "Assessments plugin: please assess the article or click cancel";

				AssessmentForm frmDialog = new AssessmentForm();

				returnVal = (frmDialog.ShowDialog(ref State.Classification, ref State.Importance, ref State.NeedsInfobox, ref State.NeedsAttention, ref State.NeedsPhoto, State.NextTalkPageExpected) == DialogResult.OK);

				if (returnVal) {
					PluginManager.StatusText.Text = "Processing " + TheArticle.FullArticleTitle;

					foreach (PluginBase p in PluginManager.ActivePlugins) {
						if (p.ProcessTalkPage(TheArticle, State.Classification, State.Importance, State.NeedsInfobox, State.NeedsAttention, true, ProcessTalkPageMode.ManualAssessment, ReqPhoto || State.NeedsPhoto) && (ReqPhoto || State.NeedsPhoto) && p.HasReqPhotoParam)
							WeAddedAReqPhotoParam = true;
						if (TheArticle.PluginManagerGetSkipResults == SkipResults.SkipBadTag) {
							MessageBox.Show("Bad tag(s). Fix manually.", "Bad tag", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							break; // TODO: might not be correct. Was : Exit For
						}
					}
				} else {
					pluginSettings.PluginStats.SkippedMiscellaneousIncrement(false);
					PluginManager.StatusText.Text = "Skipping this talk page";
					LoadArticle();
				}
			}

			if (returnVal) {
				switch (State.Classification) {
					case Classification.Code:
					case Classification.Unassessed:
						TheArticle.EditSummary = "Assessed article using " + conWikiPlugin;
						break;
					default:
						TheArticle.EditSummary = "Assessing as " + State.Classification.ToString() + " class, using " + conWikiPlugin;
						break;
				}

				ReqPhoto = WeAddedAReqPhotoParam;
			} else {
				ReqPhoto = false;
			}

			return returnVal;
		}

		// Private:
		private void IsThisABug(string text)
		{
			PluginManager.StatusText.Text = "Unexpected sequence: Assessments plugin is stopping AWB.";
			MessageBox.Show("The assessments plugin was expecting to receive " + text + " next. Is this a bug or is your list messed up?", "Expecting " + text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			PluginManager.StopAWB();
		}
		private void ToggleAWBCleanup(bool Cleanup)
		{
			foreach (CheckBox chk in AWBCleanupCheckboxes) {
				chk.Checked = Cleanup;
			}
		}
		private void LoadTalkPage()
		{
			//State.blnWaitingForATalkPage = True

			ToggleAWBCleanup(false);

			PluginManager.StatusText.Text = "Assessments plugin: loading talk page";
		}
		private void LoadArticle()
		{
			ToggleAWBCleanup(PluginSettings.Cleanup);

		}

		// UI event handlers:
		private void Save_Click(object sender, EventArgs e)
		{
			if (!disposed) {
				if (State.NextEventShouldBeMainSpace) {
					LoadTalkPage();
				} else {
					LoadArticle();
				}

				State.NextEventShouldBeMainSpace = !State.NextEventShouldBeMainSpace;
			}
		}
		private void Skip_Click(object sender, EventArgs e)
		{
			if (!disposed) {
				if (State.NextEventShouldBeMainSpace) {
					LoadTalkPage();
				} else {
					LoadArticle();
					PluginManager.AWBForm.TraceManager.SkippedArticle("User", WikiFunctions.Logging.AWBLogListener.StringUserSkipped);
					PluginSettings.PluginStats.SkippedMiscellaneousIncrement(true);
				}

				State.NextEventShouldBeMainSpace = !State.NextEventShouldBeMainSpace;
			}
		}
		private void CleanupCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!disposed && PluginSettings.Cleanup)
				ToggleAWBCleanup(true);
		}

		// Webcontrol event handlers:
		private void EditorStatusChanged(AsyncApiEdit sender)
		{
			if (PluginManager.AWBForm.TheSession.Editor.IsActive) {
				LoadArticle();
			}
		}

		// State:
		private sealed class StateClass
		{
			//Friend LastArticle As String


			string page;
			internal string NextTalkPageExpected {
				get { return page; }
				set {
					pageRegex = new Regex(Variables.NamespacesCaseInsensitive[Namespace.Talk] + value);
					page = Variables.Namespaces[Namespace.Talk] + value;
				}
			}

			//Friend EditSummary As String


			private Regex pageRegex;
			internal bool IsNextPage(string title)
			{
				return pageRegex.IsMatch(title);
			}

			internal bool NextEventShouldBeMainSpace;

			internal bool NextArticleShouldBeTalk;
			// Assessment:
			internal Classification Classification;
			internal Importance Importance;
			internal bool NeedsInfobox;
			internal bool NeedsAttention;
			internal bool NeedsPhoto;
		}
	}
}
