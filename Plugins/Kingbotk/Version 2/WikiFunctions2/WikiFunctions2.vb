Public Module WikiFunctions2
    ''' <summary>
    ''' WikiFunctions2.dll version
    ''' </summary>
    Public ReadOnly Property Version() As System.Version
        Get
            Static rtn As System.Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
            Return rtn
        End Get
    End Property
End Module
