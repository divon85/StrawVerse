Imports System.Text.Json
Imports StrawVerse.Core.Models

Namespace Providers

    Public Class AnimePaheProvider
        Inherits ProviderBase
        Implements IProvider

        Public Overrides ReadOnly Property Name As String = "AnimePahe"
        Public Overrides ReadOnly Property BaseUrl As String = "https://animepahe.ru"

        ' ---------------------------------------------------------
        ' 1. SEARCH
        ' ---------------------------------------------------------
        Public Async Function SearchAsync(query As String) _
            As Task(Of List(Of SearchResult)) Implements IProvider.SearchAsync

            Dim url = $"{BaseUrl}/api?m=search&q={Uri.EscapeDataString(query)}"
            Dim json = Await GetStringAsync(url)

            Dim doc = JsonSerializer.Deserialize(Of AnimePaheSearchResponse)(json)
            Dim results As New List(Of SearchResult)

            If doc Is Nothing OrElse doc.data Is Nothing Then Return results

            For Each item In doc.data
                results.Add(New SearchResult With {
                    .Title = item.title,
                    .Url = $"{BaseUrl}/anime/{item.session}",
                    .ThumbnailUrl = item.poster,
                    .Type = "anime"
                })
            Next

            Return results
        End Function

        ' ---------------------------------------------------------
        ' 2. ANIME INFO
        ' ---------------------------------------------------------
        Public Async Function GetAnimeInfoAsync(url As String) _
            As Task(Of AnimeInfo) Implements IProvider.GetAnimeInfoAsync

            Dim session = url.Split("/"c).Last()
            Dim apiUrl = $"{BaseUrl}/api?m=release&id={session}&sort=episode_asc"

            Dim json = Await GetStringAsync(apiUrl)
            Dim doc = JsonSerializer.Deserialize(Of AnimePaheEpisodeResponse)(json)

            Dim info As New AnimeInfo With {
                .Url = url,
                .Title = doc.title,
                .Description = doc.description,
                .CoverImageUrl = doc.poster
            }

            Return info
        End Function

        ' ---------------------------------------------------------
        ' 3. EPISODE LIST
        ' ---------------------------------------------------------
        Public Async Function GetEpisodeListAsync(anime As AnimeInfo) _
            As Task(Of List(Of EpisodeInfo)) Implements IProvider.GetEpisodeListAsync

            Dim session = anime.Url.Split("/"c).Last()
            Dim page = 1
            Dim episodes As New List(Of EpisodeInfo)

            While True
                Dim apiUrl = $"{BaseUrl}/api?m=release&id={session}&sort=episode_asc&page={page}"
                Dim json = Await GetStringAsync(apiUrl)
                Dim doc = JsonSerializer.Deserialize(Of AnimePaheEpisodeResponse)(json)

                If doc Is Nothing OrElse doc.data Is Nothing Then Exit While

                For Each ep In doc.data
                    episodes.Add(New EpisodeInfo With {
                        .EpisodeNumber = ep.episode,
                        .Title = $"Episode {ep.episode}",
                        .Url = $"{BaseUrl}/play/{session}/{ep.session}"
                    })
                Next

                If doc.last_page <= page Then Exit While
                page += 1
            End While

            Return episodes
        End Function

        ' ---------------------------------------------------------
        ' 4. GET DOWNLOAD URL
        ' ---------------------------------------------------------
        Public Async Function GetEpisodeDownloadUrlAsync(episode As EpisodeInfo) _
            As Task(Of String) Implements IProvider.GetEpisodeDownloadUrlAsync

            Dim html = Await GetStringAsync(episode.Url)
            Dim doc = LoadHtml(html)

            ' AnimePahe uses <a data-src="..."> for video sources
            Dim node = SelectNode(doc, "//a[@data-src]")

            If node Is Nothing Then
                Throw New ProviderException("Download URL not found.")
            End If

            Return GetAttr(node, "data-src")
        End Function

    End Class

End Namespace
