Namespace Models

    Public Class EpisodeInfo
        Public Property EpisodeNumber As Integer
        Public Property Title As String
        Public Property Url As String
        Public Property DownloadUrl As String   ' Filled by provider
        Public Property FileName As String      ' Suggested output name

        Public Overrides Function ToString() As String
            Return $"Episode {EpisodeNumber}: {Title}"
        End Function
    End Class

End Namespace
