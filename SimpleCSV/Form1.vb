Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Threading

Public Class Form1

    Private ReadOnly sw As New Stopwatch

    'saat DataGridView memiliki data yang banyak, sebut saja > 100000
    'ketika scroll baik vertikal/horizontal, datanya akan terlihat seperti terjeda/ lambat
    'umumnya DoubleBuffered dikhususkan untuk Form, 
    'namun dapat kita gunakan ke Control lainnya menggunakan method khusus
    'seperti ini.. pemanggilannya di dalam constructor new

    Private Sub SetDoubleBuffered(ctl As Control, setting As Boolean)
        Dim dgvType As Type = ctl.GetType()
        Dim pi As PropertyInfo = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)
        pi.SetValue(ctl, setting, Nothing)
    End Sub

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        SetDoubleBuffered(DGV, True)

    End Sub

#Region "Import CSV BackgroundWorker"
    Private Sub BtnImportCSV_BGW_Click(sender As Object, e As EventArgs) Handles BtnImportCSV_BGW.Click

        Dim CSVPath As String = "100000 Sales Records.csv"
        DGV.DataSource = Nothing
        'reset stopwatch dan atur button = disabled
        sw.Reset()
        sw.Start()
        LblStatus.Text = "Proses memuat data dimulai....."

        If Not BGW.IsBusy Then
            BGW.RunWorkerAsync(CSVPath)
        End If

    End Sub

    Private Sub BGW_DoWork(sender As Object, e As DoWorkEventArgs) Handles BGW.DoWork
        Try

            'parameter FileStream
            '4096 = default buffer
            'True/False = useAsync = asynchronous / synchronous mode

            Using FS As New FileStream(CType(e.Argument, String),
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                False)

                Using SR As New StreamReader(FS)

                    'siapkan variable DT sebagai datatable, yang nantinya jadi output
                    Using DT As New DataTable

                        '============== membuat DataColumn ======================
                        'ekspresi linq
                        Dim cols() As DataColumn = (From col In SR.ReadLine.Split(",")
                                                    Select New DataColumn(col, GetType(String))).ToArray

                        DT.Columns.AddRange(cols)

                        'ekspresi lambda
                        'Dim cols() As DataColumn = SR.ReadLine.Split(",").Select(Function(x) New DataColumn(x, GetType(String))).ToArray

                        'loop tradisional
                        'For Each x In SR.ReadLine.Split(",")
                        '    DT.Columns.Add(x, GetType(String))
                        'Next
                        '=========================================================================

                        'tambahkan DataRow sampai akhir baris text/csv
                        While Not SR.EndOfStream

                            'DataTable dapat menerima parameter array/list object, 
                            'langsung saja tambahkan object tersebut, di sini berupa array string
                            DT.Rows.Add(SR.ReadLine.Split(","))

                            'tampung nilai progressbar
                            Dim PBValue As Integer = FS.Position / FS.Length * 100

                            'gunakan method ReportProgress utk mengupdate UI
                            'method ini akan mentrigger event ProgressChanged
                            'di dalam event tersebutlah ProgressBar dan label nilainya diupdate
                            If PBValue Mod 10 = 0 Then
                                'atur supaya hanya melaporkan setiap 10 persen, 
                                'supaya Thread utama tidak terkuras resourcenya hanya untuk mengupdate ui
                                BGW.ReportProgress(PBValue, String.Format("Total data : {0}   |   Proses : {1}%", DT.Rows.Count, PBValue))
                            End If

                        End While

                        BGW.ReportProgress(100, "Menampilkan data.....")
                        Thread.Sleep(500)
                        'beri jeda 500ms supaya progressbar selesai mengupdate nilai dan animasinya


                        e.Result = DT

                    End Using

                End Using
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
            e.Cancel = True
        End Try
    End Sub

    Private Sub BGW_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BGW.ProgressChanged
        'update nilai dan animasi progressbar
        PB.Value = e.ProgressPercentage
        LblStatus.Text = e.UserState

    End Sub

    Private Sub BGW_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BGW.RunWorkerCompleted
        'jika tidak ada error
        If Not e.Cancelled Then
            'atur output, tampilkan ke datagridview
            Dim DT As DataTable = CType(e.Result, DataTable)

            DGV.DataSource = DT
            'hindari autosize seperti di bawah jika data yang dimuat banyak.. > 100000
            'DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

            LblStatus.Text = String.Format("Data selesai dimuat   |   Total data : {0}", DGV.Rows.Count)
            sw.Stop()

            MsgBox(String.Format("Total waktu eksekusi   |   {0:00} menit | {1:00} detik | {2:00} milidetik", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10))

        End If

        GC.Collect()
    End Sub


