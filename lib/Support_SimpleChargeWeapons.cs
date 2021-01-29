// %img.discharge = power per shot
// %img.maxCharge = max charge
// %img.chargeAmount = charge per tick
// %img.chargeTickTime = time per tick (millisecond)
// %pl.weaponCharge[%slot] = charge total for the weapon in that slot

package Support_SimpleChargeWeapons
{
	function GameConnection::spawnPlayer(%cl)
	{
		%ret = parent::spawnPlayer(%cl);

		if (!isEventPending($SimpleWeaponChargeSchedule))
		{
			SimpleWeaponChargeLoop();
		}

		return %ret;
	}

	function WeaponImage::checkAmmo(%this, %obj, %slot)
	{
		if (%obj.getClassName() $= "AIPlayer")
		{
			return;
		}

		if (%this.maxCharge <= 0)
		{
			return parent::checkAmmo(%this, %obj, %slot);
		}

		if (%obj.weaponCharge[%obj.currTool] <= 0)
		{
			%obj.weaponCharge[%obj.currTool] = 0;
			%obj.setImageAmmo(%slot, 0);
		}
		else if (%obj.weaponCharge[%obj.currTool] < %this.discharge)
		{
			%obj.setImageAmmo(%slot, 0);
		}
		else
		{
			%obj.setImageAmmo(%slot, 1);
		}

		if (%obj.weaponCharge[%obj.currTool] > %this.maxCharge)
		{
			%obj.weaponCharge[%obj.currTool] = %this.maxCharge;
		}

		SimpleCharge_BottomprintEnergyLevel(%obj);
	}
};
activatePackage(Support_SimpleChargeWeapons);

function SimpleWeaponChargeLoop()
{
	cancel($SimpleWeaponChargeSchedule);

	if (!isObject(MissionCleanup))
	{
		return;
	}

	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%pl = ClientGroup.getObject(%i).player;
		if (isObject(%pl))
		{
			%pl.chargeAllWeapons();
		}
	}

	$SimpleWeaponChargeSchedule = schedule(33, MissionCleanup, SimpleWeaponChargeLoop);
}

function Player::chargeAllWeapons(%pl)
{
	if (%pl.getState() $= "Dead")
	{
		return;
	}

	%db = %pl.getDatablock();
	for (%i = 0; %i < %db.maxTools; %i++)
	{
		%tool = %pl.tool[%i];
		%img = %tool.image;

		%maxCharge = %img.maxCharge;
		%chargeRate = %img.chargeRate;
		%chargeTickTime = %img.chargeTickTime;
		%currCharge = %pl.weaponCharge[%i];

		if (%maxCharge <= 0)
		{
			continue;
		}
		else if (%currCharge $= "")
		{
			%currCharge = %maxCharge - %chargeRate;
		}

		%equipped = 1;
		if (%pl.currTool != %i || %pl.getMountedImage(0) != %pl.tool[%i].image.getID())
		{
			%chargeRate = mCeil(%chargeRate / 5);
			%equipped = 0;
		}

		if (%currCharge < %maxCharge && %pl.nextChargeTime[%i] < getSimTime())
		{
			%currCharge = getMin(%currCharge + %chargeRate, %maxCharge);
			%pl.weaponCharge[%i] = %currCharge;
			%pl.nextChargeTime[%i] = (getSimTime() + %chargeTickTime) | 0;
			if (%equipped)
			{
				if (%pl.tickSound > 1)
				{
					%pl.playthread(0, shiftRight);
					serverPlay3D(Beep_Key_Sound, %pl.getPosition());
				}
				
				%pl.tickSound = (%pl.tickSound + 1) % 3;
			}
		}

		if (%maxCharge > 0 && %equipped)
		{
			SimpleCharge_BottomprintEnergyLevel(%pl);
		}
	}
}

function SimpleCharge_BottomprintEnergyLevel(%obj)
{
	if (!isObject(%obj.client) || %obj.getClassName() $= "AIPlayer" || %obj.currTool < 0 || %obj.client.doNotDisplay)
		return;
	%image = %obj.tool[%obj.currtool].image;
	if (!%image.discharge)
		return;
	%maxCharge = %image.maxCharge;
	%itemName = %image.getName();
	%currCharge = mFloor(%obj.weaponCharge[%obj.currTool]);
	
	while (strLen(%currCharge) < 3)
		%currCharge = " " @ %currCharge;	

	if (%currCharge < %maxCharge/10 || %currCharge < %obj.tool[%obj.currtool].image.discharge*2)
		%color = "\c0";
	else if (%currCharge < %maxCharge/2)
		%color = "\c3";
	else
		%color = "\c4";

	%gunInfo = "<font:Consolas:16>\c6ENERGY: " @ %currCharge @ " / " @ %maxCharge;
	%chargeInfo = "<font:Impact:22>" @ createPowerChargeBar(%maxcharge, %currCharge, 30, "I");

	%obj.client.bottomprint("<just:right>" @ %gunInfo @ " <br>" @ %chargeInfo @ " ", 1, 1);
}

