Namespace Models

    ' Search result list
    Public Class WeebCentralSearchResponse
        Public Property series As List(Of WeebCentralSeriesItem)
    End Class

    ' Single series entry in search
    Public Class WeebCentralSeriesItem
        Public Property id As String
        Public Property title As String
        Public Property cover As String
    End Class

    ' Series detail with chapters
    Public Class WeebCentralSeriesDetail
        Public Property id As String
        Public Property title As String
        Public Property description As String
        Public Property cover As String
        Public Property chapters As List(Of WeebCentralChapterItem)
    End Class

    Public Class WeebCentralChapterItem
        Public Property id As String
        Public Property number As Decimal
        Public Property title As String
    End Class

    ' Chapter pages
    Public Class WeebCentralChapterPages
        Public Property id As String
        Public Property pages As List(Of String)
    End Class

End Namespace
