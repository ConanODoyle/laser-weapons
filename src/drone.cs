
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

	maxDamage = 400;
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
			%obj.spawnExplosion(spawnExplosion, 1);
			%obj.schedule(50, delete);
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
		%index--;

		if ((%obj.LaserDroneExpireTime >= 0 && %obj.LaserDroneExpireTime < getSimTime()) 
			|| %obj.getDamageState() !$= "Enabled")
		{
			%removeList[%removeCount++ - 1] = %obj;
			continue;
		}

		//TODO: run AI function here
	}

	for (%i = 0; %i < %removeCount; %i++)
	{
		if (isObject(%removeList[%i])) { $LaserDroneSimSet.remove(%removeList[%i]); }
	}

	if (%index < 0) { %index = $LaserDroneSimSet.getCount(); }
	$LaserDroneCheckSchedule = schedule(33, $LaserDroneSimSet, laserDroneAILoop, %index);
}

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
	for (%i = 0; %i < %set.getCount(); %i++)
	{
		%obj = %set.getObject(%i);
		%obj.spawnExplosion("1 1 1", PlayerSpawnProjectile);
		// %obj.schedule(33, delete);
	}
}

function Player::spawnLaserDrone(%pl, %position)
{
	%droneSet = %pl.getLaserDroneSet();

	%mount = new StaticShape(){ dataBlock = droneBotMount; };
	%bot = new AIPlayer() { dataBlock = droneBotArmor; };

	%bot.staticShapeMount = %mount;
	%bot.sourceObject = %pl;
	%bot.sourceClient = %pl.client;

	%mount.mountObject(%bot, 1);
	%mount.setTransform(%position);
	%bot.mountImage(droneJetImage, 2);

	$LaserDroneSimSet.add(%bot);
	%droneSet.add(%bot);
}










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
}

function droneDeployImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	//%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function droneDeployImage::onChargeStart(%this, %obj, %slot)
{
	%obj.playthread(0, shiftRight);
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
	%obj.unMountImage(%slot);
	%obj.tool[%obj.chargeStartToolSlot] = "";
	if (isObject(%obj.client))
	{
		messageClient(%obj.client, 'MsgItemPickup', "", %obj.chargeStartToolSlot, 0);
	}
}

function droneDeployProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	serverPlay3D(empGrenadeBounceSound, %pos);
	talk(%obj.sourceObject);
	if (isFunction(%obj.sourceObject.getClassName(), "spawnLaserDrone")
		&& %obj.sourceObject.getDamageState() $= "Enabled")
	{
		%obj.sourceObject.spawnLaserDrone(vectorAdd(%pos, "0 0 2"));
	}
	%obj.delete();
}