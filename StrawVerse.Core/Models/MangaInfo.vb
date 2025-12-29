Namespace Models

    Public Class MangaInfo
        Public Property Title As String
        Public Property Url As String
        Public Property CoverImageUrl As String
        Public Property Chapters As List(Of MangaChapterInfo)

        Public Sub New()
            Chapters = New List(Of MangaChapterInfo)
        End Sub
    End Class

End Namespace