function createPowerChargeBar(%maxcharge, %currCharge, %totalBars, %char)
{
	%numColoredBars = mCeil(%currCharge/%maxcharge*%totalBars);
	%char = %char $= "" ? "=" : %char;
	%bars = "\c7";
	%i = 0;

	for (%i = 0; %i < %totalBars - %numColoredBars; %i++)
	{
		%bars = %bars @ %char;
	}

	%threshold = mFloor(%totalBars / 2); //blue threshold
	if (%i < %threshold)
	{
		%bars = %bars @ "\c4";
		for (%j = %i; %j < %threshold; %j++)
		{
			%bars = %bars @ %char;
			%i += 1;
		}
	}

	%threshold = mFloor(%totalBars * 3 / 4); //yellow threshold
	if (%i < %threshold)
	{
		%bars = %bars @ "\c3";
		for (%j = %i; %j < %threshold; %j++)
		{
			%bars = %bars @ %char;
			%i += 1;
		}
	}

	%threshold = %totalBars; //red threshold
	if (%i < %threshold)
	{
		%bars = %bars @ "\c0";
		for (%j = %i; %j < %threshold; %j++)
		{
			%bars = %bars @ %char;
			%i += 1;
		}
	}
	return %bars;
}

datablock ShapeBaseImageData(SimpleChargeImageFramework_SemiAuto)
{
	// Basic Item properties
	shapeFile = "base/data/shapes/empty.dts";

	stateName[0]						= "Activate";
	stateTimeoutValue[0]				= 0.10;
	stateTransitionOnTimeout[0]			= "AmmoCheck";
	stateSound[0]						= weaponSwitchSound;

	stateName[1]						= "AmmoCheck";
	stateTimeoutValue[1]				= 0.15;
	stateScript[1]						= "checkAmmo";
	stateTransitionOnTimeout[1]			= "AmmoConfirm";

	stateName[2]						= "AmmoConfirm";
	stateTransitionOnAmmo[2]			= "Ready";
	stateTransitionOnNoAmmo[2]			= "AmmoCheck";

	stateName[3]						= "Ready";
	stateTransitionOnTriggerDown[3]		= "Fire";
	stateWaitForTimeout[3]				= false;
	stateSequence[3]					= "Ready";
	stateScript[3]						= "checkAmmo";
	stateTimeoutValue[3]				= 0.05;
	stateTransitionOnTimeout[3]			= "Ready2";

	stateName[4]						= "Fire";
	stateTransitionOnTimeout[4]			= "Smoke";
	stateTimeoutValue[4]				= 0.1;
	stateFire[4]						= true;
	stateAllowImageChange[4]			= false;
	stateSequence[4]					= "Fire";
	stateScript[4]						= "onFire";

	stateName[5]						= "Smoke";
	stateTimeoutValue[5]				= 0.1;
	stateWaitForTimeout[5]				= true;
	stateTransitionOnTimeout[5]			= "PostFire";

	stateName[6]						= "Ready2";
	stateTransitionOnTriggerDown[6]		= "Fire";
	stateWaitForTimeout[6]				= false;
	stateSequence[6]					= "Ready";
	stateScript[6]						= "checkAmmo";
	stateTimeoutValue[6]				= 0.05;
	stateTransitionOnTimeout[6]			= "Ready";

	stateName[7]						= "PostFire";
	stateTransitionOnTriggerUp[7]		= "AmmoCheck";
};

datablock ShapeBaseImageData(SimpleChargeImageFramework_Auto)
{
	// Basic Item properties
	shapeFile = "base/data/shapes/empty.dts";

	stateName[0]						= "Activate";
	stateTimeoutValue[0]				= 0.10;
	stateTransitionOnTimeout[0]			= "AmmoCheck";
	stateSound[0]						= weaponSwitchSound;

	stateName[1]						= "AmmoCheck";
	stateTimeoutValue[1]				= 0.15;
	stateScript[1]						= "checkAmmo";
	stateTransitionOnTimeout[1]			= "AmmoConfirm";

	stateName[2]						= "AmmoConfirm";
	stateTransitionOnAmmo[2]			= "Ready";
	stateTransitionOnNoAmmo[2]			= "AmmoCheck";

	stateName[3]						= "Ready";
	stateTransitionOnTriggerDown[3]		= "Fire";
	stateWaitForTimeout[3]				= false;
	stateSequence[3]					= "Ready";
	stateScript[3]						= "checkAmmo";
	stateTimeoutValue[3]				= 0.05;
	stateTransitionOnTimeout[3]			= "Ready2";

	stateName[4]						= "Fire";
	stateTransitionOnTimeout[4]			= "Smoke";
	stateTimeoutValue[4]				= 0.1;
	stateFire[4]						= true;
	stateAllowImageChange[4]			= false;
	stateSequence[4]					= "Fire";
	stateScript[4]						= "onFire";

	stateName[5]						= "Smoke";
	stateTimeoutValue[5]				= 0.1;
	stateWaitForTimeout[5]				= true;
	stateTransitionOnTimeout[5]			= "AmmoCheck";

	stateName[6]						= "Ready2";
	stateTransitionOnTriggerDown[6]		= "Fire";
	stateWaitForTimeout[6]				= false;
	stateSequence[6]					= "Ready";
	stateScript[6]						= "checkAmmo";
	stateTimeoutValue[6]				= 0.05;
	stateTransitionOnTimeout[6]			= "Ready";
};

