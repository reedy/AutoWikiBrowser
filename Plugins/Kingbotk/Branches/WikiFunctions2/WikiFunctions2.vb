Public Module WikiFunctions2
    ''' <summary>
    ''' WikiFunctions2.dll version
    ''' </summary>
    Public ReadOnly Property Version() As System.Version
        Get
            Return System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
        End Get
    End Property
End Module
