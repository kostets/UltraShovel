if (Interface){	Interface.Unload();	} 

Interface = {
		weaponIndexes = { [1] = 1, [2] = 1, [3] = 1	},
		stancesIndexes = { [1] = 1, [2] = 1, [3] = 1 },
		SkillsWindow = 'skills' .. Settings['Language'],
		PotionsWindow = 'potions' .. Settings['Language'],
		SettingsWindow = 'settings' .. Settings['Language'],
		AlarmsWindow = 'alarms' .. Settings['Language'],
		MenuWindow = 'menu' .. Settings['Language'],
		ItemsWindow = 'items' .. Settings['Language'],
		AddonsWindow = 'addons' .. Settings['Language'],
		NewsWindow = 'news' .. Settings['Language'],
		AboutWindow = 'about' .. Settings['Language']
	}	

Interface.Load = func(){
	Close('accountinfo');
	Interface.LoadWindow('menu');
	var text = Shovel.Commander .. ', ' .. Text['WelcomeMessage'] .. '\n';
	text = text .. Text['Ver'] .. ': ' .. Shovel.AIVersion .. '\n';
	text = text .. Text['Region'] .. ': ' .. Shovel.Region .. '\n\n\n';
	text = text .. Text['AutoKeep'] .. '\n';
	text = text .. 'Чтобы сменить язык, перейдите в Настройки.\n';
	text = text .. 'If you want to change language, then go to Settings.\n';
	System.Log(Interface.MenuWindow , text);
	if (Shovel.IsDebug) SysMsg("Interface loaded");
}

Interface.Unload = func(){
	Interface = nil;
}
	
Interface.CloseAll = func(){
	Close(Interface.SkillsWindow);
	Close(Interface.PotionsWindow);
	Close(Interface.SettingsWindow);
	Close(Interface.AlarmsWindow);
	Close(Interface.MenuWindow);
	Close(Interface.ItemsWindow);
	Close(Interface.AddonsWindow);
	Close(Interface.NewsWindow);
	Close(Interface.AboutWindow);
}

Interface.ShowMini = func(message){
	Open('accountinfo');
	var frame = GetFrameByName('accountinfo');
	if (!frame) return;
	SetTextByKey(frame, 'msg1', message);
}

Interface.LoadWindow = func(window){
	Interface.CloseAll();	
	
	if (window == 'menu') Interface.LoadMenuWindow();
	else if (window == 'skills') Interface.LoadSkillsWindow();
	else if (window == 'potions') Interface.LoadPotionsWindow();
	else if (window == 'alarms') Interface.LoadAlarmsWindow();
	else if (window == 'settings') Interface.LoadSettingsWindow();
	else if (window == 'items') Interface.LoadItemsWindow();
	else if (window == 'addons') Interface.LoadAddonsWindow();
	else if (window == 'news') Interface.LoadNewsWindow();
	else if (window == 'about') Interface.LoadAboutWindow();
	else SysMsg("NOT READY YET");
}

Interface.LoadMenuWindow = func(){
	Open(Interface.MenuWindow);
	var uiFrame = GetFrameByName(Interface.MenuWindow);
	if (!uiFrame) return;

	SetTextByKey(uiFrame, 'as' , Text['AutoSkills']);
	SetTextByKey(uiFrame, 'ai' , Text['Potions']);
	SetTextByKey(uiFrame, 'al' , Text['Alarms']);
	SetTextByKey(uiFrame, 'se' , Text['Settings']);	
	SetTextByKey(uiFrame, 'it' , Text['Items']);
	SetTextByKey(uiFrame, 'ne' , Text['News']);
	SetTextByKey(uiFrame, 'ad' , Text['Addons']);
	SetTextByKey(uiFrame, 'ab' , Text['About']);
	SetTextByKey(uiFrame, 'lo' , Text['LogWin']);
}

