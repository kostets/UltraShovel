if (sGeWithTheShovel){	sGeWithTheShovel.Unload();	} 

sGeWithTheShovel = {
		Splitter = "\n",
		LogStringsCounter = 0,
		IsCapcha = nil,
		ShovelTried = 1,
		f = "system",
		ShovelLogMessage = ""
	}

sGeWithTheShovel.Unload = func (){
	sGeWithTheShovel = nil;
}

sGeWithTheShovel.Load = func(){
	if (Shovel.IsDebug) SysMsg("Shovel loaded");
}

sGeWithTheShovel.AutoInspect = func()
{
	if (IsVisible('autoinspect') == 'YES')
	{
		Close('autoinspect');
	}
	else Open('autoinspect');
}

sGeWithTheShovel.Monitoring = func(){
	if (!sGeWithTheShovel.IsCapcha) {
		if (IsVisible('autoinspect') == 'YES') {
			sGeWithTheShovel.IsCapcha = true;
			sGeWithTheShovel.ShovelLog(Text['ShovelStart']);
			Call();
			SelectAll();
			sleep(1000);
			ChangeTactics('TS_NONE');
		}
		return;
	}

	if (IsVisible('autoinspect') == 'NO') 
	{
		sGeWithTheShovel.IsCapcha = nil;
		sGeWithTheShovel.ShovelLog(Text['ShovelUnload']);
		sleep(100);
		sGeWithTheShovel.ShovelLog("Close");
		return Keep();
	}

	var answer = io.open(sGeWithTheShovel.f, 'r');
	if (!answer) return;
	var str = answer:read("*a");
	answer:close();
	if (string.len (str) < 5) return;

	var o = io.open(sGeWithTheShovel.f, 'w');
	if (o) {
		o:write('');
		o:flush();
		o:close();
	}
	
	SendAutoInspectAnswer(str);
	sGeWithTheShovel.ShovelLog(Text['ShovelSend'] .. str);
	sleep(2000);
	if (IsVisible('autoinspect') == 'YES') {
		var tried = sGeWithTheShovel.ShovelTried;
		if (tried >= 4) {
			sGeWithTheShovel.ShovelLog('Shovel™: Failed.');
			return;
		} else {
			sGeWithTheShovel.ShovelLog(Text['ShovelIncorrect']);
			sleep(10000);
			SendAutoInspectPageRefresh();
			sGeWithTheShovel.ShovelTried = tried + 1;
			sleep(1000);
		}
	}
	return;
}

sGeWithTheShovel.ShovelLog = func(message){
    if (message == "Close")
	{
		sGeWithTheShovel.LogStringsCounter = 0;
		sGeWithTheShovel.ShovelLogMessage = "";
		Close('tutomessage');
		return;
	}
	
    sGeWithTheShovel.LogStringsCounter = sGeWithTheShovel.LogStringsCounter + 1;
	sGeWithTheShovel.ShovelLogMessage = sGeWithTheShovel.ShovelLogMessage .. os.date('%X') .. ' | ' .. message .. sGeWithTheShovel.Splitter;
	
	if (sGeWithTheShovel.LogStringsCounter > 8)
	{
		var index = string.find (sGeWithTheShovel.ShovelLogMessage, splitter)
		sGeWithTheShovel.ShovelLogMessage = string.sub(sGeWithTheShovel.ShovelLogMessage, index + 2);
	}
	
	SetPropertyString('Tip', 'BOSS_ALQA_1', 'HelpName', 'AI');
	SetPropertyString('Tip', 'BOSS_ALQA_1', 'Notice', Text['ShovelLogName']);
	SetPropertyString('Tip', 'BOSS_ALQA_1', 'Title', Text['ShovelLogTitle']);
	SetPropertyString('Tip', 'BOSS_ALQA_1', 'Desc', sGeWithTheShovel.ShovelLogMessage);
	HelpSelect('AI');
	return;
} 

sGeWithTheShovel.Load();