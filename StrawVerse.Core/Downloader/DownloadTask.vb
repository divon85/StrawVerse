Imports System.Threading

Namespace Downloader

    Public Class DownloadTask
        Implements IDownloadTask

        Private ReadOnly _downloader As HttpDownloader
        Private _cts As CancellationTokenSource

        Public Sub New(sourceUrl As String, destination As String)
            Id = Guid.NewGuid()
            Me.SourceUrl = sourceUrl
            Me.DestinationPath = destination
            _downloader = New HttpDownloader()
        End Sub

        Public ReadOnly Property Id As Guid Implements IDownloadTask.Id
        Public ReadOnly Property SourceUrl As String Implements IDownloadTask.SourceUrl
        Public ReadOnly Property DestinationPath As String Implements IDownloadTask.DestinationPath

        Public Event ProgressChanged(percent As Integer) Implements IDownloadTask.ProgressChanged
        Public Event Completed(success As Boolean, message As String) Implements IDownloadTask.Completed

        Public Async Function StartAsync(ct As CancellationToken) As Task Implements IDownloadTask.StartAsync
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct)

            Try
                Dim progress = New Progress(Of Integer)(Sub(p) RaiseEvent ProgressChanged(p))

                Await _downloader.DownloadAsync(SourceUrl, DestinationPath, progress, _cts.Token)

                RaiseEvent Completed(True, "Download completed")
            Catch ex As OperationCanceledException
                RaiseEvent Completed(False, "Download canceled")
            Catch ex As Exception
                RaiseEvent Completed(False, $"Error: {ex.Message}")
            End Try
        End Function

        Public Sub Cancel() Implements IDownloadTask.Cancel
            _cts?.Cancel()
        End Sub

    End Class

End Namespace
