Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
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
