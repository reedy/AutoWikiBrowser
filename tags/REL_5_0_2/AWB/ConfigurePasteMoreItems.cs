/*
Copyright (C) 2009

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

*/

using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class ConfigurePasteMoreItems : Form
    {
        public ConfigurePasteMoreItems()
        {
            InitializeComponent();
        }

        public ConfigurePasteMoreItems(string string1, string string2, string string3, string string4, string string5, string string6, string string7, string string8, string string9, string string10)
            : this()
        {
            String1 = string1;
            String2 = string2;
            String3 = string3;
            String4 = string4;
            String5 = string5;
            String6 = string6;
            String7 = string7;
            String8 = string8;
            String9 = string9;
            String10 = string10;
        }

        public string String1 { get { return textBox1.Text; } private set { textBox1.Text = value; } }
        public string String2 { get { return textBox2.Text; } private set { textBox2.Text = value; } }
        public string String3 { get { return textBox3.Text; } private set { textBox3.Text = value; } }
        public string String4 { get { return textBox4.Text; } private set { textBox4.Text = value; } }
        public string String5 { get { return textBox5.Text; } private set { textBox5.Text = value; } }
        public string String6 { get { return textBox6.Text; } private set { textBox6.Text = value; } }
        public string String7 { get { return textBox7.Text; } private set { textBox7.Text = value; } }
        public string String8 { get { return textBox8.Text; } private set { textBox8.Text = value; } }
        public string String9 { get { return textBox9.Text; } private set { textBox9.Text = value; } }
        public string String10 { get { return textBox10.Text; } private set { textBox10.Text = value; } }
    }
}