Interface.LoadSettingsWindow = func(){
	Open(Interface.SettingsWindow);
	var uiFrame = GetFrameByName(Interface.SettingsWindow);
	if (!uiFrame) return;
	for (i = 1, Brain.GetCharsCount()) 
	{
		SetTextByKey(uiFrame, 'n' .. i , Shovel.Actors[i].Name);
		Interface.UpdateWindow(i, Interface.SettingsWindow);
	}
	SetTextByKey(uiFrame, 'k' , Text['KeepRange']);
	SetTextByKey(uiFrame, 'p' , Text['PickRange']);
	SetTextByKey(uiFrame, 'kv' , Settings['KeepRange']);
	SetTextByKey(uiFrame, 'pv' , Settings['PickRange']);	
	SetTextByKey(uiFrame, 'a', Text['AutoAttack']);
	SetTextByKey(uiFrame, 'm', Text['Mode']);
	SetTextByKey(uiFrame, 'capb', Text['CallAtBuff']);
	SetTextByKey(uiFrame, 'lang', Text['Language']);
	SetTextByKey(uiFrame, 'ru', Text['Rus']);
	SetTextByKey(uiFrame, 'en', Text['Eng']);
	var uiCheckBox = GetCheckBox(uiFrame, 'CallAtPartyBuff');
	var value = 0;
	if (tostring(Settings['CallAtPartyBuff']) == 'true') value = 1;
	SetCheck(uiCheckBox, value);	
	if (tonumber(Settings['Mode']) == 0)
	{
		SetTextByKey(uiFrame, 're', Text['ReturnEach']);
		SetText(uiFrame, "eac", Settings['ReturnTime']);
	}
	else 
	{
		SetTextByKey(uiFrame, 're', Text['CallEach']);
		SetText(uiFrame, "eac", Settings['CallTime']);
	}
}

Interface.LoadAlarmsWindow = func(){
	Open(Interface.AlarmsWindow);
	var uiFrame = GetFrameByName(Interface.AlarmsWindow);
	if (!uiFrame) return;
	for (i = 1, Brain.GetCharsCount()) 
	{
		SetTextByKey(uiFrame, 'n' .. i , Shovel.Actors[i].Name);
	}
	SetTextByKey(uiFrame, 'ra' , Text['ReportAlarm']);
	SetTextByKey(uiFrame, 'ia' , Text['InventoryAlarm']);
	SetTextByKey(uiFrame, 'da' , Text['DeadAlarm']);
	SetTextByKey(uiFrame, 'sa' , Text['UseSimpleAlarm']);
	SetTextByKey(uiFrame, 'sa2' , Text['UseSimpleAlarm2']);
	SetText(uiFrame, "ReportAlarmSound", Settings['ReportAlarmSound']);
	SetText(uiFrame, "InventoryAlarmSound", Settings['InventoryAlarmSound']);
	SetText(uiFrame, "DeadAlarmSound", Settings['DeadAlarmSound']);
	
	var uiCheckBox = GetCheckBox(uiFrame, "usa");
	var value = 0;
	if (tostring(Settings["UseSimpleAlarm"]) == 'true') value = 1;
	SetCheck(uiCheckBox, value);
	
	Interface.UpdateWindow(0 , Interface.AlarmsWindow);
}

Interface.LoadPotionsWindow = func(){
	Open(Interface.PotionsWindow);
	var uiFrame = GetFrameByName(Interface.PotionsWindow);
	if (!uiFrame) return;
	for (i = 1, Brain.GetCharsCount()) 
	{
		SetTextByKey(uiFrame, 'n' .. i , Shovel.Actors[i].Name);
		SetTextByKey(uiFrame, 'h' .. i , Text['UseHP']);
		SetTextByKey(uiFrame, 'm' .. i , Text['UseMP']);
		SetTextByKey(uiFrame, 'sh' .. i , Text['HPlvl']);
		SetTextByKey(uiFrame, 'sm' .. i , Text['MPlvl']);
		Interface.UpdateWindow(i, Interface.PotionsWindow);
	}
}

