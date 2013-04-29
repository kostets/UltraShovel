if (Shovel) 
{
	var status, rez = pcall(Shovel.Unload);
		if (!status) SysMsg(rez);
} 
else 
{
	Shovel = {
		AIVersion = 		"Release 1.33", 
		Name = 				"UltraShovel",
		Commander = 		GetMyCommanderName(), 
		Region = 			GetNation(), 
		GameStartTime = 	os.time(), 
		Ext = 				".cs",
		LanguagesFolder = 	"languages",
		ModulesFolder =		"modules",
		CharsFolder = 		"chars",
		SoundsFolder = 		"sounds",
		AiFolder = 			"UltraShovel",
		SettingsFolder =	"settings",
		DataFolder =        "database",
		AddonsFolder = 		"addons",
		EOL = 				string.char(13), 
		FSP = 				string.char(92), 
		TAB = 				string.char(9), 
		WINEOL = 			string.char(10),
		Passed = 			0,
		Now =				0,
		Actors = {},
		CharsCounter = 1,
		Chars = {},
		_G_ = { SysMsg = SysMsg , GetNation = GetNation, sleep = sleep},
		IsReady = false,
		IsUnloading = false,
		IsDebug = false,
		Farm = false
	}	
		
	SysMsg = func (text) { Shovel._G_.SysMsg(os.date('[%X] ')..tostring(text)); }
	GetNation = func () { var x = Shovel._G_.GetNation(); if (x == '') return 'SEA'; else return x; }	
	
	Shovel.Region = GetNation();
	
	Shovel.Unload = func (){
		IsUnloading = true;
		ChangeTactics("TS_NONE");
		var k, v;
		if (Shovel._G_) 
		{
			for (k, v in pairs(Shovel._G_)) 
			{
				if (type(v) == 'function') _G[k] = v;
				else _G[k] = nil;
			}
			Shovel._G_ = nil;
		}		
		Close('tutomessage');
		Interface.ShowMini(Text['GoodByeMessage']);		
		Interface.CloseAll();
		System.UnloadModules();
		Shovel.Actors = nil;
		Shovel = nil;
	}

	Shovel.Initialize = func(){
		pcall(dofile, Shovel.AiFolder .. Shovel.FSP .. Shovel.ModulesFolder .. Shovel.FSP .. 'system' .. Shovel.Ext );
		Shovel.LoadAI();
		Close('tutomessage');
		Shovel.Actors[1] = {};
		Shovel.Actors[2] = {};
		Shovel.Actors[3] = {};
		Shovel.CharsCounter = 1;
		ShowAllPcLevel();	
		Settings.LoadFamily();
		SelectAll();
		SetAllSelectMode();
		ChangeTactics("TS_NONE");
	}
		
	Shovel.LoadAI = func(){		
		var k, v;
		for (k, v in pairs(Shovel)) {
			if (type(v) == 'function') 
			{
				if (_G[k] && type(_G[k]) == 'function') 
				{
					if (!Shovel._G_[k]) Shovel._G_[k] = _G[k];	
					
					_G[k] = v;
				} else if (string.sub(k, 1, 4) == 'SCR_') {
					Shovel._G_[k] = true;					
					_G[k] = v;
				}
			}
		}
	}
		
	Shovel.IsTime = func (time, seconds, now) {
		if (time) 
		{
			if (now) return (tonumber(now) - tonumber(time)) >= tonumber(seconds);
			return (tonumber(Shovel.Now) - tonumber(time)) >= tonumber(seconds);
		}
		return false;
	}
	
	sleep = func (ms) {
		if (Shovel.IsUnloading) return;
		Interface.Update();
		Alarms.Update();
		Brain.AutoPotions();
		Shovel.Passed = Shovel.Passed + tonumber(ms);
		if (Shovel.Passed >= 1000) {
			Shovel.Now = os.difftime(os.time(), Shovel.GameStartTime);
			Shovel.Passed = 0;
			if (sGeWithTheShovel) sGeWithTheShovel.Monitoring();
			Addons.Update();
		}
	    Shovel._G_.sleep(tonumber(ms));
	}
	
	Shovel.SCR_TS_NONE = func (self) {	
		Shovel.Prepare(self, "None");
		while (!Shovel.IsReady) sleep(200);
				
		var AiActor = Brain.GetActorBySelf(self);	

		if (Shovel.Farm)
		{
			Shovel.TryReturnToFarm(AiActor);
		}
		
		while(true && !Shovel.IsUnloading && Shovel.IsReady)
		{
			sleep(200);
			Brain.UserTarget(AiActor);
		}
	}
	
	Shovel.SCR_TS_MOVE = func (self) { 		
		Shovel.Prepare(self, "Move");
		while (!Shovel.IsReady) sleep(200);

		var AiActor = Brain.GetActorBySelf(self);	
		
		if (Shovel.Farm)
		{
			Shovel.TryReturnToFarm(AiActor);
		}
				
		while(true && !Shovel.IsUnloading && Shovel.IsReady)
		{	
			sleep(200);
			Brain.UserTarget(AiActor);
		}		
	}

	Shovel.SCR_TS_KEEP = func (self){ 
		Shovel.Prepare(self, "Keep");
		
		while (!Shovel.IsReady) sleep(200);
								
		var AiActor = Brain.GetActorBySelf(self);	
		
		Settings.SaveActor(AiActor);
		if (IsLeader(GetAiActor(self)) == 'YES' )
		{
			Settings.SaveFamily();
		}
		
		while(true && !Shovel.IsUnloading && Shovel.IsReady)
		{	
			sleep(100);
			Brain.AutoAttack(AiActor);
			Brain.AutoPick(AiActor);	
			Brain.AutoItems(AiActor);
			Brain.UserTarget(AiActor);
			Brain.AutoSkill(AiActor);
			Brain.KeepPosition(AiActor);
			Brain.KnockdownProtect(AiActor);
		}
		
	}
			

	Shovel.Prepare = func(self, stance) {
		sleep(100);
		if (Shovel.IsDebug) SysMsg(stance .. " + " .. Brain.GetAiIndex(self));
		Brain.HandleActor(self);
	}
	
	Shovel.TryReturnToFarm = func(Actor)	{	
		if (tonumber(Settings['Mode']) == 0)
		{			
			while (IsNearFromKeepDestPosition(Actor.SelfAi, 50) != 'YES')
			{			
				KeepDestMoveTo(Actor.SelfAi);
				sleep(1000);
			}
			SysMsg('Returned to farm');
			ChangeTacticsAi(Actor.SelfAi, 'TS_KEEP');
		}
		else
		{
			Keep();
		}
	}
	
	Shovel.ReInit = func()	{
		if (Shovel.IsDebug) SysMsg("Re-Initialization");
		for (i = 1,3)
		{ 
			Shovel.Actors[i] = nil;
			Shovel.Actors[i] = {};
		}
		Shovel.IsReady = false;
		Shovel.CharsCounter = 1;
		
		Shovel.Chars = nil;
		Shovel.Chars = {};
		System.LoadCharacters();
		
		SelectAll();
		SetAllSelectMode();
		
		ChangeTactics("TS_NONE");
	}
	
	Shovel.SCR_ATTACKER_TS_NONE = Shovel.SCR_TS_NONE;
	Shovel.SCR_ATTACKER_TS_MOVE = Shovel.SCR_TS_MOVE;
	Shovel.SCR_ATTACKER_TS_KEEP = Shovel.SCR_TS_KEEP;
	Shovel.SCR_HEALER_TS_NONE = Shovel.SCR_TS_NONE;
	Shovel.SCR_HEALER_TS_MOVE = Shovel.SCR_TS_MOVE;
	Shovel.SCR_HEALER_TS_KEEP = Shovel.SCR_TS_KEEP;
	Shovel.SCR_PUPPET_TS_NONE = Shovel.SCR_TS_NONE;
	Shovel.SCR_PUPPET_TS_MOVE = Shovel.SCR_TS_MOVE;
	Shovel.SCR_PUPPET_TS_KEEP = Shovel.SCR_TS_KEEP;
	 
	Shovel.Initialize();	
}