function SimpleChargeImage::onFire(%this, %obj, %slot)
{
	if (%obj.getClassName() !$= "Player")
	{
		%obj.weaponnCharge[%obj.currTool] = %this.discharge;
	}

	if (%obj.weaponCharge[%obj.currTool] < %this.discharge || %obj.getDamagePercent() >= 1.0)
	{
		%obj.setImageAmmo(%slot, 0);
		SimpleCharge_BottomprintEnergyLevel(%obj);
		return;
	}

	%obj.weaponCharge[%obj.currTool] -= %this.discharge;
	%obj.nextChargeTime[%obj.currTool] = (getSimTime() + %this.chargeDisableTime) | 0;

	SimpleCharge_BottomprintEnergyLevel(%obj);
	%obj.spawnExplosion(BigRecoilProjectile, 0.3);

	/////////////////////////////////////////////////////////////////////////////////////////////////////

	%obj.playThread(2, plant);
	%obj.tickSound  = 3;

	%projectile = %this.projectile;
	%spread = %this.spread;
	%shellcount = getMax(1, %this.shellcount);

	if (%this.markerlightSupport && !%obj.markerlightDisabled && isFunction(getMarkerlightVector))
	{
		%searchProj = isObject(%this.markerlightProjectile) ? %this.markerlightProjectile : %this.projectile;
		%muzzleVector = getMarkerlightVector(%obj, %searchProj, %this.markerlightMaxRange, 
			%this.markerlightMaxAngle, %obj.getMuzzleVector(%slot), %obj.getMuzzlePoint(%slot));
		%foundTarget = getField(%muzzleVector, 1);
		%muzzleVector = getField(%muzzleVector, 0);

		if (isObject(%foundTarget))
		{
			if (%this.markerlightSpread > 0) %spread = %this.markerlightSpread;
			if (%this.markerlightProjectile > 0) %projectile = %this.markerlightProjectile;
			if (%this.markerlightShellCount > 0) %shellCount = %this.markerlightShellCount;
		}
	}
	else
	{
		%muzzleVector = %obj.getMuzzleVector(%slot);
	}

	for(%shell = 0; %shell < %shellcount; %shell++)
	{
		%vector = %muzzleVector;
		%objectVelocity = %obj.getVelocity();
		%vector1 = VectorScale(%vector, %projectile.muzzleVelocity);
		%vector2 = VectorScale(%objectVelocity, %projectile.velInheritFactor);
		%velocity = VectorAdd(%vector1,%vector2);
		%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
		%velocity = MatrixMulVector(%mat, %velocity);

		%p = new (%this.projectileType)()
		{
			dataBlock = %projectile;
			initialVelocity = %velocity;
			initialPosition = %obj.getMuzzlePoint(%slot);
			sourceObject = %obj;
			sourceSlot = %slot;
			client = %obj.client;
		};
		MissionCleanup.add(%p);
	}
	return %p;
}

function Player::setLaserChargePercent(%pl, %percent)
{
	if (%pl.getState() $= "Dead")
	{
		return;
	}

	%db = %pl.getDatablock();
	for (%i = 0; %i < %db.maxTools; %i++)
	{
		%tool = %pl.tool[%i];
		%img = %tool.image;

		%maxCharge = %img.maxCharge;
		%chargeTickTime = 1000;
		%pl.weaponCharge[%i] = mFloor(%maxCharge * %percent / 100);
		%pl.nextChargeTime[%i] = (getSimTime() + %chargeTickTime) | 0;

		%equipped = (%pl.currTool != %i || %pl.getMountedImage(0) != %pl.tool[%i].image.getID()) ? 0 : 1;
		
		if (%maxCharge > 0 && %equipped)
		{
			SimpleCharge_BottomprintEnergyLevel(%pl);
		}
	}
}

function Player::AddLaserCharge(%pl, %amount)
{
	if (%pl.getState() $= "Dead")
	{
		return;
	}

	%db = %pl.getDatablock();
	for (%i = 0; %i < %db.maxTools; %i++)
	{
		%tool = %pl.tool[%i];
		%img = %tool.image;

		%maxCharge = %img.maxCharge;
		%chargeTickTime = 1000;
		%pl.weaponCharge[%i] = getMax(getMin(%maxCharge, %pl.weaponCharge[%i] + %amount), 0);
		%pl.nextChargeTime[%i] = (getSimTime() + %chargeTickTime) | 0;

		%equipped = (%pl.currTool != %i || %pl.getMountedImage(0) != %pl.tool[%i].image.getID()) ? 0 : 1;
		
		if (%maxCharge > 0 && %equipped)
		{
			SimpleCharge_BottomprintEnergyLevel(%pl);
		}
	}
}

registerOutputEvent("Player", "setLaserChargePercent", "int 0 100 0");
registerOutputEvent("Player", "AddLaserCharge", "int -1000 1000 0");