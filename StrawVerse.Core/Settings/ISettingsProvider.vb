Namespace Settings

    Public Interface ISettingsProvider
        Function Load() As AppSettings
        Sub Save(settings As AppSettings)
    End Interface

End Namespace
