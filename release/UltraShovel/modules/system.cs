if (System){	System.Unload();	} 

System = {}

System.Unload = func (){
	System = nil;
}

System.Load = func(){	
	System.LoadModules();
	System.LoadCharacters();
	System.LoadData();
	if (Shovel.IsDebug) SysMsg("System loaded");
}

System.Log = func(frameName, text){
	var uiFrame = GetFrameByName(frameName);
	if (!uiFrame) return;
	SetTextByKey(uiFrame, 'l', text);
}

System.LoadFile = func(module){
	var status, error = pcall (dofile, module);
	if (!status) SysMsg(tostring(error));
}

System.LoadModules = func(){
	for (k, v in pairs(System.Modules)) 
	{
		System.LoadFile( Shovel.AiFolder .. Shovel.FSP .. Shovel.ModulesFolder .. Shovel.FSP .. v .. Shovel.Ext );
	}
}

System.LoadData = func(){
	System.LoadFile( Shovel.AiFolder .. Shovel.FSP .. Shovel.DataFolder .. Shovel.FSP .. 'Ammo' .. Shovel.Ext );
}

System.UnloadModules = func(){
	Interface = nil;
	Brain = nil;
	Alarms = nil;
	Text = nil;
	Addons = nil;
	Settings = nil;
	sGeWithTheShovel = nil;
	Hotkeys = nil;
}

System.LoadCharacters = func(){
	var file = io.open( Shovel.AiFolder .. Shovel.FSP .. Shovel.CharsFolder .. Shovel.FSP .. Shovel.Region .. Shovel.FSP ..'characters_list.shovel', 'r');
	if (file) {
		var fileContent, charFile = file:read('*a');
		file:close();
		for (charFile in string.gfind(fileContent, '([a-zA-Z%s]+' .. Shovel.Ext .. ')' .. Shovel.WINEOL)) {
			System.LoadFile( Shovel.AiFolder .. Shovel.FSP .. Shovel.CharsFolder .. Shovel.FSP .. Shovel.Region .. Shovel.FSP .. charFile)
		}
	}
}

System.Modules ={
	[1] = 'settings',
	[2] = 'localization',
	[3] = 'interface',
	[4] = 'addons',
	[5] = 'brain',
	[6] = 'alarms',
	[7] = 'sgewiththeshovel',
	[8] = 'hotkeys'
}

System.Load();