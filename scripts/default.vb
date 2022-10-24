Sub OnInitParameters()
	DIM allKeys AS Array[String]
    'pobieramy wszystkie wartości SHM do zmiennej typu "Array"
    VizCommunication.Map.GetKeys(allKeys)
    For i = 0 to allKeys.Size - 1
        'dla każdego klucza w SHM wyzwalamy funkcję "OnSharedMemoryVariableChanged"
        OnSharedMemoryVariableChanged(VizCommunication.Map, allKeys[i])
    Next
    'rejestrujemy funkcję typu "callbacl" dla SHM
	VizCommunication.Map.RegisterChangedCallback("")

    Println " >> BackgroundScene - OnInitParameters() << "
End Sub

Sub OnInit()
	Println " >> BackgroundScene - OnInit() << "
End Sub


Sub OnSharedMemoryVariableChanged(map AS SharedMemory, key AS String)
	'FOREX_USD_PLN|bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06
	
	'zmienna "key"      przyjmuje wartość: "FOREX_USD_PLN" 
	'zmienna "map[key]" przyjmuje wartość: "bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06" 
	
    DIM keySplit AS Array[String]
	key.split("_", keySplit)
	If keySplit.Size < 2 Then
		Println " >> key: " & key & ", value: " & (string)map[key]
	Else
	    DIM prefix     AS String = keySplit[0]
	    DIM _key       AS String = keySplit[1]
        DIM systemName As String = prefix & "/" & _key

		'zmienna "prefix"      przyjmuje wartość: "FOREX" / "POGO" / "SMOG"
		'zmienna "_key"        przyjmuje wartość: np. "USD"
		'zmienna "systemName"  przyjmuje wartość: np. "FOREX/USD"
		
	    Println " >> process_" & prefix & " << - key: " & _key & ", value: " & (string)map[key]
	    Select Case prefix
	        Case "FOREX"
	            process_FOREX(systemName, (string)map[key])
	        Case "SMOG"
	            process_SMOG(systemName, (string)map[key])
	        Case "POGO"
	            process_POGO(systemName, (string)map[key])
	        Case Else
	            Println " >> BackgroundScene - process_" & prefix & " << - key: " & _key & ", value: " & (string)map[key] & " - nieznany prefix:" & prefix
	    End Select
	End If
End Sub


Sub process_FOREX(systemName AS String, value AS String)
    'zmienna "systemName" przyjmuje wartość: "FOREX/USD"
	'zmienna "value"      przyjmuje wartość: "bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06"
	
    DIM valueSplit AS Array[String]
    value.split(";", valueSplit)
	
	'zmienna "valueSplit[0]" przyjmuje wartość: "bid=4.9738"
	'zmienna "valueSplit[1]" przyjmuje wartość: "pctchange=-0.3"
	'zmienna "valueSplit[2]" przyjmuje wartość: "updated=2022-10-13 13:26:06"
	
    For i = 0 to valueSplit.Size - 1
        DIM valuePair AS Array[String]
        valueSplit[i].split("=", valuePair)
        
		'zmienna "valuePair[0]" przyjmuje wartość: "bid"
		'zmienna "valuePair[1]" przyjmuje wartość: "4.9738"
		
		Select Case valuePair[0]
	        Case "bid"
				
				'zamiana typu "String" na typ "Double" aby móc wykonać funkcję precyzji.
                DIM _CDbl As Double = CDbl(valuePair[1])
				'ustawiamy żądaną precyzję, w wypadku "bid" potrzebujemy 4 miejsca po przecinku.
				valuePair[1] = DoubleToString(_CDbl, 4)
				'zamiana "." na ","
	            'valuePair[1].Replace(valuePair[1].Find("."), 1, ",")
				
	        Case "pctchange"
				
				'zamiana typu "String" na typ "Double" aby móc wykonać funkcję precyzji.
                DIM _CDbl As Double = CDbl(valuePair[1])
				'ustawiamy żądaną precyzję, w wypadku "pctchange" potrzebujemy 2 miejsca po przecinku.
				valuePair[1] = DoubleToString(_CDbl, 2)
				'zamiana "." na ","
	            'valuePair[1].Replace(valuePair[1].Find("."), 1, ",")
				
	        Case "updated"
	    End Select
		
		'zasilenie kontenera "/FOREX/USD/bid"       wartością: "4,9738"
		'zasilenie kontenera "/FOREX/USD/pctchange" wartością: "-0.3"
		DIM key As String = "/" & systemName & "/" & valuePair[0]
		System.Map[key] = valuePair[1]
		
		Println " >> BackgroundScene - WRITE key:" & key & ", value:" & valuePair[1]
    Next
End Sub

Sub process_POGO(systemName AS String, value AS String)
    'POGO_Szczecin|chmrd=schlch;tempd=16;chmrn=m;tempn=7;chmrj=schm;tempj=16
End Sub

Sub process_SMOG(systemName AS String, value AS String)
    'SMOG_Lodz|1
End Sub

