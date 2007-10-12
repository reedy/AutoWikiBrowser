Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class GenericExtraParametersForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.dgvParameters = New System.Windows.Forms.DataGridView
            Me.colParameter = New System.Windows.Forms.DataGridViewTextBoxColumn
            Me.colValue = New System.Windows.Forms.DataGridViewCheckBoxColumn
            Me.colEnabled = New System.Windows.Forms.DataGridViewCheckBoxColumn
            CType(Me.dgvParameters, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'dgvParameters
            '
            Me.dgvParameters.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvParameters.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colParameter, Me.colValue, Me.colEnabled})
            Me.dgvParameters.Location = New System.Drawing.Point(12, 36)
            Me.dgvParameters.Name = "dgvParameters"
            Me.dgvParameters.Size = New System.Drawing.Size(396, 169)
            Me.dgvParameters.TabIndex = 0
            '
            'colParameter
            '
            Me.colParameter.HeaderText = "Parameter"
            Me.colParameter.Name = "colParameter"
            '
            'colValue
            '
            Me.colValue.HeaderText = "Value"
            Me.colValue.Name = "colValue"
            '
            'colEnabled
            '
            Me.colEnabled.HeaderText = "Enabled"
            Me.colEnabled.Name = "colEnabled"
            '
            'GenericExtraParametersForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(420, 217)
            Me.Controls.Add(Me.dgvParameters)
            Me.Name = "GenericExtraParametersForm"
            Me.Text = "Extra Parameters"
            CType(Me.dgvParameters, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents dgvParameters As System.Windows.Forms.DataGridView
        Friend WithEvents colParameter As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colValue As System.Windows.Forms.DataGridViewCheckBoxColumn
        Friend WithEvents colEnabled As System.Windows.Forms.DataGridViewCheckBoxColumn
    End Class
End Namespace