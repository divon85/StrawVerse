Namespace Models

    Public Class MangaChapterInfo
        Public Property ChapterNumber As Decimal
        Public Property Url As String
        Public Property PageUrls As List(Of String)

        Public Sub New()
            PageUrls = New List(Of String)
        End Sub

        Public Overrides Function ToString() As String
            Return $"Chapter {ChapterNumber}"
        End Function
    End Class

End Namespace
