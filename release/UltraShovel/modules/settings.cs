if (Settings){	Settings.Unload();	} 

Settings = {}

Settings.Unload = func (){
	Settings = nil;
}

Settings.Load = func(){
	var f = io.open(Shovel.AiFolder .. '\settings.shovel.ini', 'r');
	if (!f) { Settings.Unload(); }
	var s, k, v = f:read('*a');
	f:close();
	s = string.gsub(s, '#[^' .. string.char(10) .. ']*', '');
	for (k, v in string.gfind(s, '([%[%]%w]+)%s*=%s*([^' .. string.char(10) .. ']*)')) {
		if (tostring(v) == 'true')
		{
			Settings[k] = 'true';
		}
		else if (tostring(v) == 'false')
		{
			Settings[k] = 'false';
		}
		else 
		{
			Settings[k] = v;
		}
	}			
	if (Shovel.IsDebug) SysMsg("Settings loaded");	
}

Settings.ChangeLanguage = func(lang){
	var name = Shovel.AiFolder .. '\settings.shovel.ini';
	var f = io.open(name, 'r');
	var s = f:read('*a');
	f:close();
	var k = '';
	if (tostring(lang) == 'rus')
	{
		k = string.gsub(s, "Language = eng", "Language = rus");
	}
	else
	{
		k = string.gsub(s, "Language = rus", "Language = eng");
	}
	
	var o = io.open(name, 'w');
	if (o) {
		o:write('');
		o:write(k);
		o:flush();
		o:close();
	}
}

Settings.SaveFamily = func(){
	var name = Shovel.AiFolder .. Shovel.FSP .. Shovel.SettingsFolder .. Shovel.FSP .. 'family_' ..Shovel.Commander;
	
	var o = io.open(name, 'w');
	if (o) {
		o:write('');
		o:flush();
		o:close();
	}
	
	var file = io.open(name, 'w');
	if (file) 
	{
		for (key, value in Settings)
		{
			if (string.find(tostring(value) , 'function') == nil)
			{
				file:write(key .. ' = ' .. tostring(value));
				file:write(Shovel.WINEOL);
			}
		}
		file:flush();
		file:close();
	}	
}

Settings.LoadFamily = func(){
	var name = Shovel.AiFolder .. Shovel.FSP .. Shovel.SettingsFolder .. Shovel.FSP .. 'family_' ..Shovel.Commander;
	var f = io.open(name, 'r');
	if (!f) { 
		SysMsg("Cant find settings for " .. Shovel.Commander);
		return;
	}
	var s, k, v = f:read('*a');
	f:close();
	s = string.gsub(s, '#[^' .. string.char(10) .. ']*', '');
	for (k, v in string.gfind(s, '([%[%]%w]+)%s*=%s*([^' .. string.char(10) .. ']*)')) {
		if (tostring(v) == 'true')
		{
			Settings[k] = 'true';
		}
		else if (tostring(v) == 'false')
		{
			Settings[k] = 'false';
		}
		else if (tonumber(v) == nil)
		{
			Settings[k] = tostring(v);
		}
		else 
		{
			Settings[k] = tonumber(v);
		}
	}				
	
	SysMsg(Text['SettingsLoadedForFamily'] .. Shovel.Commander);
}

Settings.SaveActor = func(AiActor){	

	if (AiActor.Stances)
	{
		var name = Shovel.AiFolder .. Shovel.FSP .. Shovel.SettingsFolder .. Shovel.FSP .. 'char_skills_' .. AiActor.Name;
		
		var o = io.open(name, 'w');
		if (o) {
			o:write('');
			o:flush();
			o:close();
		}
		
		var file = io.open(name, 'w');
		if (file) 
		{
			for (key, value in AiActor.Stances)
			{
				for (keyStance, valueStance in value)
				{
					if (string.find(tostring(valueStance) , 'table') != nil)
					{
						for (keySkill, valueSkill in valueStance)
						{
							file:write(tostring(valueSkill.InUse) .. ' = ' .. tostring(valueSkill.Timing) .. Shovel.WINEOL);
						}
					}
				}
			}
			file:flush();
			file:close();
		}
	}
	
	var name = Shovel.AiFolder .. Shovel.FSP .. Shovel.SettingsFolder .. Shovel.FSP .. 'char_settings_' .. AiActor.Name;
	
	var o = io.open(name, 'w');
	if (o) {
		o:write('');
		o:flush();
		o:close();
	}
	
	var file = io.open(name, 'w');
	if (file) 
	{
		file:write('UseBullets' .. ' = ' .. tostring(AiActor.UseBullets) .. Shovel.WINEOL);
		file:write('BulletsID' .. ' = ' .. tostring(AiActor.BulletsID) .. Shovel.WINEOL);
		file:write('NumPadButton' .. ' = ' .. tostring(AiActor.NumPadButton) .. Shovel.WINEOL);
		file:write('AutoAttack' .. ' = ' .. tostring(AiActor.AutoAttack) .. Shovel.WINEOL);
		file:write('HPlevel' .. ' = ' .. tostring(AiActor.HPlevel) .. Shovel.WINEOL);
		file:write('MPlevel' .. ' = ' .. tostring(AiActor.MPlevel) .. Shovel.WINEOL);
		file:write('WeaponIndex' .. ' = ' .. tostring(Interface.weaponIndexes[AiActor.Index + 1]) .. Shovel.WINEOL);
		file:write('StanceIndex' .. ' = ' .. tostring(Interface.stancesIndexes[AiActor.Index + 1]) .. Shovel.WINEOL);
		
		file:flush();
		file:close();
	}
}

