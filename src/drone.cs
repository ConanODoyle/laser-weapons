
if (!isObject($LaserDroneSimSet))
{
	$LaserDroneSimSet = new SimSet();
}

datablock StaticShapeData(droneBotMount)
{
	shapeFile = "base/data/shapes/empty.dts";
};

datablock PlayerData(droneBotArmor : PlayerStandardArmor)
{
	shapeFile = "./resources/drone.dts";
	uiName = "";

	boundingBox = vectorScale("1 1 1.2", 4);
	crouchBoundingBox = vectorScale("1 1 1.2", 4);

	disableEmotes = 1;
	disableBurn = 1;

	maxDamage = 250;
	useCustomPainEffects = 1;
	painHighImage = "";
	painMidImage = "";
	painLowImage = "";
};

datablock DebrisData(droneBotDebris)
{
	emitters = "jeepDebrisTrailEmitter";

	shapeFile = "./droneDebris.dts";
	lifetime = 3.0;
	minSpinSpeed = -300.0;
	maxSpinSpeed = 300.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 1;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 2;
};

datablock ShapeBaseImageData(droneJetImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;

	mountPoint = 3;

	stateName[0]					= "Emitter1";
	stateEmitter[0]					= "droneJetEmitter";
	stateEmitterTime[0] 			= 1.2;
	stateTimeoutValue[0]			= 1;
	stateTransitionOnTimeout[0]		= "Emitter2";

	stateName[1]					= "Emitter2";
	stateEmitter[1]					= "droneJetEmitter";
	stateEmitterTime[1] 			= 1.2;
	stateTimeoutValue[1]			= 1;
	stateTransitionOnTimeout[1]		= "Emitter1";
};

package ChargeLaserDrones
{
	function AIPlayer::setNodeColor(%obj, %node, %color)
	{
		if (%obj.isLaserTurret && !%obj.allowColorChange)
		{
			return;
		}
		parent::setNodeColor(%obj, %node, %color);
	}

	function Armor::onDisabled(%this, %obj, %state)
	{
		if (isObject(%obj.laserDroneSet))
		{
			%obj.explodeLaserDrones();
			%obj.laserDroneSet.clear();
			%obj.laserDroneSet.delete();
		}

		if (isObject(%obj.staticShapeMount))
		{
			%obj.staticShapeMount.delete();
		}

		if (%this.getName() $= "droneBotArmor")
		{
			%obj.client = "";
			%obj.spawnExplosion(spawnProjectile, 1);
			%obj.schedule(50, delete);
			return;
		}

		return parent::onDisabled(%this, %obj, %state);
	}

	function Armor::onRemove(%this, %obj)
	{
		if (isObject(%obj.laserDroneSet))
		{
			%obj.explodeLaserDrones();
			%obj.laserDroneSet.clear();
			%obj.laserDroneSet.delete();
		}

		if (isObject(%obj.staticShapeMount))
		{
			%obj.staticShapeMount.delete();
		}

		return parent::onRemove(%this, %obj);
	}

	function Armor::damage(%this, %obj, %sourceObj, %pos, %damage, %damageType)
	{
		%pre = %obj.getDamageLevel();
		%ret = parent::damage(%this, %obj, %sourceObj, %pos, %damage, %damageType);
		if (%obj.getDamageLevel() > %pre && %this.getName() $= "droneBotArmor")
		{
			%obj.spawnExplosion(radioWaveProjectile, getWord(%obj.getScale(), 2) * 2);
		}
		return %ret;
	}

	function Player::emote(%obj, %emote)
	{
		if (%obj.getDatablock().disableEmotes)
		{
			return;
		}
		return parent::emote(%obj,%emote);
	}

	function Player::burn(%obj, %time)
	{
		if (%obj.getDatablock().disableBurn)
		{
			return;
		}
		return parent::burn(%obj, %time);
	}

	function minigameCanDamage(%client, %victimObject)
	{
		// if (%client.getType() & $Typemasks::PlayerObjectType && %client.getDatablock().getName() $= "droneBotArmor")
		// {
		// 	%client = %client.sourceClient;
		// }
		// if (%victimObject.getType() & $Typemasks::PlayerObjectType && %victimObject.getDatablock().getName() $= "droneBotArmor")
		// {
		// 	%victimObject = %victimObject.sourceClient;
		// }
		return parent::minigameCanDamage(%client, %victimObject);
	}

	function getBrickgroupFromObject(%obj)
	{
		if (%obj.getType() & $Typemasks::PlayerObjectType && %obj.getDatablock().getName() $= "droneBotArmor")
		{
			return %obj.sourceClient.brickGroup;
		}
		return parent::getBrickgroupFromObject(%obj);
	}

	function getMinigameFromObject(%obj)
	{
		if (%obj.getType() & $Typemasks::PlayerObjectType && %obj.getDatablock().getName() $= "droneBotArmor")
		{
			return %obj.sourceClient.minigame;
		}
		return parent::getMinigameFromObject(%obj);
	}
};
activatePackage(ChargeLaserDrones);



