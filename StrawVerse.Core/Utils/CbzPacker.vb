Imports System.IO
Imports System.IO.Compression
Imports StrawVerse.Core.Models

Namespace Utils

    Public Class CbzPacker

        ''' <summary>
        ''' Creates a CBZ file from a list of downloaded image files.
        ''' </summary>
        Public Shared Sub CreateCbz(outputPath As String, imageFiles As List(Of String))
            If File.Exists(outputPath) Then
                File.Delete(outputPath)
            End If

            Using zip = ZipFile.Open(outputPath, ZipArchiveMode.Create)
                Dim index As Integer = 1

                For Each img In imageFiles
                    If File.Exists(img) Then
                        Dim entryName = $"{index:D4}{Path.GetExtension(img)}"
                        zip.CreateEntryFromFile(img, entryName, CompressionLevel.NoCompression)
                        index += 1
                    End If
                Next
            End Using
        End Sub

    End Class

End Namespace
