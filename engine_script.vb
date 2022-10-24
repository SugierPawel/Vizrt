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

    Println " >> OnInitParameters() << "
End Sub

Sub OnInit()
	Println " >> OnInit() << "
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
	    DIM prefix AS String = keySplit[0]
	    DIM _key   AS String = keySplit[1]
        
		'zmienna "prefix" przyjmuje wartość: "FOREX" / "POGO" / "SMOG"
		'zmienna "_key"   przyjmuje wartość: np. "USD"
		
	    Println " >> process_" & prefix & " >> key: " & _key
	    Select Case prefix
	        Case "FOREX"
	            process_FOREX(_key, (string)map[key])
	        Case "SMOG"
	            process_SMOG(_key, (string)map[key])
	        Case "POGO"
	            process_POGO(_key, (string)map[key])
	        Case Else
	            Println " >>> key: " & _key & " - nieznany prefix:" & prefix
	    End Select
	End If
End Sub


Sub process_FOREX(key AS String, value AS String)
	'zmienna "key"   przyjmuje wartość: "USD"
	'zmienna "value" przyjmuje wartość: "bid=4.9738;pctchange=-0.3;updated=2022-10-13 13:26:06"
	
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
				
				'zamieniamy zmienną typu "String" na typ "Double" aby móc wykonać funkcję precyzji.
                DIM _CDbl As Double = CDbl(valuePair[1])
				'ustawiamy żądaną precyzję, w wypadku "bid" potrzebujemy 4 miejsca po przecinku.
				valuePair[1] = DoubleToString(_CDbl, 4)
				'zamiana "." na ","
	            valuePair[1].Replace(valuePair[1].Find("."), 1, ",")
				
	        Case "pctchange"
				
				'zamieniamy zmienną typu "String" na typ "Double" aby móc wykonać funkcję precyzji oraz wysterować strzałkę.
                DIM _CDbl As Double = CDbl(valuePair[1])
				'ustawiamy żądaną precyzję, w wypadku "pctchange" potrzebujemy 2 miejsca po przecinku.
				valuePair[1] = DoubleToString(_CDbl, 2)
				'zamiana "." na ","
	            valuePair[1].Replace(valuePair[1].Find("."), 1, ",")
				
				Dim pi_omo As PluginInstance
                pi_omo = Scene.FindContainer(key & "_sel").GetFunctionPluginInstance("Omo")
                
                If _CDbl < 0 Then
		        	pi_omo.SetParameterInt("vis_con", 1)
					Scene.FindContainer(key & "_COLOR").material.Color.SetRgb(255, 0, 0)
                ElseIf _CDbl > 0 Then
                    pi_omo.SetParameterInt("vis_con", 0)
					Scene.FindContainer(key & "_COLOR").material.Color.SetRgb(0, 255, 0)
                ElseIf _CDbl = 0 Then
                    pi_omo.SetParameterInt("vis_con", 2)
					Scene.FindContainer(key & "_COLOR").material.Color.SetRgb(0, 0, 255)
                End If
	        Case "updated"
	    End Select
		
		'zasilenie kontenera "USD_bid"       wartością: "4,9738"
		'zasilenie kontenera "USD_pctchange" wartością: "-0.3"
		Println " >> property:" & valuePair[0] & ", value:" & valuePair[1]
        
        Scene.FindContainer(key & "_" & valuePair[0]).geometry.text = valuePair[1]
    Next
End Sub

Sub process_POGO(key AS String, value AS String)
    'POGO_Szczecin|chmrd=schlch;tempd=16;chmrn=m;tempn=7;chmrj=schm;tempj=16
	
    DIM ricContainer AS Container = Scene.FindContainer(key)
    DIM valueSplit AS Array[String]
    value.split(";", valueSplit)
    For i = 0 to valueSplit.Size - 1
        DIM valuePair AS Array[String]
        valueSplit[i].split("=", valuePair)
        Println " >> property:" & valuePair[0] & ", value:" & valuePair[1]
    Next
End Sub

Sub process_SMOG(key AS String, value AS String)
    'SMOG_Lodz|1
    Println " >> property:" & key & ", value: " & value
End Sub