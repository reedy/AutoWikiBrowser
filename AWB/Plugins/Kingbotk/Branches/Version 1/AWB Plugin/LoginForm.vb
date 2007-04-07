<Assembly: system.Security.Permissions.RegistryPermissionAttribute( _
    system.Security.permissions.SecurityAction.RequestMinimum, ViewAndModify:="HKEY_CURRENT_USER")> 
Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class LoginForm
        Private Const conRegKey As String = "Software\SDK Software\Kingbotk Plugin"
        Private Const conPassPhrase As String = "oi frjweopi 4r390%^($%%^$HJKJNMHJGY 2`';'[#"
        Private Const conSalt As String = "SH1ew yuhn gxe$£$%^y HNKLHWEQ JEW`b"
        Private Const conIV16Chars As String = "tnf47bgfdwlp9,.q"

        Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
            Me.Close()
        End Sub
        Friend ReadOnly Property GetUsernamePassword() As UsernamePassword
            Get
                GetUsernamePassword = New UsernamePassword

                With GetUsernamePassword
                    Try
                        .Username = Computer.Registry.GetValue("HKEY_CURRENT_USER\" & conRegKey, "1", "").ToString
                        .Password = Computer.Registry.GetValue("HKEY_CURRENT_USER\" & conRegKey, "2", "").ToString
                        If .Username = "" OrElse .Password = "" Then _
                            Return GetFromForm()
                    Catch
                        Return GetFromForm()
                    End Try

                    .Username = Decrypt(.Username)
                    .Password = Decrypt(.Password)
                    .IsSet = True
                End With

                Exit Property
            End Get
        End Property
        Private Function Decrypt(ByVal Text As String) As String
            Return Encryption.RijndaelSimple.Decrypt(Text, conPassPhrase, conSalt, "SHA1", 2, conIV16Chars, 256)
        End Function
        Private Function Encrypt(ByVal Text As String) As String
            Return Encryption.RijndaelSimple.Encrypt(Text, conPassPhrase, conSalt, "SHA1", 2, conIV16Chars, 256)
        End Function
        Friend Function GetFromForm() As UsernamePassword
            Me.ShowDialog()
            GetFromForm = New UsernamePassword
            With GetFromForm
                .Username = UsernameTextBox.Text
                .Password = PasswordTextBox.Text
                If .Username = "" OrElse .Password = "" Then
                    MessageBox.Show("Please enter a username and password", "Upload Login Details", _
                       MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return Me.GetFromForm
                Else
                    If CheckBox1.Checked Then
                        Dim key As Microsoft.Win32.RegistryKey = Computer.Registry.CurrentUser.CreateSubKey(conRegKey)
                        key.SetValue("1", Encrypt(.Username))
                        key.SetValue("2", Encrypt(.Password))
                    End If
                    .IsSet = True
                End If
            End With
        End Function
    End Class
End Namespace