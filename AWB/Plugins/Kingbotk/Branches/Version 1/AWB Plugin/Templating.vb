Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
    ''' <summary>
    ''' An object which wraps around a collection of template parameters
    ''' </summary>
    ''' <remarks></remarks>
    Friend NotInheritable Class Templating
        Friend FoundTemplate As Boolean = False, BadTemplate As Boolean = False
        Friend Parameters As New Dictionary(Of String, TemplateParametersObject)

        Friend Sub AddTemplateParmFromExistingTemplate(ByVal ParameterName As String, ByVal ParameterValue As String)
            If Parameters.ContainsKey(ParameterName) Then
                If Not Parameters.Item(ParameterName).Value = ParameterValue Then BadTemplate = True
            Else
                Parameters.Add(ParameterName, New TemplateParametersObject(ParameterName, ParameterValue))
            End If
        End Sub
        Friend Sub NewTemplateParm(ByVal ParameterName As String, ByVal ParameterValue As String)
            Parameters.Add(ParameterName, New TemplateParametersObject(ParameterName, ParameterValue))
        End Sub
        Friend Sub NewTemplateParm(ByVal ParameterName As String, ByVal ParameterValue As String, _
        ByVal LogItAndUpdateEditSummary As Boolean, ByVal TheArticle As Article, ByVal PluginName As String, _
        Optional ByVal MinorEdit As Boolean = False)
            NewTemplateParm(ParameterName, ParameterValue)
            If LogItAndUpdateEditSummary Then TheArticle.ParameterAdded(ParameterName, ParameterValue, _
               PluginName, MinorEdit)
        End Sub
        ''' <summary>
        ''' Checks for the existence of a parameter and adds it if missing/optionally changes it
        ''' </summary>
        ''' <returns>True if made a change</returns>
        Friend Function NewOrReplaceTemplateParm(ByVal ParameterName As String, ByVal ParameterValue As String, _
        ByVal TheArticle As Article, ByVal LogItAndUpdateEditSummary As Boolean, _
        ByVal ParamHasAlternativeName As Boolean, Optional ByVal DontChangeIfSet As Boolean = False, _
        Optional ByVal ParamAlternativeName As String = "", Optional ByVal PluginName As String = "", _
        Optional ByVal MinorEditOnlyIfAdding As Boolean = False) As Boolean

            If Parameters.ContainsKey(ParameterName) Then
                NewOrReplaceTemplateParm = ReplaceTemplateParm(ParameterName, ParameterValue, TheArticle, _
                   LogItAndUpdateEditSummary, DontChangeIfSet, PluginName)
            ElseIf ParamHasAlternativeName AndAlso Parameters.ContainsKey(ParamAlternativeName) Then
                NewOrReplaceTemplateParm = ReplaceTemplateParm(ParamAlternativeName, ParameterValue, TheArticle, _
                   LogItAndUpdateEditSummary, DontChangeIfSet, PluginName)
            Else ' Doesn't contain parameter
                NewTemplateParm(ParameterName, ParameterValue, LogItAndUpdateEditSummary, TheArticle, PluginName, _
                   MinorEditOnlyIfAdding)

                If MinorEditOnlyIfAdding Then TheArticle.ArticleHasAMinorChange() _
                   Else TheArticle.ArticleHasAMajorChange()
                Return True
            End If

            If NewOrReplaceTemplateParm Then TheArticle.ArticleHasAMajorChange()
        End Function
        Private Function ReplaceTemplateParm(ByVal ParameterName As String, ByVal ParameterValue As String, _
        ByVal TheArticle As Article, ByVal LogItAndUpdateEditSummary As Boolean, ByVal DontChangeIfSet As Boolean, _
        ByVal PluginName As String) As Boolean
            Dim ExistingValue As String = _
               WikiFunctions.WikiRegexes.Comments.Replace(Parameters(ParameterName).Value, "").Trim

            If Not ExistingValue = ParameterValue Then ' Contains parameter with a different value
                If ExistingValue = "" OrElse Not DontChangeIfSet _
                Then ' Contains parameter with a different value, and _
                    ' we want to change it; or contains an empty parameter
                    Parameters(ParameterName).Value = ParameterValue
                    TheArticle.ArticleHasAMajorChange()
                    If LogItAndUpdateEditSummary Then
                        If ExistingValue = "" Then
                            TheArticle.ParameterAdded(ParameterName, ParameterValue, PluginName)
                        Else
                            TheArticle.DoneReplacement(ParameterName & "=" & ExistingValue, ParameterValue, _
                               True, PluginName)
                        End If
                    End If
                    ReplaceTemplateParm = True
                Else ' Contains param with a different value, and we don't want to change it
                    PluginManager.AWBForm.TraceManager.WriteArticleActionLine( _
                       String.Format("{0} not changed, has existing value of {1}", _
                       ParameterName, ParameterValue), PluginName)
                End If
            End If ' Else: Already contains parameter and correct value; no need to change
        End Function
        Friend Sub RemoveParentWorkgroup(ByVal ChildWorkGroupParm As String, ByVal ParentWorkGroupParm As String, _
        ByVal AddChildWorkGroupParm As Boolean, ByVal Article As Article, ByVal PluginName As String)
            Dim AddedParm As Boolean

            If AddChildWorkGroupParm AndAlso Not Parameters.ContainsKey(ChildWorkGroupParm) Then
                NewTemplateParm(ChildWorkGroupParm, "yes")
                AddedParm = True
            End If

            If (AddedParm OrElse HasYesParam(ChildWorkGroupParm)) AndAlso Parameters.ContainsKey(ParentWorkGroupParm) Then
                Parameters.Remove(ParentWorkGroupParm)
                Article.DoneReplacement(ParentWorkGroupParm, ChildWorkGroupParm, True, PluginName)
            ElseIf AddedParm Then
                Article.ParameterAdded(ChildWorkGroupParm, "yes", PluginName)
            End If
        End Sub
        Friend Function ParametersToString(ByVal ParameterBreak As String) As String
            ParametersToString = ""
            For Each o As KeyValuePair(Of String, TemplateParametersObject) In Parameters
                With o
                    ParametersToString += "|" + .Key + "=" + .Value.Value + ParameterBreak
                End With
            Next

            ParametersToString += "}}" + Microsoft.VisualBasic.vbCrLf
        End Function
        Friend Function HasYesParam(ByVal ParamName As String) As Boolean
            Return (Parameters.ContainsKey(ParamName) AndAlso Parameters(ParamName).Value = "yes")
        End Function
        Friend Function HasYesParamLowerOrTitleCase(ByVal Yes As Boolean, ByVal ParamName As String) As Boolean
            ' A little hack to ensure we don't change no to No or yes to Yes as our only edit, and also for checking "yes" values
            If Parameters.ContainsKey(ParamName) Then
                If Yes Then
                    Select Case Parameters(ParamName).Value
                        Case "yes", "Yes"
                            Return True
                    End Select
                Else
                    Select Case Parameters(ParamName).Value
                        Case "no", "No"
                            Return True
                    End Select
                End If
            End If
        End Function

        ''' <summary>
        ''' An object which represents a template parameter
        ''' </summary>
        ''' <remarks></remarks>
        Friend NotInheritable Class TemplateParametersObject
            Public Name As String
            Public Value As String

            Friend Sub New(ByVal ParameterName As String, ByVal ParameterValue As String)
                Name = ParameterName
                Value = ParameterValue
            End Sub
        End Class
    End Class
End Namespace