if (Brain){	Brain.Unload();	} 

Brain = {
	AmmoTime = 0,
	ReturnTime = 0,
	ItemsTime = { 
				[1] = 0,
				[2] = 0,
				[3] = 0,
				[4] = 0,
				[5] = 0,
				[6] = 0,
				[7] = 0				
				}
}

Brain.Unload = func (){
	Brain = nil;
}

Brain.Load = func(){	
	if (Shovel.IsDebug) SysMsg("Brain loaded");
}

Brain.KeepPosition = func(Actor){
	if (tonumber(Settings['Mode']) == 0){
		if (!Shovel.IsTime(Brain.ReturnTime, tonumber(Settings['ReturnTime']))) return;
		Brain.ReturnTime = Shovel.Now;
		
		for (i = 1, Brain.GetCharsCount())
		{
			if(IsNearFromKeepDestPosition(Shovel.Actors[i].SelfAi, 100) != 'YES')
			{
				if (Shovel.IsDebug) SysMsg('Return ' .. Shovel.Actors[i].Name);
				KeepDestMoveTo(Shovel.Actors[i].SelfAi);				
			}
		}
		
		sleep(1000);	
	
	}
	else{
		sleep(3000);
		if (Shovel.IsTime(Brain.ReturnTime, tonumber(Settings['CallTime'])))
		{	
			Brain.ReturnTime = Shovel.Now;
			Call();
		}			
	}
}

Brain.AutoAttack = func(Actor){
	sleep(100);
	if (Actor.AutoAttack == 'false') { return; }
	if (IsMoving(Actor.SelfAi) == 'YES') { return; }
	var curTarget = GetNearAtkableEnemy(Actor.SelfAi, tonumber(Settings["KeepRange"]));
	if (!curTarget) return;
	SetAiTarget(Actor.SelfAi, curTarget);
	if (IsAbleToAttack(Actor.SelfAi) == 'YES')
	{
		Attack(Actor.SelfAi, curTarget);
	}
}

Brain.AutoPick = func(Actor){
	sleep(100);
	if (IsMoving(Actor.SelfAi) == 'YES') { return false; }
	if (tostring(Settings['AutoPickChar' .. (Actor.Index + 1)]) == 'true')
	{
		var nearItem = GetNearItem(Actor.SelfAi, tonumber(Settings["PickRange"]));
		if (nearItem != 0) 
		{
			PickItem(Actor.SelfAi, nearItem);
			return true;
		}
		else return false;
	}
}

Brain.KnockdownProtect = func (Actor){

}

Brain.AutoSkill = func(Actor){
	sleep(100);
		
	if (!Actor.Stances) return;
	for (stanceIndex, stance in pairs(Actor.Stances))
	{
		for (skillIndex, skill in pairs(stance.Skills))
		{			
			if (Brain.UseSkill(Actor, skill, skillIndex-1, stanceIndex) == true){
				sleep(200);				
			}			
		}
	}
}

Brain.AutoItems = func(Actor){

	for (i = 1,7)
	{
		if (tostring(Settings['ItemUse' .. i]) == 'true')
		{
			if (Shovel.IsTime(Brain.ItemsTime[i], tonumber(Settings['Item' .. i .. 'Time'])))
			{
				Brain.ItemsTime[i] = Shovel.Now;
				UseCommonItem(tonumber(Settings['Item' .. i]));
			}
		}
	}
	
	if (Actor.UseBullets == false) return;
	if (!Shovel.IsTime(Brain.AmmoTime, tonumber(Settings['AmmoUsageDelay']))) return;
	Brain.AmmoTime = Shovel.Now;
	
	var numberOfBullets = GetItemCountByType(Actor.BulletsID);
	if (numberOfBullets < 5000)
	{
		UseCommonItem(tonumber(Actor.NumPadButton));
	}
}

