using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoWikiBrowser.Plugins.TheTemplator
{
	public partial class TemplatorParameterDetails : Form
	{
		public TemplatorParameterDetails()
		{
			InitializeComponent();
		}

		public TemplatorParameterDetails(string param, string regex)
		{
			InitializeComponent();
			paramName.Text = param;
			paramRegex.Text = regex;
		}

		public string ParamName { get { return paramName.Text; } }
		public string ParamRegex { get { return paramRegex.Text; } }

		private void regexHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			regexHelp.LinkVisited = true;
			WikiFunctions.Tools.OpenURLInBrowser("http://msdn.microsoft.com/en-us/library/hs600312.aspx");
		}

	}
}
