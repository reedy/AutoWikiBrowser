Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
    Friend Enum SkipResults As Integer
        NotSet = -1
        'Processed = 0
        SkipNoChange = 1
        SkipBadTag
        SkipRegex
    End Enum
    Friend Enum Classification As Integer
        Unassessed = 0
        Stub
        Start
        B
        GA
        A
        FA
        NA
        Code = 100
    End Enum
    Friend Enum Importance As Integer
        Unassessed = 0
        Low
        Mid
        High
        Top
        NA
        Code = 100
    End Enum
    Friend Enum ProcessTalkPageMode As Integer
        Normal
        ManualAssessment
        NonStandardTalk
    End Enum

    Friend Module Constants
        ' Regular expression strings:
        Friend Const TemplatePrefix As String = "\{\{\s*(?<tl>template *:)?\s*"
        Friend Const conRegexpLeft As String = TemplatePrefix & "(?<tlname>" ' put "(?<!<nowiki[\n\r]*>\s*)" at the start to ignore nowiki, but was difficult to get secondchanceregex adapted (without becoming too strict) so I gave up. It seemed the engine was trying to do it's best to *avoid* matching this negative group
        Friend Const conRegexpRight As String = _
           ")\b\s*(((\|\|*| \||\| |\s*\|\s*(?<parm>[^}{|\s=]*))\s*)+(=\s*(?<val>[^}{|\n\r]*?)\s*)?)*\}\}\s*" ' put "\s(?!</nowiki[\n\r]*>)" at the end to ignore nowiki
        ' Don't delete these just yet:
        ' Last known good before loosened up to accept null parameters ("|||")
        '")\b\s*((\s*\|\s*(?<parm>[^}{|\s=]*)\s*)+(=\s*(?<val>[^}{|\n\r]*)\s*)?)*\}\}\s*"
        ' This version ignored stray vertical bars but would freeze up at with some text like the following:
        ' )\b\s*((\s*\|\s*([\|\s]*|(?<parm>[^}{|\s=]*)\s*)+(=\s*(?<val>[^}{|\n\r]*?)\s*)?))*\}\}\s*
        ' And so did this simplified version:
        ' ")\b\s*((([\s\|]*|\s*\|\s*(?<parm>[^}{|\s=]*))\s*)+(=\s*(?<val>[^}{|\n\r]*?)\s*)?)*\}\}\s*"
        '{{WPBiography
        '|small = 
        '|living = no
        '|class = Stub
        '|priority = Low
        '|needs-infobox = no
        '<!--|listas = Cech, Martin == remove sneaky, hidden missorting-->
        '|needs-photo = yes
        '|sports-work-group = yes
        '}}
        '{{Ice hockey|class=Stub}}
        '{{DEFAULTSORT:Cech, Martin}}
        Friend Const conRegexpRightNotStrict As String = ")\b" '")\b[^}]*"
        Friend Const conRegexpOptions As RegexOptions = RegexOptions.Compiled Or RegexOptions.Multiline Or _
           RegexOptions.IgnoreCase Or RegexOptions.ExplicitCapture

        ' Identifiers:
        Friend Const Biography As String = "Biography"
        Friend Const conWikiPlugin As String = "[[User:Kingbotk/P|Plugin++]]"
        Friend Const conWikiPluginBrackets As String = "(" & conWikiPlugin & ") "
        Friend Const conAWBPluginName As String = "Kingbotk Plugin"
        'Placeholders:
        Friend Const conTemplatePlaceholder As String = "{{xxxTEMPLATExxx}}"
    End Module
End Namespace