Interface.LoadSkillsWindow = func(){
	Open(Interface.SkillsWindow);
	var uiFrame = GetFrameByName(Interface.SkillsWindow);
	if (!uiFrame) return;

	for (i = 1, Brain.GetCharsCount()) 
	{
		SetTextByKey(uiFrame, 'n' .. i , Shovel.Actors[i].Name);
		SetTextByKey(uiFrame, 'j' .. i , Shovel.Actors[i].Job);
		Interface.UpdateWindow(i , Interface.SkillsWindow);
	}
}

Interface.LoadItemsWindow = func(){
	Open(Interface.ItemsWindow);
	var uiFrame = GetFrameByName(Interface.ItemsWindow);
	if (!uiFrame) return;
	for (i = 1, Brain.GetCharsCount()) 
	{
		SetTextByKey(uiFrame, 'n' .. i , Shovel.Actors[i].Name);
		Interface.UpdateWindow(i, Interface.ItemsWindow);
	}
	
	SetTextByKey(uiFrame, 'ai' , Text['AutoBullets']);
	SetTextByKey(uiFrame, 'ait' , Text['AutoItems']);
	SetTextByKey(uiFrame, 'c1' , Text['Characters']);
	SetTextByKey(uiFrame, 'c2' , Text['BulletType']);
	SetTextByKey(uiFrame, 'c3' , Text['NumButton']);
	SetTextByKey(uiFrame, 'sec' , Text['Sec']);
	SetTextByKey(uiFrame, 're' , Text['UseEach']);
	
	for (i = 1, 7) 
	{
		SetNumber(uiFrame, "Item" .. i, Settings["Item" .. i]);
		SetText(uiFrame, "autoitem" .. i .. "_edit", Settings["Item" .. i .. "Time"]);
		var uiCheckBox = GetCheckBox(uiFrame, "autoitem" .. i .. "_check");
		var value = 0;
		if (tostring(Settings["ItemUse" .. i]) == 'true') value = 1;
		SetCheck(uiCheckBox, value);
	}
}

Interface.LoadAddonsWindow = func(){
	Open(Interface.AddonsWindow);
	var uiFrame = GetFrameByName(Interface.AddonsWindow);
	if (!uiFrame) return;
	
	SetTextByKey(uiFrame, 't' , Text['AddonList']);
}

Interface.LoadNewsWindow = func(){
	Open(Interface.NewsWindow);
}

Interface.LoadAboutWindow = func(){
	Open(Interface.AboutWindow);
}

