'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk
    ''' <summary>
    ''' An object which wraps around a collection of template parameters
    ''' </summary>
    ''' <remarks></remarks>
    Friend NotInheritable Class Templating
        Friend FoundTemplate As Boolean, BadTemplate As Boolean
        Friend Parameters As New Dictionary(Of String, TemplateParametersObject)

        ''' <summary>
        ''' Store a parameter from the exploded on-page template into the Parameters collectiom
        ''' </summary>
        Friend Sub AddTemplateParmFromExistingTemplate(ByVal ParameterName As String, ByVal ParameterValue As String)
            If Parameters.ContainsKey(ParameterName) Then ' v2.0: Let's merge duplicates when one or both is empty:
                ' This code is very similar to ReplaceTemplateParm(), but that is for programmatic changes (i.e. not
                ' from template), needs an Article object, doesn't understand empty new values, and doesn't report
                ' bad tags. Turned out to be easier to rewrite here than to modify it.
                If Not Parameters.Item(ParameterName).Value = ParameterValue Then
                    If Parameters.Item(ParameterName).Value = "" Then ' existing value is empty, overwrite with new
                        Parameters.Item(ParameterName).Value = ParameterValue
                    ElseIf ParameterValue = "" Then ' new value is empty, keep existing
                    Else ' 2 different non-empty values, template is bad
                        BadTemplate = True
                    End If
                End If ' Else: 2 values the same, do nothing
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
            Dim res As Boolean

            If Parameters.ContainsKey(ParameterName) Then
                res = ReplaceTemplateParm(ParameterName, ParameterValue, TheArticle, _
                   LogItAndUpdateEditSummary, DontChangeIfSet, PluginName)
            ElseIf ParamHasAlternativeName AndAlso Parameters.ContainsKey(ParamAlternativeName) Then
                res = ReplaceTemplateParm(ParamAlternativeName, ParameterValue, TheArticle, _
                   LogItAndUpdateEditSummary, DontChangeIfSet, PluginName)
            Else ' Doesn't contain parameter
                NewTemplateParm(ParameterName, ParameterValue, LogItAndUpdateEditSummary, TheArticle, PluginName, _
                   MinorEditOnlyIfAdding)

                If MinorEditOnlyIfAdding Then TheArticle.ArticleHasAMinorChange() _
                   Else TheArticle.ArticleHasAMajorChange()
                Return True
            End If

            If res Then TheArticle.ArticleHasAMajorChange()
            Return res
        End Function
        Private Function ReplaceTemplateParm(ByVal ParameterName As String, ByVal ParameterValue As String, _
        ByVal TheArticle As Article, ByVal LogItAndUpdateEditSummary As Boolean, ByVal DontChangeIfSet As Boolean, _
        ByVal PluginName As String) As Boolean
            Dim ExistingValue As String = _
               WikiFunctions.WikiRegexes.Comments.Replace(Parameters(ParameterName).Value, "").Trim ' trim still needed because altho main regex shouldn't give us spaces at the end of vals any more, the .Replace here might

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
                    Return True
                Else ' Contains param with a different value, and we don't want to change it
                    PluginManager.AWBForm.TraceManager.WriteArticleActionLine( _
                       String.Format("{0} not changed, has existing value of {1}", _
                       ParameterName, ParameterValue), PluginName)
                    Return False
                End If
            End If ' Else: Already contains parameter and correct value; no need to change
            Return False
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
            Dim res As String = ""
            For Each o As KeyValuePair(Of String, TemplateParametersObject) In Parameters
                With o
                    res += "|" + .Key + "=" + .Value.Value + ParameterBreak
                End With
            Next

            res += "}}" + Microsoft.VisualBasic.vbCrLf
            Return res
        End Function
        Friend Function HasYesParam(ByVal ParamName As String) As Boolean
            Return (Parameters.ContainsKey(ParamName) AndAlso Parameters(ParamName).Value = "yes")
        End Function
        Friend Function HasYesParamLowerOrTitleCase(ByVal Yes As Boolean, ByVal ParamName As String) As Boolean
            ' A little hack to ensure we don't change no to No or yes to Yes as our only edit, and also for checking "yes" values
            If Parameters.ContainsKey(ParamName) Then
                If Yes AndAlso Parameters(ParamName).Value.ToLower = "yes" Then
                    Return True
                ElseIf Not Yes AndAlso Parameters(ParamName).Value.ToLower = "No" Then
                    Return True
                End If
            End If

            Return False
        End Function

        ''' <summary>
        ''' An object which represents a template parameter
        ''' </summary>
        ''' <remarks></remarks>
        Friend NotInheritable Class TemplateParametersObject
            Friend Name As String
            Friend Value As String

            Friend Sub New(ByVal ParameterName As String, ByVal ParameterValue As String)
                Name = ParameterName
                Value = ParameterValue
            End Sub
        End Class
    End Class
End Namespace