Namespace Utils

    Public Interface ILogger
        Sub Info(message As String)
        Sub Warn(message As String)
        Sub [Error](message As String, ex As Exception)
    End Interface

End Namespace
