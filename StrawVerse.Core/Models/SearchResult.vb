Namespace Models

    Public Class SearchResult
        Public Property Title As String
        Public Property Url As String
        Public Property ThumbnailUrl As String
        Public Property Type As String   ' "anime", "manga", etc.

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

End Namespace