Brain.UseSkill = func(MyActor, skill, skillIndex, stanceIndex){	
	var AiBrain = GetAiActor(MyActor.Self);
	if (tostring(skill.InUse) == 'false') return false;
	if (!Shovel.IsTime(skill.UsageTime, skill.Timing)) return false;	
	if (skill.ID == 0) return false;	
	//if (Shovel.IsDebug) SysMsg('[' .. MyActor.Name .. '] Skill: ' .. skill.Name);
	
	var target = tostring(skill.Target);
	if (target == 'Ground' || target == 'None')
	{	
		Brain.ChangeStanceIfIncorrect(MyActor, stanceIndex);
		Brain.UseNoTargetSkill(MyActor, skill.ID, skillIndex);
	}
	else if (target == 'Enemy' || target == 'Wave')
	{	
		var curTarget = GetNearAtkableEnemy(AiBrain, tonumber(Settings["KeepRange"]));
		if (curTarget)
		{
			Brain.ChangeStanceIfIncorrect(MyActor, stanceIndex);
			UseSkill(AiBrain, curTarget, skill.ID);
		}
	}
	else if (target == 'Party')
	{
		var curTarget = GetNeedHealFriend(AiBrain, tonumber(Settings["KeepRange"]), 95)
		if (curTarget)
		{
			Brain.ChangeStanceIfIncorrect(MyActor, stanceIndex);
			UseSkill(AiBrain, curTarget, skill.ID);
		}
	}
	else if (target == 'Corpse')
	{
		var curTarget = Brain.GetDeadTeamMember();
		if (curTarget)
		{
			Brain.ChangeStanceIfIncorrect(MyActor, stanceIndex);
			UseSkill(AiBrain, curTarget, skill.ID);
		}
	}
	else if (target == 'PartyBuff')
	{	
		SetSquad(4);
		InstantHold(AiBrain);
		if (tostring(Settings['CallAtPartyBuff']) == 'true')
		{				
			Brain.ChangeStanceIfIncorrectWithCall(MyActor, stanceIndex);
			Brain.UseNoTargetSkill(MyActor, skill.ID, skillIndex);			
		}
		else
		{		
			Brain.ChangeStanceIfIncorrect(MyActor, stanceIndex);		
			Brain.UseNoTargetSkill(MyActor, skill.ID, skillIndex);
		}
	}
	
	skill.UsageTime = Shovel.Now; 
	while(IsSkillUsing(AiBrain) == 'YES') { sleep(200); }
	return true;
}

Brain.UseNoTargetSkill = func (Actor, id, index) {
	if (UseSkillNone) UseSkillNone(Actor.SelfAi, tonumber(id));
	else 
	Skill(tonumber(Actor.Index), tonumber(index));
}

Brain.AutoPotions = func(){
	if (!Shovel.IsReady) return;
	
	for(i = 1, Brain.GetCharsCount())
	{
		var uiFrame = GetFrameByName('charbaseinfo', i - 1);
		if (!uiFrame) return;
		Shovel.Actors[i].HP = tonumber(GetTextByKey(uiFrame, 'hp'));
		Shovel.Actors[i].MP = tonumber(GetTextByKey(uiFrame, 'sp'));

		if(tostring(Settings["UseHPChar" .. i]) == 'true')
		{
			var onePercent = Shovel.Actors[i].MaxHP / 100;
			var currentPercent = Shovel.Actors[i].HP / onePercent;
			if (currentPercent < Shovel.Actors[i].HPlevel)
			{
				UseItem(Shovel.Actors[i].Index, 0);
			}
		}
		if(tostring(Settings["UseMPChar" .. i]) == 'true')
		{
			var onePercent = Shovel.Actors[i].MaxMP / 100;
			var currentPercent = Shovel.Actors[i].MP / onePercent;
			if (currentPercent < Shovel.Actors[i].MPlevel)
			{
				UseItem(Shovel.Actors[i].Index, 1);
			}
		}
	}
}

Brain.UserTarget = func(Actor){
	var selfAi = Actor.SelfAi;
	var userTarget = GetUserTarget(selfAi);
	if(userTarget != nil){
		SetAiTarget(selfAi, userTarget);
		ClearUserTarget(selfAi);
		if (IsAbleToAttack(selfAi) == 'YES')
		{
			Attack(selfAi, userTarget);
		}
	}
}

Brain.GetAiIndex = func(self){
	for (i = 1, Brain.GetCharsCount())
	{ if (self == Shovel.Actors[i].Self) return i; }
	return -1;
}

