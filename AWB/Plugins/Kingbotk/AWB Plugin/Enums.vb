Namespace AWB
    Public Enum Namespaces As Integer
        Media = -2
        Special = -1
        Main = 0
        Talk
        User
        UserTalk
        Project
        ProjectTalk
        Image
        ImageTalk
        Mediawiki
        MediawikiTalk
        Template
        TemplateTalk
        Help
        HelpTalk
        Category
        CategoryTalk
        Portal = 100
        PortalTalk
    End Enum
End Namespace

Namespace AWB.Plugins.SDKSoftware.Kingbotk
    Friend Enum SkipResults As Integer
        NotSet = -1
        'Processed = 0
        SkipNoChange = 1
        SkipBadTag
        SkipMiscellaneous
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
End Namespace
