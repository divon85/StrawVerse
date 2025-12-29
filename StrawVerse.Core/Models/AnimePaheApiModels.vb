Namespace Models

    Public Class AnimePaheSearchResponse
        Public Property data As List(Of AnimePaheSearchItem)
    End Class

    Public Class AnimePaheSearchItem
        Public Property title As String
        Public Property session As String
        Public Property poster As String
    End Class

    Public Class AnimePaheEpisodeResponse
        Public Property title As String
        Public Property description As String
        Public Property poster As String
        Public Property data As List(Of AnimePaheEpisodeItem)
        Public Property last_page As Integer
    End Class

    Public Class AnimePaheEpisodeItem
        Public Property episode As Integer
        Public Property session As String
    End Class

End Namespace
