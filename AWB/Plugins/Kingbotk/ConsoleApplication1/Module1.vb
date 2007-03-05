Imports system.xml
''' <summary>
''' test
''' </summary>
''' <remarks></remarks>
Module Module1

    Sub Main()
        Dim DefaultSortRegex As New Regex("\{{2,3}\s*(template\s*:\s*|)\s*DEFAULTSORT[\s\n\r]*(\||:)")

        Debug.Print(DefaultSortRegex.IsMatch("{{{DEFAULTSORT:Bloggs, Fred}}}").ToString)
    End Sub

End Module
