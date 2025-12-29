Imports System.IO
Imports StrawVerse.Core.Downloader
Imports StrawVerse.Core.Models

Namespace Utils

    Public Class MangaCbzBuilder

        Public Shared Async Function BuildCbzAsync(
            chapter As MangaChapterInfo,
            outputCbzPath As String,
            ct As Threading.CancellationToken
        ) As Task

            Dim downloader As New MangaChapterDownloader()

            ' 1. Download all pages
            Dim tempFolder = Path.GetDirectoryName(outputCbzPath)
            Dim images = Await downloader.DownloadChapterAsync(chapter, tempFolder, ct)

            ' 2. Create CBZ
            CbzPacker.CreateCbz(outputCbzPath, images)

            ' 3. Cleanup temp images
            For Each img In images
                Try
                    File.Delete(img)
                Catch
                End Try
            Next

        End Function

    End Class

End Namespace
