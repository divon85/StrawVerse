Imports StrawVerse.Core.Models
Imports HtmlAgilityPack

Namespace Providers

    Public Class MangaseeProvider
        Inherits ProviderBase
        Implements IMangaProvider

        Public Overrides ReadOnly Property Name As String = "Mangasee"
        Public Overrides ReadOnly Property BaseUrl As String = "https://mangasee123.com"

        ' ---------------------------------------------------------
        ' 1. SEARCH
        ' ---------------------------------------------------------
        Public Async Function SearchAsync(query As String) _
            As Task(Of List(Of SearchResult)) Implements IMangaProvider.SearchAsync

            Dim url = $"{BaseUrl}/search/?name={Uri.EscapeDataString(query)}"
            Dim html = Await GetStringAsync(url)
            Dim doc = LoadHtml(html)

            Dim results As New List(Of SearchResult)
            Dim nodes = SelectNodes(doc, "//div[@class='row']/div[contains(@class,'col-md-6')]")

            If nodes Is Nothing Then Return results

            For Each item In nodes
                Dim link = item.SelectSingleNode(".//a")
                Dim img = item.SelectSingleNode(".//img")

                results.Add(New SearchResult With {
                    .Title = GetText(link),
                    .Url = CombineUrl(BaseUrl, GetAttr(link, "href")),
                    .ThumbnailUrl = GetAttr(img, "src"),
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

            Dim html = Await GetStringAsync(url)
            Dim doc = LoadHtml(html)

            Dim info As New MangaInfo With {
                .Url = url,
                .Title = GetText(SelectNode(doc, "//h1")),
                .CoverImageUrl = GetAttr(SelectNode(doc, "//img[@class='img-fluid']"), "src")
            }

            Return info
        End Function

        ' ---------------------------------------------------------
        ' 3. CHAPTER LIST
        ' ---------------------------------------------------------
        Public Async Function GetChapterListAsync(manga As MangaInfo) _
            As Task(Of List(Of MangaChapterInfo)) Implements IMangaProvider.GetChapterListAsync

            Dim html = Await GetStringAsync(manga.Url)
            Dim doc = LoadHtml(html)

            Dim chapters As New List(Of MangaChapterInfo)
            Dim nodes = SelectNodes(doc, "//a[contains(@class,'chapter-link')]")

            If nodes Is Nothing Then Return chapters

            For Each ch In nodes
                Dim chapterUrl = CombineUrl(BaseUrl, GetAttr(ch, "href"))
                Dim chapterNum = ExtractChapterNumber(GetText(ch))

                chapters.Add(New MangaChapterInfo With {
                    .ChapterNumber = chapterNum,
                    .Url = chapterUrl
                })
            Next

            Return chapters.OrderBy(Function(c) c.ChapterNumber).ToList()
        End Function

        Private Function ExtractChapterNumber(text As String) As Decimal
            Dim parts = text.Split(" "c)
            Dim num As Decimal
            Decimal.TryParse(parts.Last(), num)
            Return num
        End Function

        ' ---------------------------------------------------------
        ' 4. PAGE URLS
        ' ---------------------------------------------------------
        Public Async Function GetPageUrlsAsync(chapter As MangaChapterInfo) _
            As Task(Of List(Of String)) Implements IMangaProvider.GetPageUrlsAsync

            Dim html = Await GetStringAsync(chapter.Url)
            Dim doc = LoadHtml(html)

            Dim pages As New List(Of String)
            Dim nodes = SelectNodes(doc, "//img[contains(@class,'img-fluid')]")

            If nodes Is Nothing Then Return pages

            For Each img In nodes
                pages.Add(GetAttr(img, "src"))
            Next

            chapter.PageUrls = pages
            Return pages
        End Function

    End Class

End Namespace
