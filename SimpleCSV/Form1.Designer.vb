<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.DGV = New System.Windows.Forms.DataGridView()
        Me.BtnImportCSV_BGW = New System.Windows.Forms.Button()
        Me.PB = New System.Windows.Forms.ProgressBar()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.BGW = New System.ComponentModel.BackgroundWorker()
        Me.BtnImportCSV_Async1 = New System.Windows.Forms.Button()
        Me.BtnImportCSV_Async2 = New System.Windows.Forms.Button()
        CType(Me.DGV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DGV
        '
        Me.DGV.AllowUserToAddRows = False
        Me.DGV.AllowUserToDeleteRows = False
        Me.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV.Location = New System.Drawing.Point(12, 12)
        Me.DGV.Name = "DGV"
        Me.DGV.ReadOnly = True
        Me.DGV.Size = New System.Drawing.Size(582, 273)
        Me.DGV.TabIndex = 0
        '
        'BtnImportCSV_BGW
        '
        Me.BtnImportCSV_BGW.Location = New System.Drawing.Point(12, 350)
        Me.BtnImportCSV_BGW.Name = "BtnImportCSV_BGW"
        Me.BtnImportCSV_BGW.Size = New System.Drawing.Size(582, 23)
        Me.BtnImportCSV_BGW.TabIndex = 1
        Me.BtnImportCSV_BGW.Text = "Import CSV BackgroundWorker (ReadLine)"
        Me.BtnImportCSV_BGW.UseVisualStyleBackColor = True
        '
        'PB
        '
        Me.PB.Location = New System.Drawing.Point(12, 321)
        Me.PB.Name = "PB"
        Me.PB.Size = New System.Drawing.Size(582, 23)
        Me.PB.TabIndex = 2
        '
        'LblStatus
        '
        Me.LblStatus.Location = New System.Drawing.Point(12, 295)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(582, 23)
        Me.LblStatus.TabIndex = 3
        Me.LblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BGW
        '
        Me.BGW.WorkerReportsProgress = True
        Me.BGW.WorkerSupportsCancellation = True
        '
        'BtnImportCSV_Async1
        '
        Me.BtnImportCSV_Async1.Location = New System.Drawing.Point(12, 379)
        Me.BtnImportCSV_Async1.Name = "BtnImportCSV_Async1"
        Me.BtnImportCSV_Async1.Size = New System.Drawing.Size(582, 23)
        Me.BtnImportCSV_Async1.TabIndex = 4
        Me.BtnImportCSV_Async1.Text = "Import CSV Async Await (ReadLineAsync)"
        Me.BtnImportCSV_Async1.UseVisualStyleBackColor = True
        '
        'BtnImportCSV_Async2
        '
        Me.BtnImportCSV_Async2.Location = New System.Drawing.Point(15, 408)
        Me.BtnImportCSV_Async2.Name = "BtnImportCSV_Async2"
        Me.BtnImportCSV_Async2.Size = New System.Drawing.Size(582, 23)
        Me.BtnImportCSV_Async2.TabIndex = 5
        Me.BtnImportCSV_Async2.Text = "Import CSV Async Await (ReadLine)"
        Me.BtnImportCSV_Async2.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(606, 449)
        Me.Controls.Add(Me.BtnImportCSV_Async2)
        Me.Controls.Add(Me.BtnImportCSV_Async1)
        Me.Controls.Add(Me.LblStatus)
        Me.Controls.Add(Me.PB)
        Me.Controls.Add(Me.BtnImportCSV_BGW)
        Me.Controls.Add(Me.DGV)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Simple CSV Async Await/ BackgroundWorker"
        CType(Me.DGV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DGV As DataGridView
    Friend WithEvents BtnImportCSV_BGW As Button
    Friend WithEvents PB As ProgressBar
    Friend WithEvents LblStatus As Label
    Friend WithEvents BGW As System.ComponentModel.BackgroundWorker
    Friend WithEvents BtnImportCSV_Async1 As Button
    Friend WithEvents BtnImportCSV_Async2 As Button
End Class