function laserDroneAILoop(%index)
{
	cancel($LaserDroneCheckSchedule);

	if (!isObject($LaserDroneSimSet))
	{
		return;
	}

	%count = $LaserDroneSimSet.getCount();
	if (%index >= %count) { %index = %count - 1; }
	
	while (%iter++ <= 32 && %count > 0)
	{
		if (%index < 0)
		{
			break;
		}

		%obj = $LaserDroneSimSet.getObject(%index);
		droneAICheck(%obj);
		%index--;
	}

	if (%index < 0) { %index = $LaserDroneSimSet.getCount(); }
	$LaserDroneCheckSchedule = schedule(33, $LaserDroneSimSet, laserDroneAILoop, %index);
}

laserDroneAILoop(0);

function Player::getLaserDroneSet(%pl)
{
	if (!isObject(%pl.laserDroneSet))
	{
		%pl.laserDroneSet = new SimSet();
	}
	return %pl.laserDroneSet;
}

function Player::explodeLaserDrones(%pl)
{
	%set = %pl.getLaserDroneSet();
	while (%set.getCount() > 0)
	{
		%obj = %set.getObject(0);
		%obj.spawnExplosion(spawnProjectile, 1);
		%obj.delete();
	}
}

function Player::spawnLaserDrone(%pl, %position, %rightImage, %leftImage, %faceVector)
{
	%droneSet = %pl.getLaserDroneSet();

	%mount = new StaticShape(){ dataBlock = droneBotMount; };
	%bot = new AIPlayer() { dataBlock = droneBotArmor; };

	%bot.staticShapeMount = %mount;
	%bot.sourceObject = %pl;
	%bot.sourceClient = %pl.client;
	%bot.client = %bot;
	%bot.isBot = 1;
	%bot.isLaserTurret = 1;
	%bot.maxYawSpeed = 50;
	%bot.maxPitchSpeed = 50;

	%mount.setTransform(%position);
	%mount.mountObject(%bot, 1);
	%bot.setTransform("0 0 0 0 0 1 " @ getRandom() * 3.14159 * 2);
	%bot.mountImage(droneJetImage, 2);

	if (isObject(%rightImage)) { %bot.mountImage(%rightImage, 0); }
	if (isObject(%leftImage)) { %bot.mountImage(%leftImage, 1); }

	%bot.setAimVector(%faceVector);
	%bot.playThread(2, passive);
	%bot.spawnExplosion(spawnProjectile, 1);

	$LaserDroneSimSet.add(%bot);
	%droneSet.add(%bot);
}

