Imports StrawVerse.Core.Models
Imports HtmlAgilityPack

Namespace Providers

    Public Class HianimeProvider
        Inherits ProviderBase
        Implements IProvider

        Public Overrides ReadOnly Property Name As String = "Hianime"
        Public Overrides ReadOnly Property BaseUrl As String = "https://hianime.to"

        ' ---------------------------------------------------------
        ' 1. SEARCH
        ' ---------------------------------------------------------
        Public Async Function SearchAsync(query As String) _
            As Task(Of List(Of SearchResult)) Implements IProvider.SearchAsync

            Dim url = $"{BaseUrl}/search?keyword={Uri.EscapeDataString(query)}"
            Dim html = Await GetStringAsync(url)
            Dim doc = LoadHtml(html)

            Dim results As New List(Of SearchResult)

            Dim nodes = SelectNodes(doc, "//div[@class='flw-item']")
            If nodes Is Nothing Then Return results

            For Each item In nodes
                Dim titleNode = item.SelectSingleNode(".//h3/a")
                Dim imgNode = item.SelectSingleNode(".//img")

                Dim result As New SearchResult With {
                    .Title = GetText(titleNode),
                    .Url = CombineUrl(BaseUrl, GetAttr(titleNode, "href")),
                    .ThumbnailUrl = GetAttr(imgNode, "data-src"),
                    .Type = "anime"
                }

                results.Add(result)
            Next

            Return results
        End Function

        ' ---------------------------------------------------------
        ' 2. ANIME INFO
        ' ---------------------------------------------------------
        Public Async Function GetAnimeInfoAsync(url As String) _
            As Task(Of AnimeInfo) Implements IProvider.GetAnimeInfoAsync

            Dim html = Await GetStringAsync(url)
            Dim doc = LoadHtml(html)

            Dim info As New AnimeInfo With {
                .Url = url,
                .Title = GetText(SelectNode(doc, "//h2[@class='film-name']")),
                .Description = GetText(SelectNode(doc, "//div[@class='film-description']")),
                .CoverImageUrl = GetAttr(SelectNode(doc, "//img[@class='film-poster-img']"), "src")
            }

            Return info
        End Function

        ' ---------------------------------------------------------
        ' 3. EPISODE LIST
        ' ---------------------------------------------------------
        Public Async Function GetEpisodeListAsync(anime As AnimeInfo) _
            As Task(Of List(Of EpisodeInfo)) Implements IProvider.GetEpisodeListAsync

            Dim html = Await GetStringAsync(anime.Url)
            Dim doc = LoadHtml(html)

            Dim episodes As New List(Of EpisodeInfo)

            Dim nodes = SelectNodes(doc, "//ul[@class='episodes']/li/a")
            If nodes Is Nothing Then Return episodes

            For Each epNode In nodes
                Dim ep As New EpisodeInfo With {
                    .EpisodeNumber = Integer.Parse(GetAttr(epNode, "data-number")),
                    .Title = $"Episode {GetAttr(epNode, "data-number")}",
                    .Url = CombineUrl(BaseUrl, GetAttr(epNode, "href"))
                }

                episodes.Add(ep)
            Next

            Return episodes.OrderBy(Function(e) e.EpisodeNumber).ToList()
        End Function

        ' ---------------------------------------------------------
        ' 4. GET DOWNLOAD URL
        ' ---------------------------------------------------------
        Public Async Function GetEpisodeDownloadUrlAsync(episode As EpisodeInfo) _
            As Task(Of String) Implements IProvider.GetEpisodeDownloadUrlAsync

            Dim html = Await GetStringAsync(episode.Url)
            Dim doc = LoadHtml(html)

            ' Hianime uses <a class="btn-download"> links
            Dim node = SelectNode(doc, "//a[contains(@class,'btn-download')]")

            If node Is Nothing Then
                Throw New ProviderException("Download link not found.")
            End If

            Dim downloadUrl = GetAttr(node, "href")
            Return CombineUrl(BaseUrl, downloadUrl)
        End Function

    End Class

End Namespace
