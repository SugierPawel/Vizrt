Sub OnInitParameters()
    Println " >> FOREX - OnInitParameters() << "
	
	DIM requestedKeys As Array[String]
	requestedKeys.push("/FOREX/USD/")
	requestedKeys.push("/FOREX/EUR/")
	requestedKeys.push("/FOREX/CHF/")
	requestedKeys.push("/FOREX/GBP/")
	
	DIM subValues As Array[String]
	subValues.push("bid")
	subValues.push("pctchange")
	
    For i = 0 to requestedKeys.Size - 1
		For j = 0 to subValues.Size - 1
			DIM name As String = requestedKeys[i] & subValues[j]
			if System.Map.ContainsKey(name) = true Then
	        	OnSharedMemoryVariableChanged(System.Map, name)
			End if
			System.Map.RegisterChangedCallback(name)
			Println " >> FOREX - RegisterChangedCallback << - name: " & name
		Next
    Next
End Sub

Sub OnInit()
	Println " >> FOREX - OnInit() << "
End Sub

Sub OnSharedMemoryVariableChanged(map AS SharedMemory, key AS String)
	'FOREX_USD_PLN|bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06
	Println " >> FOREX - OnSharedMemoryVariableChanged << - key: " & key & ", value: " & (string)map[key]
End Sub