#End Region

#Region "Import CSV Async Await"

    Private Async Sub BtnImportCSV_Async2_Click(sender As Object, e As EventArgs) Handles BtnImportCSV_Async2.Click

        Dim path As String = "100000 Sales Records.csv"
        DGV.DataSource = Nothing
        sw.Reset()
        sw.Start()

        'event ProgressChanged umumnya sudah menjadi bagian dalam BackroundWorker
        'di .net framework 4.5 kita juga dapat menerapkannya dengan cara 
        'implementasikan IProgress melalui instance dari class Progress
        Dim _Progress As New Progress(Of Tuple(Of Integer, String))(Sub(Data)
                                                                        PB.Value = Data.Item1

                                                                        LblStatus.Text = Data.Item2

                                                                    End Sub)

        'gunakan kata await untuk supaya tidak terjadi blocking pada thread utama
        Dim DT As DataTable = Await TaskImportCSVSync(path, _Progress)
        Await Task.Delay(500)
        DGV.DataSource = DT

        LblStatus.Text = String.Format("Data selesai dimuat   |   Total data : {0}", DGV.Rows.Count)
        sw.Stop()

        MsgBox(String.Format("Total waktu eksekusi   |   {0:00} menit | {1:00} detik | {2:00} milidetik", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10))

        GC.Collect()
    End Sub
    'karna method async, tambahkan kata async di awal method/sub
    'async berpasangan dengan await
    Private Async Sub BtnImportCSV_Async1_Click(sender As Object, e As EventArgs) Handles BtnImportCSV_Async1.Click

        Dim path As String = "100000 Sales Records.csv"
        DGV.DataSource = Nothing
        sw.Reset()
        sw.Start()

        'event ProgressChanged umumnya sudah menjadi bagian dalam BackroundWorker
        'di .net framework 4.5 kita juga dapat menerapkannya dengan cara 
        'implementasikan IProgress melalui instance dari class Progress
        Dim _Progress As New Progress(Of Tuple(Of Integer, String))(Sub(Data)
                                                                        PB.Value = Data.Item1

                                                                        LblStatus.Text = Data.Item2

                                                                    End Sub)

        'gunakan kata await untuk supaya tidak terjadi blocking pada thread utama
        Dim DT As DataTable = Await TaskImportCSVAsync(path, _Progress)
        DGV.DataSource = DT

        LblStatus.Text = String.Format("Data selesai dimuat   |   Total data : {0}", DGV.Rows.Count)
        sw.Stop()

        MsgBox(String.Format("Total waktu eksekusi   |   {0:00} menit | {1:00} detik | {2:00} milidetik", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10))


        GC.Collect()
    End Sub

    'di bawah ini contoh eksekusi stream dengan teknik asynchronous
    'function yang mengembalikan nilai berupa Task yang berisi DataTable
    Private Function TaskImportCSVAsync(Path As String, Progress As IProgress(Of Tuple(Of Integer, String))) As Task(Of DataTable)
        Try

            'parameter FileStream
            '4096 = default buffer
            'True/False = useAsync = asynchronous / synchronous mode

            Dim T1 As Task(Of DataTable) =
                Task.Run(Async Function()
                             Using FS As New FileStream(Path,
                                              FileMode.Open,
                                              FileAccess.Read,
                                              FileShare.Read,
                                              4096,
                                              True)

                                 Using SR As New StreamReader(FS)

                                     'siapkan variable DT sebagai datatable, yang nantinya jadi output
                                     Using DT As New DataTable

                                         '============== membuat DataColumn ======================
                                         'ekspresi linq
                                         Dim cols() As DataColumn = (From col In (Await SR.ReadLineAsync).Split(",")
                                                                     Select New DataColumn(col, GetType(String))).ToArray

                                         DT.Columns.AddRange(cols)

                                         'ekspresi lambda
                                         'Dim cols() As DataColumn = SR.ReadLine.Split(",").Select(Function(x) New DataColumn(x, GetType(String))).ToArray

                                         'loop tradisional
                                         'For Each x In SR.ReadLine.Split(",")
                                         '    DT.Columns.Add(x, GetType(String))
                                         'Next
                                         '=========================================================================

                                         'tambahkan DataRow sampai akhir baris text/csv
                                         While Not SR.EndOfStream

                                             'DataTable dapat menerima parameter array/list object, 
                                             'langsung saja tambahkan object tersebut, di sini berupa array string
                                             DT.Rows.Add((Await SR.ReadLineAsync).Split(","))

                                             'tampung nilai progressbar
                                             Dim PBValue As Integer = FS.Position / FS.Length * 100

                                             'gunakan method ReportProgress utk mengupdate UI
                                             'method ini akan mentrigger event ProgressChanged
                                             'di dalam event tersebutlah ProgressBar dan label nilainya diupdate
                                             If PBValue Mod 10 = 0 Then
                                                 'atur supaya hanya melaporkan setiap 10 persen, 
                                                 'supaya Thread utama tidak terkuras resourcenya hanya untuk mengupdate ui
                                                 Progress.Report(New Tuple(Of Integer, String)(PBValue, String.Format("Total data : {0}   |   Proses : {1}%", DT.Rows.Count, PBValue)))
                                             End If

                                         End While
                                         Progress.Report(New Tuple(Of Integer, String)(100, "Menampilkan data....."))

                                         Await Task.Delay(500)
                                         'beri jeda 500ms supaya progressbar selesai mengupdate nilai dan animasinya

                                         Return DT


                                     End Using

                                 End Using
                             End Using
                         End Function)

            Return T1

        Catch ex As Exception
            MsgBox(ex.ToString)

            Return Nothing
        End Try

    End Function

    'di bawah ini contoh eksekusi stream dengan teknik synchronous
    'function yang mengembalikan nilai berupa Task yang berisi DataTable
    Private Function TaskImportCSVSync(Path As String, Progress As IProgress(Of Tuple(Of Integer, String))) As Task(Of DataTable)
        Try

            'parameter FileStream
            '4096 = default buffer
            'True/False = useAsync = asynchronous / synchronous mode

            Dim T1 As Task(Of DataTable) =
                Task.Run(Function()
                             Using FS As New FileStream(Path,
                                              FileMode.Open,
                                              FileAccess.Read,
                                              FileShare.Read,
                                              4096,
                                              False)

                                 Using SR As New StreamReader(FS)

                                     'siapkan variable DT sebagai datatable, yang nantinya jadi output
                                     Using DT As New DataTable

                                         '============== membuat DataColumn ======================
                                         'ekspresi linq
                                         Dim cols() As DataColumn = (From col In SR.ReadLine.Split(",")
                                                                     Select New DataColumn(col, GetType(String))).ToArray

                                         DT.Columns.AddRange(cols)

                                         'ekspresi lambda
                                         'Dim cols() As DataColumn = SR.ReadLine.Split(",").Select(Function(x) New DataColumn(x, GetType(String))).ToArray

                                         'loop tradisional
                                         'For Each x In SR.ReadLine.Split(",")
                                         '    DT.Columns.Add(x, GetType(String))
                                         'Next
                                         '=========================================================================

                                         'tambahkan DataRow sampai akhir baris text/csv
                                         While Not SR.EndOfStream

                                             'DataTable dapat menerima parameter array/list object, 
                                             'langsung saja tambahkan object tersebut, di sini berupa array string
                                             DT.Rows.Add(SR.ReadLine.Split(","))

                                             'tampung nilai progressbar
                                             Dim PBValue As Integer = FS.Position / FS.Length * 100

                                             'gunakan method ReportProgress utk mengupdate UI
                                             'method ini akan mentrigger event ProgressChanged
                                             'di dalam event tersebutlah ProgressBar dan label nilainya diupdate
                                             If PBValue Mod 10 = 0 AndAlso Progress IsNot Nothing Then
                                                 'atur supaya hanya melaporkan setiap 10 persen, 
                                                 'supaya Thread utama tidak terkuras resourcenya hanya untuk mengupdate ui
                                                 Progress.Report(New Tuple(Of Integer, String)(PBValue, String.Format("Total data : {0}   |   Proses : {1}%", DT.Rows.Count, PBValue)))
                                             End If

                                         End While
                                         Progress.Report(New Tuple(Of Integer, String)(100, "Menampilkan data....."))

                                         Return DT

                                     End Using

                                 End Using
                             End Using
                         End Function)

            Return T1

        Catch ex As Exception
            MsgBox(ex.ToString)

            Return Nothing
        End Try

    End Function

#End Region
End Class
