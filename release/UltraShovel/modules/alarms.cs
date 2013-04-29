if (Alarms){	Alarms.Unload();	} 

Alarms = {
	SoundAlarmTime = -60,
	EventTime = -tonumber(Settings['RepeatSound']),
	UpdateInventoryTime = -tonumber(Settings['UpdateInventoryTime'])
}

Alarms.Unload = func (){
	Alarms = nil;
}

Alarms.Load = func(){	
	if (Shovel.IsDebug) SysMsg("Alarms loaded");
}

Alarms.Update = func(){
	if (Shovel.IsReady)
	{	
		if (!Shovel.IsTime(Alarms.EventTime, tonumber(Settings['RepeatSound']))) return;
		Alarms.EventTime = Shovel.Now;
		Alarms.CheckDead();
		if(tostring(Settings["InventoryAlarm"]) == 'true') { Alarms.CheckInventory(); }
		if(tostring(Settings["ReportAlarm"]) == 'true') { Alarms.CheckReport(); }
	}
}

Alarms.CheckReport = func(){
	if (IsVisible('autoinspect') == 'YES')
	{
		Alarms.MakeSound('ReportAlarmSound');
		return;
	}
}

Alarms.CheckInventory = func(){
	if (!Shovel.IsTime(Alarms.UpdateInventoryTime , tonumber(Settings['UpdateInventoryTime']))) return;
	
	Alarms.UpdateInventory();
	Alarms.UpdateInventoryTime = Shovel.Now;
	var uiFrame = GetFrameByName('inventory');
	if (!uiFrame) return;
	var count = tonumber(GetTextByKey(uiFrame, 'invitem_count'));
	if (count < 245){return;}
	Alarms.MakeSound('InventoryAlarmSound');
	return;
}

Alarms.CheckDead = func(){
	for(i = 1, Brain.GetCharsCount())
	{
		if(tostring(Settings["DeadAlarmChar" .. i]) == 'true')
		{
			if (Shovel.Actors[i].HP <= 0)
			{
				Alarms.MakeSound('DeadAlarmSound');
				return;
			}
		}
	}
}

Alarms.MakeSound = func(param){
	if (!Shovel.IsTime(Alarms.SoundAlarmTime, 60)) return;
	Alarms.SoundAlarmTime = Shovel.Now;
	SysMsg("Alarm!");
	if (Settings['UseSimpleAlarm'])
	{
		DisableBGM(0);
		DisableVoiceEffect(0);
		DisableSoundEffect(0);
		SetSEVolume(250);
		SetBGMVolume(250);
		SetVCVolume(250);
		pcall(func() { PlayMusic(GetText(GetFrameByName("jukebox"), 'select song'), 1, 0); });
		return;
	}
	else
	{
		var path = Shovel.AiFolder .. Shovel.FSP .. Shovel.SoundsFolder .. Shovel.FSP .. Settings[param] .. '.mp3';
		var link = io.open(path, 'r');
		if (!link) 
		{
			SysMsg('Sound file does not exist');
		}
		else
		{
			os.execute(path);
		}
	}
}

Alarms.UpdateInventory = func(){
	Open('inventory');
	Close('inventory');
}

Alarms.Load();