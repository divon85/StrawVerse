Imports System.Collections.Concurrent

Namespace Downloader

    Public Class DownloadQueue
        Implements IDownloadQueue

        Private ReadOnly _tasks As New ConcurrentDictionary(Of Guid, IDownloadTask)
        Private ReadOnly _cts As New Threading.CancellationTokenSource()

        Public Event QueueUpdated() Implements IDownloadQueue.QueueUpdated

        Public ReadOnly Property Tasks As List(Of IDownloadTask) Implements IDownloadQueue.Tasks
            Get
                Return _tasks.Values.ToList()
            End Get
        End Property

        Public Sub Add(task As IDownloadTask) Implements IDownloadQueue.Add
            _tasks.TryAdd(task.Id, task)
            RaiseEvent QueueUpdated()
        End Sub

        Public Sub Remove(taskId As Guid) Implements IDownloadQueue.Remove
            Dim removed As IDownloadTask = Nothing
            _tasks.TryRemove(taskId, removed)
            RaiseEvent QueueUpdated()
        End Sub

        Public Sub StartAll() Implements IDownloadQueue.StartAll
            For Each task In _tasks.Values
                task.Run(Async Function()
                             Await task.StartAsync(_cts.Token)
                         End Function)
            Next
        End Sub

        Public Sub CancelAll() Implements IDownloadQueue.CancelAll
            _cts.Cancel()
        End Sub

    End Class

End Namespace
