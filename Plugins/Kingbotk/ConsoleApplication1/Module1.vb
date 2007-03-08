Imports system.xml
''' <summary>
''' test
''' </summary>
''' <remarks></remarks>
Module Module1
    Dim MainRegex As Regex
    Dim SecondChanceRegex As Regex
    Const conRegexpLeft As String = "\{\{\s*(?<tl>template\s*:)?\s*(?<tlname>"
    'dim Const conRegexpRight As String = _
    '   ")[\s\n\r]*(([\s\n\r]*\|[\s\n\r]*(?<parm>[-a-z0-9&]*)[\s\n\r]*)+(=[\s\n\r]*(?<val>[-a-z0-9]*)[\s\n\r]*)?)*\}\}[\s\n\r]*"
    Const conRegexpRight As String = _
       ")[\s\n\r]*(([\s\n\r]*\|[\s\n\r]*(?<parm>[^}{|\s\n\r=]*)[\s\n\r]*)+(=[\s\n\r]*" & _
       "(?<val>[^}{|\s\n\r]*)[\s\n\r]*)?)*\}\}[\s\n\r]*"
    Const conRegexpRightNotStrict As String = ")[^}]*"
    Const conRegexpOptions As RegexOptions = RegexOptions.Compiled Or RegexOptions.Multiline Or _
       RegexOptions.IgnoreCase Or RegexOptions.ExplicitCapture
    Dim PreferredTemplateNameRegex As Regex

    Sub Main()
        Const RegexpMiddle As String = "WPBiography|BioWikiProject|Musician|WikiProject Biography|broy"
        MainRegex = New Regex(conRegexpLeft & RegexpMiddle & conRegexpRight, conRegexpOptions)

        Dim ArticleText As String = "{{talkheader}}{{WPBiography|living=yes|class=B|importance=|musician-work-group=yes|listas=Levitin, Daniel}}" & _
           Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
           "== Too many red links hurt my brain ==" & Microsoft.VisualBasic.vbCrLf & _
           "I'm going to remove a few of them.  I hope you don't mind. -- [[User:Craigtalbert|Craigtalbert]] 13:30, 6 November 2006 (UTC)"

        Debug.Print(MainRegex.IsMatch(ArticleText).ToString)
    End Sub

End Module