Interface.SetSettings = func (type, param, charIndex){
	if (tostring(type) == 'slider')
	{
		var uiFrame = GetFrameByName(Interface.SettingsWindow);
		if (!uiFrame) return;
		var uiControl = GetControl(uiFrame, param);
		var ctlLevel = GetNumber(uiControl);
		Settings[param] = ctlLevel;	
		Interface.UpdateWindow(0 , Interface.SettingsWindow);
	}
	
	if (tostring(type) == 'check')
	{
		if (tostring(Settings[param]) == 'true')
		{
			Settings[param] = 'false';
		}
		else 
		{
			Settings[param] = 'true';
		}
	}
	
	if (tostring(type) == 'dropbox')
	{
		var uiFrame = GetFrameByName(Interface.ItemsWindow);
		if (!uiFrame) return;
		var uiControl = GetControl(uiFrame, param);
		var ctlLevel = GetNumber(uiControl);
		Settings[param] = ctlLevel;
	}
	
	if (tostring(type) == 'checkactor')
	{
		var index = tonumber(charIndex);
		if (tostring(param) == 'bullets')
		{
			if (tostring(Shovel.Actors[index].UseBullets) == 'true')
			{
				Shovel.Actors[index].UseBullets = 'false';
			}
			else
			{
				Shovel.Actors[index].UseBullets = 'true';
			}
		}
		if (tostring(param) == 'bullets' .. tostring(charIndex))
		{
			var uiFrame = GetFrameByName(Interface.ItemsWindow);
			if (!uiFrame) return;
			var uiControl = GetControl(uiFrame, param);
			var ctlLevel = GetNumber(uiControl);
			Shovel.Actors[index].BulletsID = Shovel.Ammo[ctlLevel].ID;
		}
		if (tostring(param) == 'numpad' .. tostring(charIndex))
		{
			var uiFrame = GetFrameByName(Interface.ItemsWindow);
			if (!uiFrame) return;
			var uiControl = GetControl(uiFrame, param);
			var ctlLevel = GetNumber(uiControl);
			Shovel.Actors[index].NumPadButton = ctlLevel;
		}
		if (tostring(param) == 'autoattack' .. tostring(charIndex))
		{
			if (tostring(Shovel.Actors[index].AutoAttack) == 'true')
			{
				Shovel.Actors[index].AutoAttack = 'false';
			}
			else
			{
				Shovel.Actors[index].AutoAttack = 'true';
			}
		}		
	}
	
	if (tostring(type) == 'mode')
	{
		var uiFrame = GetFrameByName(Interface.SettingsWindow);
		if (!uiFrame) return;
		var text = GetText(uiFrame, 'eac');
		if (Settings['Mode'] == 0){
			Settings['ReturnTime'] = tonumber(text);
		}
		else{
			Settings['CallTime'] = tonumber(text);			
		}
	}
	
	if (tostring(type) == 'savesounds')
	{
		var uiFrame = GetFrameByName(Interface.AlarmsWindow);
		if (!uiFrame) return;
		var text = GetText(uiFrame, 'ReportAlarmSound');
		Settings['ReportAlarmSound'] = text;
		text = GetText(uiFrame, 'InventoryAlarmSound');
		Settings['InventoryAlarmSound'] = text;
		text = GetText(uiFrame, 'DeadAlarmSound');
		Settings['DeadAlarmSound'] = text;
	}
	
	if (tostring(type) == 'lang')
	{
		var uiFrame = GetFrameByName(Interface.SettingsWindow);
		if (!uiFrame) return;
		Settings["Language"] = tostring(param);
		Settings.ChangeLanguage(tostring(param));
		
		var uiCheckBoxRu = GetCheckBox(uiFrame, "rus");
		var uiCheckBoxEn = GetCheckBox(uiFrame, "eng");
		if (tostring(Settings["Language"]) == 'rus')
		{
			SetCheck(uiCheckBoxRu, 1);
			SetCheck(uiCheckBoxEn, 0);
		}
		else
		{
			SetCheck(uiCheckBoxRu, 0);
			SetCheck(uiCheckBoxEn, 1);
		}
		
		Settings.SaveFamily();
		
		MsgBoxYesNo('Reboot AI (press F12 twice)\nПерезагрузите бота (нажмите F12 дважды)', 'Change lang / Изменить язык', 'None', 'None');
	}
}

Interface.SwitchAll = func(type , window){
	if (tostring(window) == 'potions')
	{
		if(tostring(type) == 'on')
		{
			for (i = 1, Brain.GetCharsCount())
			{				
				Settings['UseHPChar' .. i] = 'true';
				Settings['UseMPChar' .. i] = 'true';
				Interface.UpdateWindow(i, Interface.PotionsWindow);
			}
			
		}
		if(tostring(type) == 'off')
		{
			for (i = 1, Brain.GetCharsCount())
			{				
				Settings['UseHPChar' .. i] = 'false';
				Settings['UseMPChar' .. i] = 'false';
				Interface.UpdateWindow(i, Interface.PotionsWindow);
			}
		}
		if(tostring(type) == 'levels')
		{
			for (i = 1, Brain.GetCharsCount())
			{
				Shovel.Actors[i].HPlevel = 99;
				Shovel.Actors[i].MPlevel = 99;
				Interface.UpdateWindow(i, Interface.PotionsWindow);
			}
		}
	}
}

