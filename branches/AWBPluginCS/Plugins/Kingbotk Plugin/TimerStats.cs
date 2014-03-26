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
using WikiFunctions.API;

namespace AutoWikiBrowser.Plugins.Kingbotk.Components
{
	internal sealed partial class TimerStats
	{
		private AsyncApiEdit withEventsField_editor;
		private AsyncApiEdit editor {
			get { return withEventsField_editor; }
			set {
				if (withEventsField_editor != null) {
					withEventsField_editor.SaveComplete -= IncrementSavedEdits;
				}
				withEventsField_editor = value;
				if (withEventsField_editor != null) {
					withEventsField_editor.SaveComplete += IncrementSavedEdits;
				}
			}
		}
		private AutoWikiBrowser.Plugins.Kingbotk.Components.PluginSettingsControl.Stats withEventsField_mStats;
		private AutoWikiBrowser.Plugins.Kingbotk.Components.PluginSettingsControl.Stats mStats {
			get { return withEventsField_mStats; }
			set {
				if (withEventsField_mStats != null) {
					withEventsField_mStats.SkipMisc -= mStats_SkipMisc;
				}
				withEventsField_mStats = value;
				if (withEventsField_mStats != null) {
					withEventsField_mStats.SkipMisc += mStats_SkipMisc;
				}
			}
		}
		private TimeSpan TimeSpan;
		private System.DateTime Start;
		private int mNumberOfEdits;
		private int mSkipped;

		private Label mETALabel;
		private int NumberOfEdits {
			get { return mNumberOfEdits; }
			set {
				mNumberOfEdits = value;
				EditsLabel.Text = mNumberOfEdits.ToString();
			}
		}

		internal void Init(AsyncApiEdit e, Label ETALabel, PluginSettingsControl.Stats Stats)
		{
			if (!TimerEnabled) {
				editor = e;

				ResetVars();
				mETALabel = ETALabel;

				TimerEnabled = true;
				mStats = Stats;

				Timer1_Tick(null, null);
			}
		}
		internal void Reset()
		{
			ResetVars();
			TimerLabel.Text = "0:00";
			SpeedLabel.Text = "0";
			EditsLabel.Text = "0";
		}
		private void ResetVars()
		{
			NumberOfEdits = 0;
			Start = System.DateTime.Now;
			mSkipped = 0;
		}
		internal void StopStats()
		{
			ResetVars();
			TimerEnabled = false;
		}
		private string ETA {
			get { return mETALabel.Text.Replace("ETC: ", ""); }
			set { mETALabel.Text = "ETC: " + value; }
		}
		private void CalculateETA(double SecondsPerPage)
		{
			int Count = PluginManager.AWBForm.ListMaker.Count;

			if (Count == 0) {
				ETA = "Now";
			} else {
				System.DateTime ETADateTime = System.DateTime.Now.AddSeconds(SecondsPerPage * Count);

				if (ETADateTime.Date == System.DateTime.Now.Date) {
					ETA = ETADateTime.ToString("HH:mm") + " today";
				} else if (System.DateTime.Now.AddDays(1).Date == ETADateTime.Date) {
					ETA = ETADateTime.ToString("HH:mm") + " tomorrow";
				} else {
					ETA = ETADateTime.ToString("HH:mm, ddd d MMM");
				}
			}
		}
		private bool TimerEnabled {
			get { return Timer1.Enabled; }
			set {
				if (mETALabel != null) {
					mETALabel.Visible = value;
				}
				Timer1.Enabled = value;
			}
		}

		// Event handlers
		private readonly Regex timerregexp = new Regex("\\..*");
		int static_Timer1_Tick_UpdateETACount;
		private void Timer1_Tick(object sender, EventArgs e)
		{
			double SecondsPerPage = 0;

			static_Timer1_Tick_UpdateETACount += 1;
			TimeSpan = System.DateTime.Now - Start;
			TimerLabel.Text = timerregexp.Replace(TimeSpan.ToString(), "");
			if (NumberOfEdits == 0) {
				SecondsPerPage = TimeSpan.TotalSeconds;
			} else {
				SecondsPerPage = Math.Round(TimeSpan.TotalSeconds / NumberOfEdits, 2);
			}

			if (double.IsInfinity(SecondsPerPage)) {
				SpeedLabel.Text = "0";
				ETA = "-";
				if (static_Timer1_Tick_UpdateETACount > 9)
					static_Timer1_Tick_UpdateETACount = 0;
			} else {
				SpeedLabel.Text = SecondsPerPage.ToString() + " s/p";
				if (static_Timer1_Tick_UpdateETACount > 9 || ETA == "-") {
					static_Timer1_Tick_UpdateETACount = 0;
					if ((NumberOfEdits + mSkipped) == 0) {
						CalculateETA(TimeSpan.TotalSeconds);
					} else {
						CalculateETA(TimeSpan.TotalSeconds / (NumberOfEdits + mSkipped));
					}

				}
			}
		}
		internal void IncrementSavedEdits(AsyncApiEdit sender, SaveInfo save)
		{
			IncrementSavedEdits();
		}
		internal void IncrementSavedEdits()
		{
			NumberOfEdits += 1;
		}
		private void mStats_SkipMisc(int val)
		{
			mSkipped += 1;
		}
		public TimerStats()
		{
			InitializeComponent();
		}
	}
}
