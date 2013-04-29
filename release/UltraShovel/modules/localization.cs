if (Text){	Text.Unload();	} 

Text = {}

Text.Unload = func (){
	Text = nil;
}

Text.Load = func(){	
	Text.LoadLanguage(Settings["Language"]);
	if (Shovel.IsDebug) SysMsg("Localization loaded");
}

Text.LoadLanguage = func(lang){
	var f = io.open(Shovel.AiFolder .. Shovel.FSP .. Shovel.LanguagesFolder .. Shovel.FSP .. lang .. '.shovel.lang');
	if (!f) { Text.Unload(); }
	var s, k, v = f:read('*a');
	f:close();
	s = string.gsub(s, '#[^' .. string.char(10) .. ']*', '');
	for (k, v in string.gfind(s, '([%[%]%w]+)%s*=%s*([^' .. string.char(10) .. ']*)')) {
		Text[k] = v;		
	}			
}

Text.Load();