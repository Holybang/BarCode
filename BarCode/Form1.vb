Public Class Form1
    Public FinalNow As New Final
    Private Sub ButtonDraw_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDraw.Click
        Dim FinalTemp As New Final
        Dim temp As Integer
        'final1.Num = TextBox2.Text
        Me.Refresh()
        If TextBox3.Text = Nothing Then
            MsgBox("您还未输入条码高度！", MsgBoxStyle.OkOnly, "警告")
            Exit Sub
        End If
        If TextBox4.Text = Nothing Then
            MsgBox("您还未输入条码单位元素宽度！", MsgBoxStyle.OkOnly, "警告")
            Exit Sub
        End If
        If TextBox5.Text = Nothing Then
            MsgBox("您还未输入条码宽窄元素比！", MsgBoxStyle.OkOnly, "警告")
            Exit Sub
        End If
        If ComboBox1.SelectedIndex = -1 Then
            MsgBox("你好像忘了要先选择一种码制哦！", MsgBoxStyle.OkOnly, "提醒")
            Exit Sub
        End If
        temp = 1

        Select Case ComboBox1.Text
            Case Is = "39码全字符集"
                TextBox1.Text = TextBox1.Text.ToUpper
                FinalTemp = Do39()
                Call DrawBarCode(FinalTemp)
            Case Is = "交叉25码"
                FinalTemp = DoCross25()
                Call DrawBarCode(FinalTemp)
            Case Is = "EAN-13码"
                FinalTemp = DoEAN13()
                Call DrawBarCode(FinalTemp)
        End Select
    End Sub
    Private Sub DrawBarCode(ByVal Final1 As Final)
        Dim myPen As New System.Drawing.Pen(System.Drawing.Color.White)
        Dim formGraphics As System.Drawing.Graphics
        Dim i As Short
        Dim dx, dy, x0, y0, n As Single
        finalnow = Final1
        If Final1.FlagError = True Then
            MsgBox("您输入的字符中有当前码制字符库以外的字符！", MsgBoxStyle.OkOnly, "错误")
            Exit Sub
        End If
        dy = Final1.BarHeight
        dx = Final1.BarMinWidth
        n = Final1.BarWidthRatio
        x0 = 20
        y0 = 30
        formGraphics = Me.CreateGraphics()
        If ComboBox1.SelectedIndex = 2 Then
            myPen.Width = dx
            For i = 0 To Final1.Num
                If Final1.Item(i) = True Then
                    myPen.Color = Color.Black
                Else
                    myPen.Color = Color.White
                End If
                If i = 0 Or i = 1 Or i = 2 Or _
                   i = Final1.Num / 2 - 2 Or i = Final1.Num / 2 - 1 Or i = Final1.Num / 2 Or i = Final1.Num / 2 + 1 Or i = Final1.Num / 2 + 2 Or _
                   i = Final1.Num Or i = Final1.Num - 1 Or i = Final1.Num - 2 Then
                    dy = Final1.BarHeight * 22 / 20
                Else
                    dy = Final1.BarHeight
                End If
                formGraphics.DrawLine(myPen, x0, y0, x0, y0 + dy)

                x0 = x0 + myPen.Width
            Next
        Else
            For i = 1 To Final1.Num + 1
                If Final1.Item(i - 1) = True Then
                    myPen.Width = dx * n
                Else
                    myPen.Width = dx
                End If
                x0 = x0 + myPen.Width / 2
                If i Mod 2 = 1 Then
                    myPen.Color = Color.Black
                Else
                    myPen.Color = Color.White
                End If
                formGraphics.DrawLine(myPen, x0, y0, x0, y0 + dy)
                x0 = x0 + myPen.Width / 2
            Next

        End If

        myPen.Dispose()
        formGraphics.Dispose()
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        TextBox2.Text = Len(TextBox1.Text)
        If ComboBox1.SelectedIndex = 2 Then
            If TextBox1.TextLength = 12 Then
                ButtonDraw.Enabled = True
            Else
                ButtonDraw.Enabled = False
            End If
        End If
    End Sub

    Private Function DoCross25() As Final
        Dim TextSource As System.Text.StringBuilder
        Dim TextLen As Integer = TextBox2.Text
        Dim check As Integer
        DoCross25 = New Final
        Dim DataCross25(,) As Boolean = { _
        {0, 0, 1, 1, 0}, _
        {1, 0, 0, 0, 1}, _
        {0, 1, 0, 0, 1}, _
        {1, 1, 0, 0, 0}, _
        {0, 0, 1, 0, 1}, _
        {1, 0, 1, 0, 0}, _
        {0, 1, 1, 0, 0}, _
        {0, 0, 0, 1, 1}, _
        {1, 0, 0, 1, 0}, _
        {0, 1, 0, 1, 0}}
        Dim Counter() As Char = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}
        Dim i As Integer
        TextSource = New System.Text.StringBuilder(TextBox1.Text)
        '计算校验字符'''''''''''''''''
        If CheckBox1.Checked = True Then
            check = 0
            For i = 0 To TextSource.Length - 1
                If i Mod 2 = 0 Then
                    check = Val(TextSource(i)) * 3 + check
                Else
                    check = Val(TextSource(i)) + check
                End If
            Next
            check = 10 - (check Mod 10)
            TextSource.Insert(TextSource.Length, check)
        End If
        ''''''''''''''''''''''''''''''
        '是否添加虚零'''''''''''''''''
        If TextSource.Length Mod 2 = 1 Then
            TextSource.Insert(0, "0")
            'TextBox1.Text = TextSource.ToString
        End If
        ''''''''''''''''''''''''''''''
        '是否显示起始位/中止位
        Dim startp, endp As Integer
        If CheckBox3.Checked = True Then
            DoCross25.RedimLength(TextSource.Length * 5 + 6)
            startp = 4
            endp = DoCross25.Num - 3
            DoCross25.Item(DoCross25.Num - 2) = True  '设置起始符和中止符

        Else
            DoCross25.RedimLength(TextSource.Length * 5 - 1)
            startp = 0
            endp = DoCross25.Num
        End If
        ''''''''''''''''''''''''''''''
        DoCross25.BarWidthRatio = TextBox5.Text
        DoCross25.BarMinWidth = Val(TextBox4.Text) * 3
        DoCross25.BarHeight = Val(TextBox3.Text) * 3

        '检查是否有字符库以外字符'''''
        For i = 0 To TextSource.Length - 1
            If Asc(TextSource(i)) > Asc("9") Or Asc(TextSource(i)) < Asc("0") Then
                DoCross25.FlagError = True
            End If
        Next
        ''''''''''''''''''''''''''''''
        For i = startp To endp
            If (i - startp + 1) Mod 2 = 1 Then
                DoCross25.Item(i) = DataCross25(Val(TextSource(Int((i - startp) / 10))), Int((i - startp - Int((i - startp) / 10) * 10) / 2))
            Else
                DoCross25.Item(i) = DataCross25(Val(TextSource(Int((i - startp) / 10) + 1)), Int((i - startp - Int((i - startp) / 10) * 10) / 2))
            End If
        Next
        If CheckBox2.Checked = True Then
            Me.ShowTextForRead(TextSource.ToString, DoCross25, 0)
        End If
    End Function

    Private Function Do39() As Final
        Dim TextSource As New System.Text.StringBuilder(TextBox1.Text)
        Dim TextLen As Integer = TextBox2.Text
        Dim Data39(,) As Boolean = { _
        {0, 0, 1, 1, 0, 1, 0, 0, 0}, _
        {0, 0, 1, 1, 0, 0, 1, 0, 0}, _
        {1, 0, 0, 0, 1, 0, 1, 0, 0}, _
        {0, 1, 0, 0, 1, 0, 1, 0, 0}, _
        {1, 1, 0, 0, 0, 0, 1, 0, 0}, _
        {0, 0, 1, 0, 1, 0, 1, 0, 0}, _
        {1, 0, 1, 0, 0, 0, 1, 0, 0}, _
        {0, 1, 1, 0, 0, 0, 1, 0, 0}, _
        {0, 0, 0, 1, 1, 0, 1, 0, 0}, _
        {1, 0, 0, 1, 0, 0, 1, 0, 0}, _
        {0, 1, 0, 1, 0, 0, 1, 0, 0}, _
        {1, 0, 0, 0, 1, 0, 0, 1, 0}, _
        {0, 1, 0, 0, 1, 0, 0, 1, 0}, _
        {1, 1, 0, 0, 0, 0, 0, 1, 0}, _
        {0, 0, 1, 0, 1, 0, 0, 1, 0}, _
        {1, 0, 1, 0, 0, 0, 0, 1, 0}, _
        {0, 1, 1, 0, 0, 0, 0, 1, 0}, _
        {0, 0, 0, 1, 1, 0, 0, 1, 0}, _
        {1, 0, 0, 1, 0, 0, 0, 1, 0}, _
        {0, 1, 0, 1, 0, 0, 0, 1, 0}, _
        {0, 0, 1, 1, 0, 0, 0, 1, 0}, _
        {1, 0, 0, 0, 1, 0, 0, 0, 1}, _
        {0, 1, 0, 0, 1, 0, 0, 0, 1}, _
        {1, 1, 0, 0, 0, 0, 0, 0, 1}, _
        {0, 0, 1, 0, 1, 0, 0, 0, 1}, _
        {1, 0, 1, 0, 0, 0, 0, 0, 1}, _
        {0, 1, 1, 0, 0, 0, 0, 0, 1}, _
        {0, 0, 0, 1, 1, 0, 0, 0, 1}, _
        {1, 0, 0, 1, 0, 0, 0, 0, 1}, _
        {0, 1, 0, 1, 0, 0, 0, 0, 1}, _
        {0, 0, 1, 1, 0, 0, 0, 0, 1}, _
        {1, 0, 0, 0, 1, 1, 0, 0, 0}, _
        {0, 1, 0, 0, 1, 1, 0, 0, 0}, _
        {1, 1, 0, 0, 0, 1, 0, 0, 0}, _
        {0, 0, 1, 0, 1, 1, 0, 0, 0}, _
        {1, 0, 1, 0, 0, 1, 0, 0, 0}, _
        {0, 1, 1, 0, 0, 1, 0, 0, 0}, _
        {0, 0, 0, 1, 1, 1, 0, 0, 0}, _
        {1, 0, 0, 1, 0, 1, 0, 0, 0}, _
        {0, 1, 0, 1, 0, 1, 0, 0, 0}, _
        {0, 0, 0, 0, 0, 1, 1, 1, 0}, _
        {0, 0, 0, 0, 0, 1, 1, 0, 1}, _
        {0, 0, 0, 0, 0, 1, 0, 1, 1}}
        Dim Counter() As Char = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", _
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", _
        "U", "V", "W", "X", "Y", "Z", "-", "·", " ", "$", "/", "+", "%"}
        Dim CharTemp As Char
        Dim i, j, k, n, check As Short
        check = 0
        Do39 = New Final
        Do39.BarWidthRatio = TextBox5.Text
        Do39.BarMinWidth = Val(TextBox4.Text) * 3
        Do39.BarHeight = Val(TextBox3.Text) * 3
        If CheckBox1.Checked = False Then
            Do39.RedimLength((TextLen + 2) * 10 - 2)
        Else
            Do39.RedimLength((TextLen + 3) * 10 - 2)
        End If
        For i = 1 To 9
            If i Mod 2 = 1 Then
                Do39.Item(i - 1) = Data39(0, (i - 1) / 2)
                Do39.Item(Do39.Num - 9 + i) = Data39(0, (i - 1) / 2)
            Else
                Do39.Item(i - 1) = Data39(0, 4 + i / 2)
                Do39.Item(Do39.Num - 9 + i) = Data39(0, 4 + i / 2)
            End If
        Next
        Do39.Item(9) = False
        i = 0
        k = 10
        While (i < TextLen And k < Do39.Num - 9)

            CharTemp = TextSource(i)
            For j = 1 To Len(Counter)
                If CharTemp = Counter(j - 1) Then
                    check = check + j - 1
                    Exit For
                End If
            Next
            If j > Len(Counter) Then
                Do39.FlagError = True
                Exit Function
            End If
            For n = 1 To 9
                If n Mod 2 = 1 Then
                    Do39.Item(k) = Data39(j, (n - 1) / 2)
                Else
                    Do39.Item(k) = Data39(j, 4 + n / 2)
                End If
                k = k + 1
            Next
            Do39.Item(k) = False
            k = k + 1
            i = i + 1
        End While
        check = check Mod 43
        If CheckBox1.Checked = True Then
            TextSource.Insert(TextSource.Length, Counter(check), 1)
            For n = 1 To 9
                If n Mod 2 = 1 Then
                    Do39.Item(k) = Data39(check, (n - 1) / 2)
                Else
                    Do39.Item(k) = Data39(check, 4 + n / 2)
                End If
                k = k + 1
            Next
            Do39.Item(k) = False
        End If
        If CheckBox2.Checked = True Then
            Me.ShowTextForRead(TextSource.ToString, Do39, 1)
        End If
    End Function

    Private Function DoEAN13() As Final
        Dim DataEAN13(,) As Boolean = { _
        {0, 0, 0, 1, 1, 0, 1}, _
        {0, 0, 1, 1, 0, 0, 1}, _
        {0, 0, 1, 0, 0, 1, 1}, _
        {0, 1, 1, 1, 1, 0, 1}, _
        {0, 1, 0, 0, 0, 1, 1}, _
        {0, 1, 1, 0, 0, 0, 1}, _
        {0, 1, 0, 1, 1, 1, 1}, _
        {0, 1, 1, 1, 0, 1, 1}, _
        {0, 1, 1, 0, 1, 1, 1}, _
        {0, 0, 0, 1, 0, 1, 1}, _
        {0, 1, 0, 0, 1, 1, 1}, _
        {0, 1, 1, 0, 0, 1, 1}, _
        {0, 0, 1, 1, 0, 1, 1}, _
        {0, 1, 0, 0, 0, 0, 1}, _
        {0, 0, 1, 1, 1, 0, 1}, _
        {0, 1, 1, 1, 0, 0, 1}, _
        {0, 0, 0, 0, 1, 0, 1}, _
        {0, 0, 1, 0, 0, 0, 1}, _
        {0, 0, 0, 1, 0, 0, 1}, _
        {0, 0, 1, 0, 1, 1, 1}, _
        {1, 1, 1, 0, 0, 1, 0}, _
        {1, 1, 0, 0, 1, 1, 0}, _
        {1, 1, 0, 1, 1, 0, 0}, _
        {1, 0, 0, 0, 0, 1, 0}, _
        {1, 0, 1, 1, 1, 0, 0}, _
        {1, 0, 0, 1, 1, 1, 0}, _
        {1, 0, 1, 0, 0, 0, 0}, _
        {1, 0, 0, 0, 1, 0, 0}, _
        {1, 0, 0, 1, 0, 0, 0}, _
        {1, 1, 1, 0, 1, 0, 0}}
        Dim The13th(,) As Boolean = { _
        {1, 1, 1, 1, 1, 1}, _
        {1, 1, 0, 1, 0, 0}, _
        {1, 1, 0, 0, 1, 0}, _
        {1, 1, 0, 0, 0, 1}, _
        {1, 0, 1, 1, 0, 0}, _
        {1, 0, 0, 1, 1, 0}, _
        {1, 0, 0, 0, 1, 1}, _
        {1, 0, 1, 0, 1, 0}, _
        {1, 0, 1, 0, 0, 1}, _
        {1, 0, 0, 1, 0, 1}}
        Dim check As Single = 0
        Dim i, j, JO As Integer
        Dim TextSource As New System.Text.StringBuilder(TextBox1.Text)
        DoEAN13 = New Final
        DoEAN13.BarHeight = Val(TextBox3.Text) * 3
        DoEAN13.BarWidthRatio = TextBox5.Text
        DoEAN13.BarMinWidth = Val(TextBox4.Text) * 3
        DoEAN13.RedimLength(12 * 7 + 6 + 4)
        JO = Val(TextBox1.Text(0))
        '以下为计算校验值'''''''''''''''''''''
        Dim sumo, sumj As Integer
        sumo = 0
        sumj = 0
        For i = 0 To 11
            If i Mod 2 = 1 Then
                sumo = sumo + Val(TextSource(i))
            Else
                sumj = sumj + Val(TextSource(i))
            End If
        Next
        sumo = sumo * 3
        sumj = sumj + sumo
        If sumj Mod 10 <> 0 Then
            check = 10 - (sumj Mod 10)
        Else
            check = 0
        End If
        TextSource.Insert(TextSource.Length, check)
        '''''''''''''''''''''''''''''''''''''
        For i = 0 To TextSource.Length - 1
            If Asc(TextSource(i)) > Asc("9") Or Asc(TextSource(i)) < Asc("0") Then
                DoEAN13.FlagError = True
            End If
        Next
        If DoEAN13.FlagError = False Then
            For i = 3 To 44
                DoEAN13.Item(i) = DataEAN13(Val(TextSource(Int((i - 3) / 7) + 1)) + 10 * (1 + (The13th(JO, Int((i - 3) / 7)))), (i - 3) - 7 * Int((i - 3) / 7))
            Next
            For i = 50 To 91
                DoEAN13.Item(i) = DataEAN13(Val(TextSource(Int((i - 8) / 7) + 1)) + 20, (i - 8) - 7 * Int((i - 8) / 7))
            Next
            For i = 0 To 2
                DoEAN13.Item(i) = (i + 1) Mod 2
            Next
            For i = 92 To 94
                DoEAN13.Item(i) = (i + 1) Mod 2
            Next
            For i = 45 To 49
                DoEAN13.Item(i) = (i + 1) Mod 2
            Next
        End If
        '以下为绘制条码的程序

        FinalNow = DoEAN13
        If CheckBox2.Checked = True Then
            Me.ShowTextForRead(TextSource.ToString, DoEAN13, 2)
        End If
    End Function

    Private Sub ShowTextForRead(ByVal TextForRead As String, ByVal Final0 As Final, ByVal Mode As Short)
        Dim TextGraphics As System.Drawing.Graphics
        Dim myFont As Font
        Dim i As Short
        Dim WidthPerUnit, Startb, Midb As Integer
        TextGraphics = Me.CreateGraphics
        myFont = New Font(Me.Font.FontFamily, Final0.BarHeight * 2 / 9, FontStyle.Regular, GraphicsUnit.Pixel)
        If Final0.BarHeight + myFont.Size + 30 > ComboBox1.Location.Y Then
            MsgBox("当前条码高度超过可显示的最大高度，建议修改之后或者在新窗口中重新显示。", MsgBoxStyle.OkOnly, "警告")
        End If
        Select Case Mode
            Case Is = 0  '交叉25码
                WidthPerUnit = Final0.BarMinWidth * (3 + 2 * Final0.BarWidthRatio)

                If WidthPerUnit < myFont.Size Then
                    myFont = New Font(Me.Font.FontFamily, WidthPerUnit, FontStyle.Regular, GraphicsUnit.Pixel)
                End If
                If CheckBox3.Checked = True Then
                    Startb = 4 * Final0.BarMinWidth
                Else
                    Startb = 0
                End If
                If TextForRead.Length * WidthPerUnit + (Startb + 2 * Final0.BarMinWidth * (2 + Final0.BarWidthRatio)) + 20 > Me.Width Then
                    MsgBox("当前条码宽度超过窗口宽度！建议更改单位元素宽度或者在新窗口中重新显示。", MsgBoxStyle.OkOnly, "警告")
                End If
                For i = 0 To Len(TextForRead) - 1
                    TextGraphics.DrawString(TextForRead(i), myFont, Brushes.Black, 20 + Startb + (WidthPerUnit - myFont.Size) / 2 + i * WidthPerUnit, 30 + Final0.BarHeight)
                Next
            Case Is = 1  '39码
                WidthPerUnit = Final0.BarMinWidth * (7 + 3 * Final0.BarWidthRatio)
                If WidthPerUnit < myFont.Size Then
                    myFont = New Font(Me.Font.FontFamily, WidthPerUnit, FontStyle.Regular, GraphicsUnit.Pixel)
                End If
                If (TextForRead.Length + 2) * WidthPerUnit + 20 > Me.Width Then
                    MsgBox("当前条码宽度超过窗口宽度！建议更改单位元素宽度或者在新窗口中重新显示。", MsgBoxStyle.OkOnly, "警告")
                End If
                For i = 0 To Len(TextForRead) - 1
                    TextGraphics.DrawString(TextForRead(i), myFont, Brushes.Black, 20 + (WidthPerUnit - myFont.Size) / 2 + (i + 1) * WidthPerUnit, 30 + Final0.BarHeight)
                Next
            Case Is = 2  'EAN-13码
                WidthPerUnit = Final0.BarMinWidth * 7
                If WidthPerUnit < myFont.Size Then
                    myFont = New Font(Me.Font.FontFamily, WidthPerUnit, FontStyle.Regular, GraphicsUnit.Pixel)
                End If
                Startb = 3 * Final0.BarMinWidth
                Midb = 5 * Final0.BarMinWidth
                If TextForRead.Length * WidthPerUnit + (Startb * 2 + Midb) + 20 > Me.Width Then
                    MsgBox("当前条码宽度超过窗口宽度！建议更改单位元素宽度或者在新窗口中重新显示。", MsgBoxStyle.OkOnly, "警告")
                End If
                TextGraphics.DrawString(TextForRead(0), myFont, Brushes.Black, 20 - myFont.Size, 30 + Final0.BarHeight)
                For i = 1 To Len(TextForRead) - 1
                    If i = 7 Then
                        Startb = Startb + Midb
                    End If
                    TextGraphics.DrawString(TextForRead(i), myFont, Brushes.Black, 20 + Startb + (WidthPerUnit - myFont.Size) / 2 + (i - 1) * WidthPerUnit, 30 + Final0.BarHeight)
                Next
        End Select
        myFont.Dispose()
        TextGraphics.Dispose()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D
        Me.AcceptButton = ButtonDraw
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Select Case ComboBox1.SelectedIndex
            Case Is = 0
                ButtonDraw.Enabled = True
                CheckBox1.Checked = True
                CheckBox1.Enabled = True
                TextBox5.Enabled = True
                CheckBox3.Enabled = True
                CheckBox3.Checked = True
            Case Is = 1
                ButtonDraw.Enabled = True
                CheckBox1.Enabled = True
                CheckBox1.Checked = True
                TextBox5.Enabled = True
                CheckBox3.Checked = True
                CheckBox3.Enabled = False
            Case Is = 2
                TextBox5.Enabled = False
                CheckBox1.Checked = True
                CheckBox1.Enabled = False
                CheckBox3.Checked = True
                CheckBox3.Enabled = False
                If TextBox1.TextLength <> 12 Then
                    MsgBox("当前输入的字符数不是12，这样无法进行EAN-13码的绘制！", MsgBoxStyle.OkOnly, "警告")
                    If MsgBox("单击确定按钮将自动输入默认测试用字符，单击取消按钮将由您继续输入。", MsgBoxStyle.OkCancel, "提示") = MsgBoxResult.Ok Then
                        TextBox1.Text = "690123456789"
                    Else
                        ButtonDraw.Enabled = False
                    End If

                End If
        End Select
    End Sub

    Private Sub Form1_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        If FinalNow.Edited = True Then
            Me.DrawBarCode(FinalNow)
        End If
    End Sub

    Private Sub 关于ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 关于ToolStripMenuItem.Click
        FormAbout.Show()
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged
        Dim i, flag As Integer
        Dim MyString() As Char = TextBox3.Text.ToCharArray
        flag = 0
        For i = 0 To TextBox3.Text.Length - 1
            If Asc(MyString(i)) > Asc("9") Or Asc(MyString(i)) < Asc("0") Then
                flag = 1
                MyString(i) = ""
            End If
            If flag = 1 Then
                MsgBox("这里您只能输入数字！", MsgBoxStyle.OkOnly, "警告")
                TextBox3.Text = MyString
            End If
        Next
    End Sub

    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox4.TextChanged
        Dim i, flag As Integer
        Dim MyString() As Char = TextBox4.Text.ToCharArray
        flag = 0
        For i = 0 To TextBox4.Text.Length - 1
            If Asc(MyString(i)) > Asc("9") Or Asc(MyString(i)) < Asc("0") Then
                flag = 1
                MyString(i) = ""
            End If
            If flag = 1 Then
                MsgBox("这里您只能输入数字！", MsgBoxStyle.OkOnly, "警告")
                TextBox4.Text = MyString
            End If
        Next
    End Sub

    Private Sub TextBox5_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox5.TextChanged
        Dim i, flag As Integer
        Dim MyString() As Char = TextBox5.Text.ToCharArray
        flag = 0
        For i = 0 To TextBox5.Text.Length - 1
            If Asc(MyString(i)) > Asc("9") Or Asc(MyString(i)) < Asc("0") Then
                flag = 1
                MyString(i) = ""
            End If
            If flag = 1 Then
                MsgBox("这里您只能输入数字！", MsgBoxStyle.OkOnly, "警告")
                TextBox5.Text = MyString
            End If
        Next
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        FormShowBar.Visible = True

    End Sub
End Class
