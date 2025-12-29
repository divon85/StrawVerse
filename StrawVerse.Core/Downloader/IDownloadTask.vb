Namespace Downloader

    Public Interface IDownloadTask
        ReadOnly Property Id As Guid
        ReadOnly Property SourceUrl As String
        ReadOnly Property DestinationPath As String

        Event ProgressChanged(percent As Integer)
        Event Completed(success As Boolean, message As String)

        Function StartAsync(ct As Threading.CancellationToken) As Task
        Sub Cancel()

    End Interface

End Namespace
