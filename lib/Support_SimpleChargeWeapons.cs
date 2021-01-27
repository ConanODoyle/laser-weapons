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

		bottomPrintEnergyLevel(%obj);
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

	$SimpleWeaponChargeSchedule = %pl.schedule(33, chargeAllWeapons);
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

		if (%currCharge < %maxCharge && %pl.nextChargeTime[%i] < getSimTime())
		{
			// talk("Charging " @ %img.getName() @ ": " @ %chargeRate SPC getSimTime());
			%currCharge = getMin(%currCharge + %chargeRate, %maxCharge);
			%pl.weaponCharge[%i] = %currCharge;
			%pl.nextChargeTime[%i] = (getSimTime() + %chargeTickTime) | 0;
			if (%pl.currTool == %i)
			{
				if (%pl.tickSound > 1)
				{
					%pl.playthread(0, shiftRight);
					serverPlay3D(brickMoveSound, %pl.getPosition());
				}
				
				%pl.tickSound = (%pl.tickSound + 1) % 3;
			}
		}

		if (%maxCharge > 0 && %pl.currTool == %i)
		{
			bottomPrintEnergyLevel(%pl);
		}
	}
}

function bottomPrintEnergyLevel(%obj)
{
	if (!isObject(%obj.client) || %obj.getClassName() $= "AIPlayer" || %obj.currTool < 0 || %obj.client.doNotDisplay)
		return;
	%image = %obj.tool[%obj.currtool].image;
	if (!%image.discharge)
		return;
	%maxCharge = %image.maxCharge;
	%itemName = %image.getName();
	%currCharge = mFloor(%obj.weaponCharge[%obj.currTool]);
	if (%obj.storedCharge $= "")
		%obj.storedCharge = $Pref::EnergyAmmo::maxStoredEnergy;
	%storedCharge = mFloor(%obj.storedCharge);

	if (%storedCharge < 100)
		%storedCharge = %storedCharge @ "  ";
	if (%currCharge < 100)
		%currCharge = %currCharge @ "  ";
	

	if (%currCharge < %maxCharge/10 || %currCharge < %obj.tool[%obj.currtool].image.discharge*2)
		%color = "\c0";
	else if (%currCharge < %maxCharge/2)
		%color = "\c3";
	else
		%color = "\c4";

	//%gunInfo = "<font:Tahoma:22>\c3" @ %image.discharge @ "<font:Tahoma:15>\c6 ENERGY/shot"; 
	if (%obj.weaponCharge[%obj.currTool] $= "")
		%chargeInfo = "<font:Tahoma:15>\c3ENERGY\c6: CHECKING" SPC "<font:Arial:24>" @ createBarCharge(%maxcharge, %currCharge);
	else
		%chargeInfo = "<font:Tahoma:15>\c6" @ %currCharge SPC "<font:Arial:28>" @ createBarCharge(%maxcharge, %currCharge);
	//%storedInfo = "<font:Tahoma:15>\c3STORED\c6: " @ %storedCharge @ "/" @ $Pref::EnergyAmmo::maxStoredEnergy;// SPC "<font:Arial:20>" @ createBarCharge($Pref::EnergyAmmo::maxStoredEnergy, %storedCharge);

	%obj.client.bottomprint("<just:right>" @ %gunInfo @ "<font:Tahoma:15> <br>" @ %chargeInfo @ "<font:Tahoma:15> <br>" @ %storedInfo @ "<font:Tahoma:15> ", 100, 1);
}

function createBarCharge(%maxcharge, %currCharge)
{
	%numColoredBars = mFloor(%currCharge/%maxcharge*20);
	%bars = "\c7";
	%i = 0;
	for (%i = 0; %i < 20 - %numColoredBars; %i++)
		%bars = %bars @ "|";
	if (%i < 10)
	{
		%bars = %bars @ "\c4";
		for (%j = %i; %j < 10; %j++)
		{
			%bars = %bars @ "|";
			%i += 1;
		}
	}
	if (%i < 15)
	{
		%bars = %bars @ "\c3";
		for (%j = %i; %j < 15; %j++)
		{
			%bars = %bars @ "|";
			%i += 1;
		}
	}
	if (%i < 20)
	{
		%bars = %bars @ "\c0";
		for (%j = %i; %j < 20; %j++)
		{
			%bars = %bars @ "|";
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

function SimpleChargeImage::onFire(%this,%obj,%slot)
{
	if (%obj.weaponCharge[%obj.currTool] < %this.discharge)
	{
		%obj.setImageAmmo(%slot, 0);
		bottomPrintEnergyLevel(%obj);
		return;
	}

	%obj.weaponCharge[%obj.currTool] -= %this.discharge;
	%obj.nextChargeTime[%obj.currTool] = (getSimTime() + %this.chargeDisableTime) | 0;

	bottomPrintEnergyLevel(%obj);
	%obj.spawnExplosion(BigRecoilProjectile, 0.3);

	/////////////////////////////////////////////////////////////////////////////////////////////////////

	%obj.setVelocity(VectorAdd(%obj.getVelocity(),VectorScale(%obj.client.player.getEyeVector(),%this.recoilVelocity)));

	if(%obj.getDamagePercent() < 1.0)
	{
		%obj.playThread(2, plant);
	}
	%projectile = %this.projectile;
	%spread = %this.spread;
	%shellcount = getMax(1, %this.shellcount);

	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%vector = %obj.getMuzzleVector(%slot);
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