'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Imports System.Windows.Forms

Namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
    Friend NotInheritable Class AssessmentsInstructionsDialog
        Private Sub OK_Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OK_Button.Click
            If CheckBox1.Checked _
               Then Me.DialogResult = Windows.Forms.DialogResult.Yes _
               Else Me.DialogResult = System.Windows.Forms.DialogResult.OK

            Me.Close()
        End Sub
    End Class
End Namespace