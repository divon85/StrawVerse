Imports System.IO
Imports StrawVerse.Core.Models
Imports StrawVerse.Core.Utils

Namespace Downloader

    Public Class MangaChapterDownloader

        Private ReadOnly _http As New HttpDownloader()

        Public Async Function DownloadChapterAsync(
            chapter As MangaChapterInfo,
            outputFolder As String,
            ct As Threading.CancellationToken
        ) As Task(Of List(Of String))

            Dim tempDir = Path.Combine(outputFolder, $"chapter_{chapter.ChapterNumber}")
            Directory.CreateDirectory(tempDir)

            Dim downloaded As New List(Of String)
            Dim index As Integer = 1

            For Each pageUrl In chapter.PageUrls
                Dim ext = Path.GetExtension(pageUrl)
                Dim filePath = Path.Combine(tempDir, $"{index:D4}{ext}")

                Dim progress = New Progress(Of Integer)(Sub(p) ' ignore per-page progress
                                                        End Sub)

                Await _http.DownloadAsync(pageUrl, filePath, progress, ct)

                downloaded.Add(filePath)
                index += 1
            Next

            Return downloaded
        End Function

    End Class

End Namespace
