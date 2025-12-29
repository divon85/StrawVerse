Namespace Downloader

    Public Interface IDownloadQueue
        Event QueueUpdated()

        Sub Add(task As IDownloadTask)
        Sub Remove(taskId As Guid)
        Sub StartAll()
        Sub CancelAll()

        ReadOnly Property Tasks As List(Of IDownloadTask)

    End Interface

End Namespace
