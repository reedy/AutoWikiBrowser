' TODO: There's a great deal of common code used in these controls which could be inherited if I can work out how to inherit from and use a custom user control
Public Class WPSongsSettings
    Implements GenericSettingsClass

    Private Const conAutoStubParm As String = "SongsAutoStub"
    Private Const conStubClassParm As String = "SongsStubClass"

    ' UI:
    Private txtEdit As TextBox

    ' Properties:
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
    Public ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection _
    Implements GenericSettingsClass.TextInsertContextMenuStripItems
        Get

        End Get
    End Property

#Region "XML interface"
    Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements GenericSettingsClass.ReadXML
        AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub)
        StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass)
    End Sub
    Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements GenericSettingsClass.WriteXML
        Writer.WriteAttributeString(conAutoStubParm, AutoStub.ToString)
        Writer.WriteAttributeString(conStubClassParm, StubClass.ToString)
    End Sub
    Public Sub Reset() Implements GenericSettingsClass.XMLReset
        AutoStub = False
        StubClass = False
    End Sub
#End Region
End Class
