Namespace Models

    Public Class AnimeInfo
        Public Property Title As String
        Public Property Url As String
        Public Property Description As String
        Public Property CoverImageUrl As String
        Public Property Episodes As List(Of EpisodeInfo)

        Public Sub New()
            Episodes = New List(Of EpisodeInfo)
        End Sub
    End Class

End Namespace