Settings.ParseActor = func(AiActor){

    if (AiActor.Stances){
		var name = Shovel.AiFolder .. Shovel.FSP .. Shovel.SettingsFolder .. Shovel.FSP .. 'char_skills_' .. AiActor.Name;
		var skill = 1;
		var stance = 1;
		
		var f = io.open(name, 'r');
		if (!f) { return; }
		var s, k, v = f:read('*a');
		f:close();
		s = string.gsub(s, '#[^' .. string.char(10) .. ']*', '');
		for (k, v in string.gfind(s, '([%[%]%w]+)%s*=%s*([^' .. string.char(10) .. ']*)')) 
		{		
			if (tostring(k) == 'true')
			{
				AiActor.Stances[stance].Skills[skill].InUse = 'true';
			}
			else if (tostring(k) == 'false')
			{
				AiActor.Stances[stance].Skills[skill].InUse = 'false';
			}
			
			AiActor.Stances[stance].Skills[skill].Timing = tonumber(v);
			
			skill = skill + 1;
			if (skill > 6)
			{
				skill = 1;
				stance = stance + 1;
			}
		}			
	}
	
	var name = Shovel.AiFolder .. Shovel.FSP .. Shovel.SettingsFolder .. Shovel.FSP .. 'char_settings_' .. AiActor.Name;
	var f = io.open(name, 'r');
	if (!f) { return; }
	var s, k, v = f:read('*a');
	f:close();
	s = string.gsub(s, '#[^' .. string.char(10) .. ']*', '');
	for (k, v in string.gfind(s, '([%[%]%w]+)%s*=%s*([^' .. string.char(10) .. ']*)')) 
	{		
		if (tostring(k) == 'UseBullets')
		{
			if (tostring(v) == 'true')
			{
				AiActor.UseBullets = true;
			}
			else
			{
				AiActor.UseBullets = false;
			}			
		}
		else if (tostring(k) == 'BulletsID')
		{
			AiActor.BulletsID = tonumber(v);
		}
		else if (tostring(k) == 'NumPadButton')
		{
			AiActor.NumPadButton = tonumber(v);
		}
		else if (tostring(k) == 'HPlevel')
		{
			AiActor.HPlevel = tonumber(v);
		}
		else if (tostring(k) == 'MPlevel')
		{
			AiActor.MPlevel = tonumber(v);
		}
		else if (tostring(k) == 'AutoAttack')
		{
			if (tostring(v) == 'true')
			{
				AiActor.AutoAttack = 'true';
			}
			else
			{
				AiActor.AutoAttack = 'false';
			}			
		}		
		else if (tostring(k) == 'WeaponIndex')
		{
			Interface.weaponIndexes[AiActor.Index + 1] = tonumber(v);
		}		
		else if (tostring(k) == 'StanceIndex')
		{
			Interface.stancesIndexes[AiActor.Index + 1] = tonumber(v);
		}
	}			
	
	SysMsg(Text['SettingsLoaded'] .. AiActor.Name);
}

Settings.GlobalLog = func(Actor){

	var name = Shovel.AiFolder .. Shovel.FSP .. 'logs' .. Shovel.FSP .. os.date('%Y%m%d.') .. Actor.Name .. '_global_log.txt';
	var file = io.open(name, 'w');
	if (file) 
	{		
		for (key, value in Actor)
		{
			file:write(tostring(key) .. ' = ' .. tostring(value) .. Shovel.WINEOL);
			var s = "";
			Settings.PresentValue(value, file, s);
		}
		file:write(Shovel.WINEOL);

		
		file:flush();
		file:close();
	}
}

Settings.PresentValue = func(val, f, s)
{
    s = s .. Shovel.TAB;
	if (string.sub(tostring(val), 1, 5) == 'table')
	{
		for (k, v in val)
		{
			f:write(s .. tostring(k) .. ' = ' .. tostring(v) .. Shovel.WINEOL);
			Settings.PresentValue(v,f,s);
		}
	}
}

Settings.Load();
