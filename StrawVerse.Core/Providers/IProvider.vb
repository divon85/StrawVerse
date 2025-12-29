Namespace Providers

    Public Interface IProvider
        ReadOnly Property Name As String
        ReadOnly Property BaseUrl As String

        Function SearchAsync(query As String) As Task(Of List(Of Models.SearchResult))
        Function GetAnimeInfoAsync(url As String) As Task(Of Models.AnimeInfo)
        Function GetEpisodeListAsync(anime As Models.AnimeInfo) As Task(Of List(Of Models.EpisodeInfo))
        Function GetEpisodeDownloadUrlAsync(episode As Models.EpisodeInfo) As Task(Of String)

    End Interface

End Namespace
