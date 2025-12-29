Imports System.Net.Http
Imports System.Text.Json
Imports System.Xml
Imports HtmlAgilityPack

Namespace Providers

    Public MustInherit Class ProviderBase

        Protected ReadOnly Http As HttpClient

        Public Sub New()
            Http = New HttpClient()
            Http.Timeout = TimeSpan.FromSeconds(30)
        End Sub

        ' ------------------------------
        ' HTTP HELPERS
        ' ------------------------------

        Protected Async Function GetStringAsync(url As String) As Task(Of String)
            Try
                Return Await Http.GetStringAsync(url)
            Catch ex As Exception
                Throw New ProviderException($"Failed to GET: {url}", ex)
            End Try
        End Function

        Protected Async Function GetJsonAsync(Of T)(url As String) As Task(Of T)
            Try
                Dim json = Await Http.GetStringAsync(url)
                Return JsonSerializer.Deserialize(Of T)(json)
            Catch ex As Exception
                Throw New ProviderException($"Failed to GET JSON: {url}", ex)
            End Try
        End Function

        ' ------------------------------
        ' HTML PARSING HELPERS
        ' ------------------------------

        Protected Function LoadHtml(html As String) As HtmlDocument
            Dim doc As New HtmlDocument()
            doc.LoadHtml(html)
            Return doc
        End Function

        Protected Function SelectNodes(doc As HtmlDocument, xpath As String) As HtmlNodeCollection
            Return doc.DocumentNode.SelectNodes(xpath)
        End Function

        Protected Function SelectNode(doc As HtmlDocument, xpath As String) As HtmlNode
            Return doc.DocumentNode.SelectSingleNode(xpath)
        End Function

        Protected Function GetAttr(node As HtmlNode, attr As String) As String
            If node Is Nothing Then Return Nothing
            If node.Attributes(attr) Is Nothing Then Return Nothing
            Return node.Attributes(attr).Value.Trim()
        End Function

        Protected Function GetText(node As HtmlNode) As String
            If node Is Nothing Then Return Nothing
            Return node.InnerText.Trim()
        End Function

        ' ------------------------------
        ' URL HELPERS
        ' ------------------------------

        Protected Function CombineUrl(baseUrl As String, relative As String) As String
            If relative.StartsWith("http") Then Return relative
            If baseUrl.EndsWith("/") AndAlso relative.StartsWith("/") Then
                Return baseUrl.TrimEnd("/"c) & relative
            End If
            Return baseUrl.TrimEnd("/"c) & "/" & relative.TrimStart("/"c)
        End Function

        ' ------------------------------
        ' ABSTRACT MEMBERS
        ' ------------------------------

        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property BaseUrl As String

    End Class

End Namespace