Interface.Slide = func(type, charIndex){
	var index = tonumber(charIndex);
	var uiFrame = GetFrameByName(Interface.PotionsWindow);
	if (!uiFrame) return;
	var uiControl = GetControl(uiFrame, 'HPlevel' .. index);
	var ctlLevel = GetNumber(uiControl);
	Shovel.Actors[index].HPlevel = ctlLevel;
	
	var uiControl = GetControl(uiFrame, 'MPlevel' .. index);
	var ctlLevel = GetNumber(uiControl);
	Shovel.Actors[index].MPlevel = ctlLevel;
	Interface.UpdateWindow(index, Interface.PotionsWindow);
}

Interface.ChangeWeapon = func(type , charIndex){
	var index = tonumber(charIndex);
	if (Shovel.Actors[index].Weapons == nil) return;
	var max = table.getn(Shovel.Actors[index].Weapons);
	Interface.stancesIndexes[index] = 1;
	if (tostring(type) == 'next')
	{
		if (Interface.weaponIndexes[index] < max)
		{
			Interface.weaponIndexes[index] = Interface.weaponIndexes[index] + 1;
		}
	}
	if (tostring(type) == 'prev')
	{	
		if (Interface.weaponIndexes[index] > 1)
		{
			Interface.weaponIndexes[index] = Interface.weaponIndexes[index] - 1;
		}
	}
	
	Interface.UpdateWindow(index, Interface.SkillsWindow);
}

Interface.ChangeStance = func(type , charIndex){
	var index = tonumber(charIndex);
	if (Shovel.Actors[index].Weapons == nil) return;
	var currentWeapon = Shovel.Actors[index].Weapons[Interface.weaponIndexes[index]];
	var max = table.getn(currentWeapon.Stances);

	if (tostring(type) == 'next')
	{
		if (Interface.stancesIndexes[index] < max)
		{
			Interface.stancesIndexes[index] = Interface.stancesIndexes[index] + 1;
		}
	}
	if (tostring(type) == 'prev')
	{	
		if (Interface.stancesIndexes[index] > 1)
		{
			Interface.stancesIndexes[index] = Interface.stancesIndexes[index] - 1;
		}
	}
	
	Interface.UpdateWindow(index, Interface.SkillsWindow);
}

Interface.ChangeSkillUse = func(charIndex, skillIndex){
	var index = tonumber(charIndex);	
	if (Shovel.Actors[index].Weapons == nil) return;
	var sindex = tonumber(skillIndex);
	var currentStanceIndex = Shovel.Actors[index].Weapons[Interface.weaponIndexes[index]].Stances[Interface.stancesIndexes[index]];
	if (tostring(Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].InUse) == 'true')
	{
		Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].InUse = 'false';
	}
	else
	{
		Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].InUse = 'true';
	}
}

Interface.ChangePotionsUse = func(type, charIndex){
	var index = tonumber(charIndex);
	if (tostring(type) == 'hp')
	{
		if(tostring(Settings['UseHPChar' .. index]) == 'true')
		{
			Settings['UseHPChar' .. index] = 'false';
		}
		else
		{
			Settings['UseHPChar' .. index] = 'true';
		}
	}
	if (tostring(type) == 'mp')
	{
		if(tostring(Settings['UseMPChar' .. index]) == 'true')
		{
			Settings['UseMPChar' .. index] = 'false';
		}
		else
		{
			Settings['UseMPChar' .. index] = 'true';
		}
	}
}

Interface.ChangeTime = func(type, charIndex, skillIndex){

	var index = tonumber(charIndex);	
	if (Shovel.Actors[index].Weapons == nil) return;
	var sindex = tonumber(skillIndex);
	var currentStanceIndex = Shovel.Actors[index].Weapons[Interface.weaponIndexes[index]].Stances[Interface.stancesIndexes[index]];
	if (tostring(type) == 'up')
	{	
		if (Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Timing < 
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Cooldown * 40)
		{
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Timing = 
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Timing + 
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Cooldown;
		}
	}
	if (tostring(type) == 'down')
	{
		if (Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Timing >
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Cooldown)
		{
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Timing = 
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Timing - 
			Shovel.Actors[index].Stances[currentStanceIndex].Skills[sindex].Cooldown;
		}
	}
	
	Interface.UpdateWindow(index, Interface.SkillsWindow);
}