function droneAICheck(%drone)
{
	%drone.setVelocity("0 0 1");
	%target = getClosestMarkerlight(%drone, 
		%drone.getMountedImage(0).markerLightMaxRange, 
		3.14 * 2, 
		%drone.getMuzzleVector(0), 
		%drone.getMuzzlePoint(0),
		3.14 * 2);

	if (isObject(%target))
	{
		if (!isObject(%drone.target))
		{
			%gainedTarget = 1;
		}

		%drone.target = %target;
		%drone.setAimObject(%target);
		%drone.setImageTrigger(0, 1);
		%drone.triggerSchedule = %drone.schedule(500, setImageTrigger, 1, 1);
	}
	else
	{
		if (isObject(%drone.target))
		{
			%lostTarget = 1;
		}
		%drone.target = "";
		%drone.clearAim();
		%drone.setImageTrigger(0, 0);
		%drone.setImageTrigger(1, 0);
		cancel(%drone.triggerSchedule);
	}

	if (%lostTarget)
	{
		%drone.playThread(2, passive);
	}
	if (%gainedTarget)
	{
		%drone.playThread(2, root);
	}
	return (%drone.target + 0) SPC (%lostTarget + 0) SPC (%gainedTarget + 0);
}









//drone deploy item
datablock ProjectileData(droneDeployProjectile)
{
	projectileShapeName = "./resources/empGrenadeProjectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::electroDirect;
	radiusDamageType  = $DamageType::electroDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = ChargeLaserTracer;

	muzzleVelocity      = 15;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 19800;
	lifetime            = 20000;
	fadeDelay           = 19800;
	bounceElasticity    = 0.1;
	bounceFriction      = 0.8;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock ItemData(droneDeployItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./resources/empGrenade.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Drone Deployer";
	iconName = "";
	colorShiftColor = "1 0 0 1";
	doColorShift = true;

	image = droneDeployImage;
	canDrop = true;
	canPickupMultiple = 1;
};

datablock ShapeBaseImageData(droneDeployImage)
{
	shapeFile = "./resources/empGrenade.dts";
	emap = true;

	item = droneDeployItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = droneDeployItem.doColorShift;
	colorShiftColor = droneDeployItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = droneDeployProjectile;

	stateName[0]							= "Ready";
	stateSound[0]							= weaponSwitchSound;
	stateSequence[0]						= "root";
	stateTransitionOnTriggerDown[0]			= "Charge";

	stateName[1]							= "Charge";
	stateScript[1]							= "onChargeStart";
	stateSequence[1]						= "pinOut";
	stateSound[1]							= brickChangeSound;
	stateTransitionOnTriggerUp[1]			= "Fire";
	stateWaitForTimeout[1]					= false;

	stateName[2]							= "Fire";
	stateTransitionOnTimeout[2]				= "Ready";
	stateScript[2]							= "onFire";
	stateEjectShell[2]						= true;
	stateTimeoutValue[2]					= 0.3;
};

function droneDeployImage::onUnmount(%this, %obj, %slot)
{
	%obj.playThread(0, root);
	cancel(%obj.spearReadySched);
	%obj.droneDeploySlot = "";
}

function droneDeployImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%this.onFire(%obj, %slot);
}

function droneDeployImage::onChargeStart(%this, %obj, %slot)
{
	%obj.droneDeploySlot = %obj.currTool;
	%obj.playThread(0, shiftRight);
	%obj.spearReadySched = %obj.schedule(500, playThread, 0, spearReady);
}

function droneDeployImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftDown);
	%obj.playThread(0, spearThrow);
	serverPlay3D(weaponSwitchSound, %obj.getMuzzlePoint(%slot));

	%velocity = VectorScale(%obj.getMuzzleVector(%slot), %this.projectile.muzzleVelocity);
	
	%p = new Projectile()
	{
		dataBlock = %this.projectile;
		initialVelocity = %velocity;
		initialPosition = %obj.getEyePoint();
		sourceObject = %obj;
		sourceSlot = %slot;
		client = %obj.client;
	};

	//removal from inventory
	%obj.tool[%obj.droneDeploySlot] = "";
	if (isObject(%obj.client))
	{
		messageClient(%obj.client, 'MsgItemPickup', "", %obj.droneDeploySlot, 0);
	}
	%obj.unMountImage(%slot);
}

function droneDeployProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	serverPlay3D(empGrenadeBounceSound, %pos);
	if (isFunction(%obj.sourceObject.getClassName(), "spawnLaserDrone")
		&& %obj.sourceObject.getDamageState() $= "Enabled")
	{
		%faceVector = vectorNormalize(vectorSub(%pos, %obj.initialPosition));
		%obj.sourceObject.spawnLaserDrone(vectorAdd(%pos, vectorScale(%normal, 2)), 
			droneBurstGunImage, 
			droneBurstGunImageLeft,
			getWords(%faceVector, 0, 1));
	}
	%obj.delete();
}








datablock ShapeBaseImageData(droneBurstGunImage : SimpleChargeImageFramework_Auto)
{
	shapeFile = "./resources/droneBurstGun.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = "";

	correctMuzzleVector = true;
	className = "WeaponImage";

	projectile = ChargeLaserSMGProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	maxCharge = 10000; //clip
	chargeRate = 10000; //how fast to reload
	chargeTickTime = 100; //time between charge ticks, in milliseconds
	discharge = 0; //fire cost
	chargeDisableTime = 1000; //time between firing and charging resuming
	spread = 0.0012; //larger = more spread
	shellCount = 1; //projectiles per fire state
	
	markerLightSupport = 1;
	markerLightMaxRange = 128;
	markerLightSpread = 0.0012; //defaults to .spread, defined above
	markerLightAngleIgnore = 3.1415 * 2;

	stateSound[4]						= ChargeLaserQuadshotSound;
	stateTimeoutValue[5]				= 0.9;
	stateEmitter[5]						= LaserSmokeEmitter;
	stateEmitterTime[5]					= 0.7;
};
datablock ShapeBaseImageData(droneBurstGunImageLeft : droneBurstGunImage) { mountPoint = 1; };

function droneBurstGunImage::onFire(%this, %obj, %slot) { drone_SMGSubFire(%this, %obj, %slot, 0, 0); }
function droneBurstGunImageLeft::onFire(%this, %obj, %slot) { drone_SMGSubFire(%this, %obj, %slot, 0, 1); }

function drone_SMGSubFire(%this, %obj, %slot, %count, %left)
{
	if (%obj.getDamageState() !$= "Enabled" || %count >= 4 || %obj.getMountedImage(%slot) != %this)
	{
		return;
	}

	if (!%left)
	{
		%obj.playThread(0, shiftAway);
	}
	else
	{
		%obj.playThread(1, leftRecoil);
	}

	SimpleChargeImage::onFire(%this, %obj, %slot);
	cancel(%obj.subFireSchedule[%left]);
	%obj.subFireSchedule[%left] = schedule(80, %obj, drone_SMGSubFire, %this, %obj, %slot, %count + 1, %left);
}









datablock ShapeBaseImageData(droneRifleGunImage : SimpleChargeImageFramework_Auto)
{
	shapeFile = "./resources/droneRifleGun.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = "";

	correctMuzzleVector = true;
	className = "WeaponImage";

	projectile = ChargeLaserRifleProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	maxCharge = 10000; //clip
	chargeRate = 10000; //how fast to reload
	chargeTickTime = 100; //time between charge ticks, in milliseconds
	discharge = 0; //fire cost
	chargeDisableTime = 1000; //time between firing and charging resuming
	spread = 0.0002; //larger = more spread
	shellCount = 1; //projectiles per fire state
	
	markerLightSupport = 1;
	markerLightMaxRange = 256;
	markerLightSpread = 0.0012; //defaults to .spread, defined above
	markerLightAngleIgnore = 3.1415 * 2;

	stateSound[4]						= ChargeLaserRifleBlastSound;
	stateTimeoutValue[5]				= 0.9;
	stateEmitter[5]						= LaserSmokeEmitter;
	stateEmitterTime[5]					= 0.7;
};
datablock ShapeBaseImageData(droneRifleGunImageLeft : droneRifleGunImage) { mountPoint = 1; };

function droneRifleGunImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(0, shiftAway);
	SimpleChargeImage::onFire(%this, %obj, %slot);
}
function droneRifleGunImageLeft::onFire(%this, %obj, %slot)
{
	%obj.playThread(0, leftRecoil);
	SimpleChargeImage::onFire(%this, %obj, %slot);
}
