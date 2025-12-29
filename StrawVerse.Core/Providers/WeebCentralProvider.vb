Imports StrawVerse.Core.Models

Namespace Providers

    Public Class WeebCentralProvider
        Inherits ProviderBase
        Implements IMangaProvider

        Public Overrides ReadOnly Property Name As String = "WeebCentral"
        Public Overrides ReadOnly Property BaseUrl As String = "https://weebcentral.com"

        ' ---------------------------------------------------------
        ' 1. SEARCH
        ' ---------------------------------------------------------
        Public Async Function SearchAsync(query As String) _
            As Task(Of List(Of SearchResult)) Implements IMangaProvider.SearchAsync

            ' Example pattern: /api/search?query=...
            Dim url = $"{BaseUrl}/api/search?query={Uri.EscapeDataString(query)}"

            Dim resp = Await GetJsonAsync(Of WeebCentralSearchResponse)(url)
            Dim results As New List(Of SearchResult)

            If resp?.series Is Nothing Then Return results

            For Each s In resp.series
                results.Add(New SearchResult With {
                    .Title = s.title,
                    .Url = $"{BaseUrl}/series/{s.id}/{Uri.EscapeDataString(s.title)}",
                    .ThumbnailUrl = s.cover,
                    .Type = "manga"
                })
            Next

            Return results
        End Function

        ' ---------------------------------------------------------
        ' 2. MANGA INFO
        ' ---------------------------------------------------------
        Public Async Function GetMangaInfoAsync(url As String) _
            As Task(Of MangaInfo) Implements IMangaProvider.GetMangaInfoAsync

            ' Assume URL format: /series/{id}/...
            Dim parts = url.Trim("/"c).Split("/"c)
            Dim seriesId = parts(parts.Length - 2)

            Dim apiUrl = $"{BaseUrl}/api/series/{seriesId}"
            Dim detail = Await GetJsonAsync(Of WeebCentralSeriesDetail)(apiUrl)

            Dim info As New MangaInfo With {
                .Url = url,
                .Title = detail.title,
                .CoverImageUrl = detail.cover
            }

            ' (We don't fill Chapters here; GetChapterListAsync will handle it.)
            Return info
        End Function

        ' ---------------------------------------------------------
        ' 3. CHAPTER LIST
        ' ---------------------------------------------------------
        Public Async Function GetChapterListAsync(manga As MangaInfo) _
            As Task(Of List(Of MangaChapterInfo)) Implements IMangaProvider.GetChapterListAsync

            Dim parts = manga.Url.Trim("/"c).Split("/"c)
            Dim seriesId = parts(parts.Length - 2)

            Dim apiUrl = $"{BaseUrl}/api/series/{seriesId}"
            Dim detail = Await GetJsonAsync(Of WeebCentralSeriesDetail)(apiUrl)

            Dim chapters As New List(Of MangaChapterInfo)

            If detail?.chapters Is Nothing Then Return chapters

            For Each ch In detail.chapters
                chapters.Add(New MangaChapterInfo With {
                    .ChapterNumber = ch.number,
                    .Url = $"{BaseUrl}/chapters/{ch.id}",
                    .PageUrls = New List(Of String)()
                })
            Next

            ' Sort by chapter number ascending
            Return chapters.OrderBy(Function(c) c.ChapterNumber).ToList()
        End Function

        ' ---------------------------------------------------------
        ' 4. PAGE URLS
        ' ---------------------------------------------------------
        Public Async Function GetPageUrlsAsync(chapter As MangaChapterInfo) _
            As Task(Of List(Of String)) Implements IMangaProvider.GetPageUrlsAsync

            ' Assume chapter.Url = /chapters/{id}
            Dim parts = chapter.Url.Trim("/"c).Split("/"c)
            Dim chapterId = parts.Last()

            Dim apiUrl = $"{BaseUrl}/api/chapters/{chapterId}/pages"
            Dim pagesInfo = Await GetJsonAsync(Of WeebCentralChapterPages)(apiUrl)

            Dim pages As List(Of String) = pagesInfo?.pages OrElse New List(Of String)()

            chapter.PageUrls = pages
            Return pages
        End Function

    End Class

End Namespace
