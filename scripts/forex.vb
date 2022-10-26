Sub OnInitParameters()
	Log("OnInitParameters()")

	DIM rics As Array[String]
	rics.push("USD")
	rics.push("EUR")
	rics.push("CHF")
	rics.push("GBP")

	For i = 0 to rics.Size - 1
		DIM ric As String = rics[i]
		DIM container As Container = Scene.FindContainer(ric)
		If container.Name <> ric Then
			Log("pomijam kontener: " & container.Name)
		Else
			DIM shmKeyName As String = "FOREX_" & ric & "_PLN"
			VizCommunication.Map.RegisterChangedCallback(shmKeyName)
			Log("rejestruję shmKeyName: " & shmKeyName)

			if VizCommunication.Map.ContainsKey(shmKeyName) = true Then
				OnSharedMemoryVariableChanged(VizCommunication.Map, shmKeyName)
			End if
		End If
    Next
End Sub

Sub OnInit()
	Log("OnInit()")
End Sub

Sub OnSharedMemoryVariableChanged(map AS SharedMemory, key AS String)
	'FOREX_USD_PLN|bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06
	
	'zmienna "key"      przyjmuje wartość: "FOREX_USD_PLN" 
	'zmienna "map[key]" przyjmuje wartość: "bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06" 
	
    DIM keySplit AS Array[String]
	key.split("_", keySplit)
	If keySplit.Size < 2 Then
		Log(" >> key: " & key & ", value: " & (string)map[key])
	Else
	    DIM prefix AS String = keySplit[0]
	    DIM _key   AS String = keySplit[1]

		'zmienna "prefix"      przyjmuje wartość: "FOREX" / "POGO" / "SMOG"
		'zmienna "_key"        przyjmuje wartość: np. "USD"
		
	    Log("OnSharedMemoryVariableChanged - key: " & _key & ", value: " & (string)map[key])
	    process_FOREX(_key, (string)map[key])
	End If
End Sub

Sub process_FOREX(key AS String, values AS String)
    'zmienna "key"    przyjmuje wartość: "USD"
	'zmienna "values" przyjmuje wartość: "bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06"
	
    DIM valuesSplit AS Array[String]
    values.split(";", valuesSplit)
	
	'zmienna "valuesSplit[0]" przyjmuje wartość: "bid=4.9738"
	'zmienna "valuesSplit[1]" przyjmuje wartość: "pctchange=-0.3"
	'zmienna "valuesSplit[2]" przyjmuje wartość: "updated=2022-10-13 13:26:06"
	
    For i = 0 to valuesSplit.Size - 1
        DIM valuePair AS Array[String]
        valuesSplit[i].split("=", valuePair)
        
		'zmienna "valuePair[0]" przyjmuje wartość: "bid"
		'zmienna "valuePair[1]" przyjmuje wartość: "4.9738"
		
		'zamiana typu "String" na typ "Double" aby móc wykonać funkcję precyzji.
		DIM _CDbl As Double = CDbl(valuePair[1])
		DIM container As Container = Scene.FindContainer(key)

		Select Case valuePair[0]
	        Case "bid"
				If valuePair[0] = "bid" Then
					'ustawiamy żądaną precyzję, w wypadku "bid" potrzebujemy 4 miejsca po przecinku.
					valuePair[1] = DoubleToString(_CDbl, 4)
				Elseif valuePair[0] = "pctchange" Then
					'ustawiamy żądaną precyzję, w wypadku "bid" potrzebujemy 2 miejsca po przecinku.
					valuePair[1] = DoubleToString(_CDbl, 2)
				End If
				'zamiana "." na ","
				valuePair[1].Replace(valuePair[1].Find("."), 1, ",")
				container.FindSubContainer(valuePair[0]).Geometry.Text = valuePair[1]
			Case "pctchange"
				If _CDbl < 0 Then
					container.FindSubContainer("pctchange").FindSubContainer("negative").Active = true
					container.FindSubContainer("pctchange").FindSubContainer("positive").Active = false
					container.FindSubContainer("pctchange").FindSubContainer("no-change").Active = false
                ElseIf _CDbl > 0 Then
					container.FindSubContainer("pctchange").FindSubContainer("negative").Active = false
					container.FindSubContainer("pctchange").FindSubContainer("positive").Active = true
					container.FindSubContainer("pctchange").FindSubContainer("no-change").Active = false
                ElseIf _CDbl = 0 Then
					container.FindSubContainer("pctchange").FindSubContainer("negative").Active = false
					container.FindSubContainer("pctchange").FindSubContainer("positive").Active = false
					container.FindSubContainer("pctchange").FindSubContainer("no-change").Active = true
                End If
				
				If valuePair[0] = "bid" Then
					'ustawiamy żądaną precyzję, w wypadku "bid" potrzebujemy 4 miejsca po przecinku.
					valuePair[1] = DoubleToString(_CDbl, 4)
				Elseif valuePair[0] = "pctchange" Then
					'ustawiamy żądaną precyzję, w wypadku "bid" potrzebujemy 2 miejsca po przecinku.
					valuePair[1] = DoubleToString(_CDbl, 2)
				End If
				'zamiana "." na ","
				valuePair[1].Replace(valuePair[1].Find("."), 1, ",")
				container.FindSubContainer(valuePair[0]).Geometry.Text = valuePair[1]
	        Case "updated"
	    End Select
		Log("WRITE key:" & key & ", " & valuePair[0] & " = " & valuePair[1])
    Next
End Sub

Sub Log(msg As String)
	PrintLn " >> FOREX << " & msg
	'Scene.AddLogMessage(" >> FOREX << " & msg)
End Sub
