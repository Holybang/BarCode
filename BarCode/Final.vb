Public Class Final
    Public Num As Integer
    Public Item(40) As Boolean
    Public FlagError As Boolean
    Public BarWidthRatio As Single
    Public BarMinWidth As Single
    Public BarHeight As Single
    Public Edited As Boolean

    Public Sub RedimLength(ByVal NewLength As Integer)
        ReDim Preserve Item(NewLength)
        Num = NewLength
        Edited = True
    End Sub
    Public Sub New()
        Num = 40
        FlagError = False
        BarWidthRatio = 3
        BarMinWidth = 5
        BarHeight = 20
        Edited = False
    End Sub
End Class