Brain.HandleActor = func(self){	
	if (Shovel.CharsCounter > Brain.GetCharsCount())
	{		
		if  (Brain.GetAiIndex(self) == -1)
		{
			Shovel.ReInit();
			return;
		}
		else return;
	}
	if (Shovel.IsDebug) SysMsg("Loading actor " .. Shovel.CharsCounter);
	Shovel.Actors[Shovel.CharsCounter].Self = self;
	Shovel.Actors[Shovel.CharsCounter].SelfAi = GetAiActor(self);
	Shovel.Actors[Shovel.CharsCounter].Index = Shovel.CharsCounter - 1;
	var frame = GetFrameByName('status', Shovel.CharsCounter - 1);
	if (frame)
	{				
		Shovel.Actors[Shovel.CharsCounter].Name = GetTextByKey(frame, 'charactername');
		Shovel.Actors[Shovel.CharsCounter].Level = GetTextByKey(frame, 'characterlevel');
		Shovel.Actors[Shovel.CharsCounter].Job = GetTextByKey(frame, 'characterjob');
	}
	frame = GetFrameByName('charbaseinfo', Shovel.CharsCounter - 1);
	if (frame) 
	{
		Shovel.Actors[Shovel.CharsCounter].HP = tonumber(GetTextByKey(frame, 'hp'));
		Shovel.Actors[Shovel.CharsCounter].MaxHP = tonumber(GetTextByKey(frame, 'max_hp'));
		Shovel.Actors[Shovel.CharsCounter].MP = tonumber(GetTextByKey(frame, 'sp'));
		Shovel.Actors[Shovel.CharsCounter].MaxMP = tonumber(GetTextByKey(frame, 'max_sp'));
	}
	
	Shovel.Actors[Shovel.CharsCounter].Stances = Brain.GetStancesByJob(Shovel.Actors[Shovel.CharsCounter].Job);
	Shovel.Actors[Shovel.CharsCounter].Weapons = Brain.GetWeaponsByJob(Shovel.Actors[Shovel.CharsCounter].Job);

	Shovel.Actors[Shovel.CharsCounter].HPlevel = 50;
	Shovel.Actors[Shovel.CharsCounter].MPlevel = 50;
	Shovel.Actors[Shovel.CharsCounter].UseBullets = 'false';
	Shovel.Actors[Shovel.CharsCounter].BulletsID = 21116;
	Shovel.Actors[Shovel.CharsCounter].NumPadButton = 0;
	Shovel.Actors[Shovel.CharsCounter].AutoAttack = 'true';	

	Settings.ParseActor(Shovel.Actors[Shovel.CharsCounter]);
	SysMsg(Text['Slot'] .. Shovel.CharsCounter .. " : " .. Shovel.Actors[Shovel.CharsCounter].Job .. ' [' .. Shovel.Actors[Shovel.CharsCounter].Name .. 
	']');
	if (Shovel.CharsCounter == Brain.GetCharsCount()) {  if(Shovel.IsDebug) SysMsg("- Ready to farm -"); Shovel.IsReady = true; }
	Shovel.CharsCounter = Shovel.CharsCounter + 1;
}

Brain.IsNewActor = func(self){
	for (i = 0, Brain.GetCharsCount() - 1)
	{		
		if (Shovel.Actors[i + 1] == nil)
		{
			return true;
		}
	}
	return false;	
}

Brain.GetActorBySelf = func (self){
	for (i = 0, Brain.GetCharsCount() - 1)
	{
		if (Shovel.Actors[i + 1] == nil) { return nil; }
		if (Shovel.Actors[i + 1].Self == self) { return Shovel.Actors[i + 1]; }	
	}
}

Brain.GetCharsCount = func(){
	var count = 0;
	for (i = 0, 2) 
		{
			if (IsExistMyPc(i) == 'YES')
			{
				count = count + 1;
			}
		}	
	return count;
}

Brain.GetStancesByJob = func(job){
	for (key, value in pairs(Shovel.Chars)) 
	{					
		if (value.Job == job)
		{
			var Stances = {};
			var numOfStances = table.getn(value.Stances);
			for (i = 1, numOfStances)
			{
				Stances[i] = 
				{
					Name = value.Stances[i].Name,
					Skills = {}
				}
			    for (j = 1, 6)
				{					
					Stances[i].Skills[j] = 
					{
						Name 	 = value.Stances[i].Skills[j].Name,
						ID 		 = value.Stances[i].Skills[j].ID,
						InUse 	 = false,
						CastingTime = value.Stances[i].Skills[j].CastingTime,
						Duration = value.Stances[i].Skills[j].Duration,
						Cooldown = value.Stances[i].Skills[j].Cooldown,
						Timing 	 = value.Stances[i].Skills[j].Timing,
						Target   = value.Stances[i].Skills[j].Target,
						UsageTime = -1000
					}
				}
			}
			return Stances;
		}
	}
	SysMsg(Text['StancesNotFound'] .. job);
}

