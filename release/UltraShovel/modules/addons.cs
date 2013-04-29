if (Addons){	Addons.Unload();	} 

Addons = {
		Selected = -1,
		Timer = -1
	}

Addons.Unload = func (){
	Addons = nil;
}

Addons.Load = func(){
	System.LoadFile( Shovel.AiFolder .. Shovel.FSP .. Shovel.AddonsFolder .. Shovel.FSP .. 'addons_list' .. Shovel.Ext );
		
	Addons.Current = {};
	for (k, v in pairs(Addons.List)) 
	{		
		Addons.AssignTextFile(v.File, v.ID);
		System.LoadFile(Shovel.AiFolder .. Shovel.FSP .. Shovel.AddonsFolder .. Shovel.FSP .. v.File .. '.shovel.addon');
	}
	if (Shovel.IsDebug) SysMsg("Addons loaded");
}

Addons.AssignTextFile = func(addonName){

	var f = io.open(Shovel.AiFolder .. Shovel.FSP .. Shovel.LanguagesFolder .. Shovel.FSP .. addonName .. '.' .. Settings['Language'] .. '.shovel.lang');
	if (!f) { return }
	var s, k, v = f:read('*a');
	f:close();
	AddonText = {};
	s = string.gsub(s, '#[^' .. string.char(10) .. ']*', '');
	for (k, v in string.gfind(s, '([%[%]%w]+)%s*=%s*([^' .. string.char(10) .. ']*)')) {
		AddonText[k] = v;		
	}		
	return AddonText;
}

Addons.Set = func(){
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
			Selected = ctlLevel;
		}		
	}
}

Addons.Start = func(){
	for (key, value in Addons.Current)
	{
		if (Selected == value.ListID)
		{
			value.Start();
		}		
	}
}

Addons.Stop = func(){
	for (key, value in Addons.Current)
	{
		if (Selected == value.ListID)
		{
			value.Stop();
		}		
	}
}

Addons.Update = func(){
    if (!Shovel.IsTime(Addons.Timer, 1)) return;
	Addons.Timer = Shovel.Now;
		
	for (key, value in Addons.Current)
	{
		if (value.IsEnabled == true)
		{
			value.MainBody();
		}		
	}
}

Addons.Load();