Interface.UpdateWindow = func(charIndex , window){
	var index = tonumber(charIndex);
	if (window == Interface.SkillsWindow){
		var uiFrame = GetFrameByName(Interface.SkillsWindow);
		if (!uiFrame) return;
		if (Shovel.Actors[index].Weapons == nil) return;
		var currentWeapon = Shovel.Actors[index].Weapons[Interface.weaponIndexes[index]].Name;
		var currentStanceIndex = Shovel.Actors[index].Weapons[Interface.weaponIndexes[index]].Stances[Interface.stancesIndexes[index]];
		var currentStance = Shovel.Actors[index].Stances[currentStanceIndex].Name;
		var currentJob = Shovel.Actors[index].Job;
		
		if (currentWeapon == nil) currentWeapon = 'Empty';
		if (currentStance == nil) currentStance = 'Empty';
		
		currentJob = string.gsub(currentJob,'%s', '');
		
		SetTextByKey(uiFrame, 'w' .. tostring(index) , Text[currentWeapon]);
		SetTextByKey(uiFrame, 's' .. tostring(index) , Text[currentStance]);
		for (i = 1, 6) 
		{
			SetTextByKey(uiFrame, 'pic' .. tostring(index) .. i, 'eatingpeopleisfun.ru/shovel_ai/' .. currentJob .. currentStance .. '_' .. (i-1) .. '.jpg');	
		}	
		Open(Interface.SkillsWindow);
		
		for (i = 1, 6) 
		{
			var uiCheckBox = GetCheckBox(uiFrame, 'check' .. index .. i);
			var value = 0;
			if (tostring(Shovel.Actors[index].Stances[currentStanceIndex].Skills[i].InUse) == 'true') value = 1;
			SetCheck(uiCheckBox, value);	
			
			SetTextByKey(uiFrame, 't' .. tostring(index) .. tostring(i), ' ' .. tostring(Shovel.Actors[index].Stances[currentStanceIndex].Skills[i].Timing));
		}
	}
	
	if (window == Interface.PotionsWindow){	
		var uiFrame = GetFrameByName(Interface.PotionsWindow);
		var percent = Shovel.Actors[index].MaxHP / 100;				
		var current = math.floor(Shovel.Actors[index].HPlevel * percent) .. ' HP';
		SetTextByKey(uiFrame, 'hl' .. index , current);
		
		var percent = Shovel.Actors[index].MaxMP / 100;		
		var current = math.floor(Shovel.Actors[index].MPlevel * percent) .. ' MP';
		SetTextByKey(uiFrame, 'ml' .. index , current);	

		var uiCheckBox = GetCheckBox(uiFrame, 'useHP' .. index);
		var value = 0;
		if (tostring(Settings['UseHPChar' .. index]) == 'true') value = 1;
		SetCheck(uiCheckBox, value);	
		
		var uiCheckBox = GetCheckBox(uiFrame, 'useMP' .. index);
		var value = 0;
		if (tostring(Settings['UseMPChar' .. index]) == 'true') value = 1;
		SetCheck(uiCheckBox, value);	
	}
	
	if (window == Interface.SettingsWindow){
		var uiFrame = GetFrameByName(Interface.SettingsWindow);
		if (!uiFrame) return;
		SetTextByKey(uiFrame, 'kv' , Settings['KeepRange']);
		SetTextByKey(uiFrame, 'pv' , Settings['PickRange']);		
		
		for (i = 1, 3)
		{
			var uiCheckBox = GetCheckBox(uiFrame, 'AutoPickChar' .. i);
			var value = 0;
			if (tostring(Settings['AutoPickChar' .. i]) == 'true') value = 1;
			SetCheck(uiCheckBox, value);	
			
			var uiCheckBox = GetCheckBox(uiFrame, 'AutoAttack' .. i);
			var value = 0;
			if (tostring(Shovel.Actors[i].AutoAttack) == 'true') value = 1;
			SetCheck(uiCheckBox, value);	
		}
		
		var rusLang = 0;
		if (Settings["Language"] == "rus")
		{rusLang = 1;}
		
		var uiCheckBox = GetCheckBox(uiFrame, 'rus');
		SetCheck(uiCheckBox, rusLang);	
		
		var engLang = 0;
		if (Settings["Language"] == "eng")
		{engLang = 1;}
		
		var uiCheckBox = GetCheckBox(uiFrame, 'eng');
		SetCheck(uiCheckBox, engLang);	
	}
	
	if (window == Interface.AlarmsWindow){
		var uiFrame = GetFrameByName(Interface.AlarmsWindow);
		if (!uiFrame) return;	
		
		var uiCheckBox = GetCheckBox(uiFrame, 'RA');
		var value = 0;
		if (tostring(Settings['ReportAlarm']) == 'true') value = 1;
		SetCheck(uiCheckBox, value);	
		var uiCheckBox = GetCheckBox(uiFrame, 'IA');
		var value = 0;
		if (tostring(Settings['InventoryAlarm']) == 'true') value = 1;
		SetCheck(uiCheckBox, value);	
		for (i=1,3)
		{
			var uiCheckBox = GetCheckBox(uiFrame, 'DeadAlarmChar' .. i);
			var value = 0;
			if (tostring(Settings['DeadAlarmChar' .. i]) == 'true') value = 1;
			SetCheck(uiCheckBox, value);	
		}
	}

	if (window == Interface.ItemsWindow){
	
		var uiFrame = GetFrameByName(Interface.ItemsWindow);
		var uiCheckBox = GetCheckBox(uiFrame, 'autoBullets' .. index);
		var value = 0;
		if (tostring(Shovel.Actors[index].UseBullets) == 'true') value = 1;
		SetCheck(uiCheckBox, value);
		
		var bulletsIndex = 0;
		for (ind, ammo in Shovel.Ammo)
		{
			for (key, value in ammo)
			{
				if (tostring(key) == 'ID' &&  Shovel.Actors[index].BulletsID == value)
				{
					bulletsIndex = ind;
				}
			}
		}
		
		SetNumber(uiFrame, "bullets" .. index, bulletsIndex);
		SetNumber(uiFrame, "numpad" .. index, Shovel.Actors[index].NumPadButton);
	}
}

