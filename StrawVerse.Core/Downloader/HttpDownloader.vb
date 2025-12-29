Imports System.Net.Http
Imports System.IO

Namespace Downloader

    Public Class HttpDownloader

        Private Shared ReadOnly _client As HttpClient = New HttpClient()

        Public Async Function DownloadAsync(
            url As String,
            destination As String,
            progress As IProgress(Of Integer),
            ct As Threading.CancellationToken
        ) As Task

            Using response = Await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct)
                response.EnsureSuccessStatusCode()

                Dim totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault()
                Dim downloadedBytes As Long = 0

                Using input = Await response.Content.ReadAsStreamAsync(ct)
                    Using output = File.Create(destination)

                        Dim buffer(8191) As Byte
                        Dim bytesRead As Integer

                        Do
                            bytesRead = Await input.ReadAsync(buffer, 0, buffer.Length, ct)
                            If bytesRead = 0 Then Exit Do

                            Await output.WriteAsync(buffer, 0, bytesRead, ct)
                            downloadedBytes += bytesRead

                            If totalBytes > 0 Then
                                Dim percent = CInt((downloadedBytes / totalBytes) * 100)
                                progress?.Report(percent)
                            End If
                        Loop

                    End Using
                End Using
            End Using

        End Function

    End Class

End Namespace
