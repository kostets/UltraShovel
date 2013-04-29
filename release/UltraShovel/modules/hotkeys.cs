if (Hotkeys){	Hotkeys.Unload();	} 

Hotkeys = {
	
}

/// <summary>
/// Module entry point.
/// </summary>
Hotkeys.Load = func(){	
	if (Shovel.IsDebug) SysMsg("Hotkeys loaded");
}

/// <summary>
/// Change auto keep mode [on/off].
/// </summary>
Hotkeys.ChangeKeepMode = func()
{	
	Shovel.Farm = !Shovel.Farm;
	if (Shovel.Farm)
	{
		SysMsg(Text['KeepModeOn']);
		//ChangeTactics("TS_KEEP");
	}
	else
	{
		SysMsg(Text['KeepModeOff']);
		ChangeTactics("TS_NONE");
	}
}

Hotkeys.Load();