Interface.Update = func(){
	if(IsVisible(Interface.SkillsWindow))
	{
		var uiFrame = GetFrameByName(Interface.SkillsWindow);
		if (!uiFrame) return;
		UpdateUi(uiFrame);
	}
}

Interface.SetAddon = func(){
	var uiFrame = GetFrameByName(Interface.AddonsWindow);
	if (!uiFrame) return;
	var uiControl = GetControl(uiFrame, 'select_addon');
	var ctlLevel = GetNumber(uiControl);
	for (key, value in Addons.Current)
	{
		if (ctlLevel == value.ListID)
		{
			SetTextByKey(uiFrame, 'author' , value.Author);
			SetTextByKey(uiFrame, 'desc' , value.Description);
		}
		
	}
}

Interface.SetMode = func(){	
	var uiFrame = GetFrameByName(Interface.SettingsWindow);
	if (!uiFrame) return;	
	var uiControl = GetControl(uiFrame, 'select_mode');
	var ctlLevel = GetNumber(uiControl);	
	Settings['Mode'] = ctlLevel;
	if (Settings['Mode'] == 0){
		SetTextByKey(uiFrame, 're', Text['ReturnEach']);
		SetText(uiFrame, "eac", Settings['ReturnTime']);
	}
	else{
		SetTextByKey(uiFrame, 're', Text['CallEach']);
		SetText(uiFrame, "eac", Settings['CallTime']);
	}
}

Interface.SetItems = func(){
	for (i = 1, 7) 
	{	
		var uiFrame = GetFrameByName(Interface.ItemsWindow);
		if (!uiFrame) return;	
		var text = GetText(uiFrame, 'autoitem' .. i .. '_edit');
		Settings['Item' .. i .. 'Time'] = tonumber(text); 
	}
}

Interface.Load();


