Friend Class SettingsControlBase
    Implements GenericSettingsClass

    ' UI:
    Protected txtEdit As TextBox

#Region "XML interface"
    Public Overridable Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements GenericSettingsClass.ReadXML

    End Sub
    Public Overridable Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements GenericSettingsClass.WriteXML

    End Sub
    Public Overridable Sub Reset() Implements GenericSettingsClass.XMLReset

    End Sub
#End Region

    Public Property StubClass() As Boolean Implements GenericSettingsClass.StubClass
        Get
            Return StubClassCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            StubClassCheckBox.Checked = value
        End Set
    End Property
    Public Property AutoStub() As Boolean Implements GenericSettingsClass.AutoStub
        Get
            Return AutoStubCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            AutoStubCheckBox.Checked = value
        End Set
    End Property
    WriteOnly Property StubClassModeAllowed() As Boolean Implements GenericSettingsClass.StubClassModeAllowed
        Set(ByVal value As Boolean)
            StubClassCheckBox.Enabled = value
        End Set
    End Property
    Public WriteOnly Property EditTextBox() As TextBox Implements GenericSettingsClass.EditTextBox
        Set(ByVal value As TextBox)
            txtEdit = value
        End Set
    End Property
    Public Overridable ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection _
    Implements GenericSettingsClass.TextInsertContextMenuStripItems
        Get
            Return Nothing
        End Get
    End Property

    Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
    ByVal e As System.EventArgs)
        If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
    End Sub
    Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
    ByVal e As System.EventArgs)
        If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
    End Sub
End Class
