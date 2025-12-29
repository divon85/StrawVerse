Imports StrawVerse.Core.Models

Namespace Providers

    Public Interface IMangaProvider
        ReadOnly Property Name As String
        ReadOnly Property BaseUrl As String

        Function SearchAsync(query As String) As Task(Of List(Of SearchResult))
        Function GetMangaInfoAsync(url As String) As Task(Of MangaInfo)
        Function GetChapterListAsync(manga As MangaInfo) As Task(Of List(Of MangaChapterInfo))
        Function GetPageUrlsAsync(chapter As MangaChapterInfo) As Task(Of List(Of String))

    End Interface

End Namespace