Brain.GetWeaponsByJob = func(job){
	for (key, value in pairs(Shovel.Chars)) 
	{	
		if (value.Job == job)
		{		
			var Weapons = {};
			var numOfWeapons = table.getn(value.Weapons);
		
			for (i = 1, numOfWeapons)
			{
				Weapons[i] = 
				{
					Name = value.Weapons[i].Name,
					Stances = {}
				}
				var numOfStances = table.getn(value.Weapons[i].Stances);
				for (j = 1, numOfStances)
				{
					Weapons[i].Stances[j] = value.Weapons[i].Stances[j];
				}
			}			
			return Weapons;
		}
	}
	SysMsg(Text['WeaponsNotFound'] .. job);
}

Brain.GetResAbleTarget = func (i) {
	if (IsExistMyPc(i) == 'YES') {
		var Actor = Shovel.Actors[i+1];			
		var success, friend = pcall(GetSelfActor, Actor.SelfAi);
		if (success) {
			success, result = pcall(IsDead, friend);
			if (success) {
				if (result == 'YES')
					return friend;
			}
		}			
	}
}
	
Brain.GetDeadTeamMember = func () {
	return Brain.GetResAbleTarget(0) || Brain.GetResAbleTarget(1) || Brain.GetResAbleTarget(2);
}

Brain.ChangeStanceIfIncorrect = func(MyActor, stanceIndex) {
	var uiFrame = GetFrameByName('charbaseinfo', MyActor.Index);
	var uiControl = GetControl(uiFrame, 'STANCETAB');
	var ctlLevel = GetNumber(uiControl);
	var targetStance = Brain.GetTargetStance(MyActor, stanceIndex);

	if (targetStance == nil) 
	{
		SysMsg(Text['IncorrectWeapon'] .. MyActor.Name);
		return;
	}
	else
	{
		targetStance = targetStance - 1;
	}
	if (tonumber(ctlLevel) != tonumber(targetStance))
	{
		if (Shovel.IsDebug) SysMsg('[' .. MyActor.Name .. '] Changing stance from ' .. tostring(ctlLevel) .. ' to ' .. tostring(targetStance));
		var LeaderIndex = GetLeaderIndex();
		if (MyActor.Index != LeaderIndex) SelectMyPc(MyActor.Index);
		sleep(100);
		ChangeStance(tonumber(targetStance));
		if (MyActor.Index != LeaderIndex) SelectMyPc(LeaderIndex);
		sleep(2000);
	} 
}

Brain.ChangeStanceIfIncorrectWithCall = func(MyActor, stanceIndex) {
 	var uiFrame = GetFrameByName('charbaseinfo', MyActor.Index);
	var uiControl = GetControl(uiFrame, 'STANCETAB');
	var ctlLevel = GetNumber(uiControl);
	var targetStance = Brain.GetTargetStance(MyActor, stanceIndex);
	if (targetStance == nil) 
	{
		SysMsg(Text['IncorrectWeapon'] .. MyActor.Name);
		return;
	}
	else
	{targetStance = targetStance - 1;}
	if (tonumber(ctlLevel) != tonumber(targetStance))
	{
		
		if (Shovel.IsDebug) SysMsg('[' .. MyActor.Name .. '] Changing stance from ' .. tostring(ctlLevel) .. ' to ' .. tostring(targetStance));
		
		var LeaderIndex = GetLeaderIndex();
		if (MyActor.Index != LeaderIndex) SelectMyPc(MyActor.Index);
		Call();
		sleep(100);
		ChangeStance(tonumber(targetStance));
		if (MyActor.Index != LeaderIndex) SelectMyPc(LeaderIndex);
	} 
	else {
		var LeaderIndex = GetLeaderIndex();		
		if (MyActor.Index != LeaderIndex) SelectMyPc(MyActor.Index);
		Call();
		sleep(1000);
		if (MyActor.Index != LeaderIndex) SelectMyPc(tonumber(LeaderIndex));
		sleep(1000);
	}
}

Brain.GetTargetStance = func (MyActor, stanceIndex)
{
	var currentWeaponIndex = Interface.weaponIndexes[MyActor.Index + 1];
	
	for (key,value in pairs(MyActor.Weapons[currentWeaponIndex].Stances))
	{
		if (tonumber(value) == tonumber(stanceIndex))
		{
			return tonumber(key);
		}
	}
}

Brain.Load();