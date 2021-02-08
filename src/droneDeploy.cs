//drone deploy item
datablock ProjectileData(droneDeployProjectile)
{
	projectileShapeName = "./resources/droneItem.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::electroDirect;
	radiusDamageType  = $DamageType::electroDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = ChargeLaserTracer;

	muzzleVelocity      = 18;
	velInheritFactor    = 1;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 800;
	lifetime            = 1000;
	fadeDelay           = 800;
	bounceElasticity    = 0.8;
	bounceFriction      = 0.3;
	isBallistic         = true;
	gravityMod = 0.8;

	hasLight    = false;
	lightRadius = 1;
	lightColor  = "0 0 0";

	uiName = "";
};

datablock ItemData(droneBurstGunDeployItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./resources/droneItem.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Burst Drone";
	iconName = "./resources/icon_drone";
	colorShiftColor = "1 1 1 1";
	doColorShift = true;

	image = droneBurstGunDeployImage;
	canDrop = true;
	canPickupMultiple = 1;
};

datablock ShapeBaseImageData(droneBurstGunDeployImage)
{
	shapeFile = "./resources/droneItem.dts";
	emap = true;

	item = droneBurstGunDeployItem;

	mountPoint = 0;
	offset = "-0.561148 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = droneBurstGunDeployItem.doColorShift;
	colorShiftColor = droneBurstGunDeployItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = droneDeployProjectile;

	stateName[0]							= "Activate";
	stateTimeoutValue[0]					= 0.15;
	stateTransitionOnTimeout[0]				= "Ready";

	stateName[1]							= "Ready";
	stateSound[1]							= weaponSwitchSound;
	stateSequence[1]						= "root";
	stateTransitionOnTriggerDown[1]			= "Fire";

	stateName[2]							= "Fire";
	stateTransitionOnTimeout[2]				= "Ready";
	stateScript[2]							= "onFire";
	stateEjectShell[2]						= true;
	stateTimeoutValue[2]					= 0.3;
};

datablock ItemData(droneRifleGunDeployItem : droneBurstGunDeployItem)
{
	uiName = "Rifle Drone";
	image = droneRifleGunDeployImage;
	colorShiftColor = "0.7 0.7 0.7 1";
};

datablock ShapeBaseImageData(droneRifleGunDeployImage : droneBurstGunDeployImage)
{
	item = droneRifleGunDeployItem;
	colorShiftColor = "0.7 0.7 0.7 1";
};



function droneBurstGunDeployImage::onMount(%this, %obj, %slot)
{
	%obj.playThread(1, armReadyBoth);
	if (isObject(%obj.client))
	{
		if ($Pref::Server::MarkerLasers::ClickRecoverDrone)
		{
			%pickup = " <br>\c2[[ CLICK DEPLOYED DRONE TO PICK UP]]";
		}
		%obj.client.centerprint("<br><br><br><font:Consolas:16>\c6[[ CLICK TO DEPLOY ]]" @ %pickup, 6);
	}
}

function droneRifleGunDeployImage::onMount(%this, %obj, %slot)
{
	%obj.playThread(1, armReadyBoth);
	if (isObject(%obj.client))
	{
		if ($Pref::Server::MarkerLasers::ClickRecoverDrone)
		{
			%pickup = " <br>\c2[[ CLICK DEPLOYED DRONE TO PICK UP]]";
		}
		%obj.client.centerprint("<br><br><br><font:Consolas:16>\c6[[ CLICK TO DEPLOY ]]" @ %pickup, 6);
	}
}

function droneBurstGunDeployImage::onUnmount(%this, %obj, %slot) { if (isObject(%obj.client)) %obj.client.centerprint("", 1); }
function droneRifleGunDeployImage::onUnmount(%this, %obj, %slot) { if (isObject(%obj.client)) %obj.client.centerprint("", 1); }

function droneBurstGunDeployImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(1, activate2);
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
		rightImage = droneBurstGunImage;
		leftImage = droneBurstGunImageLeft;
		colorScale = getWord(%this.colorShiftColor, 0);
		itemDB = %this.item;
	};

	//removal from inventory
	%obj.tool[%obj.currTool] = "";
	if (isObject(%obj.client))
	{
		messageClient(%obj.client, 'MsgItemPickup', "", %obj.currTool, 0);
	}
	%obj.unMountImage(%slot);
}

function droneRifleGunDeployImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(1, activate2);
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
		rightImage = droneRifleGunImage;
		leftImage = droneRifleGunImageLeft;
		colorScale = getWord(%this.colorShiftColor, 0);
		itemDB = %this.item;
	};

	//removal from inventory
	%obj.tool[%obj.currTool] = "";
	if (isObject(%obj.client))
	{
		messageClient(%obj.client, 'MsgItemPickup', "", %obj.currTool, 0);
	}
	%obj.unMountImage(%slot);
}



function droneDeployProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	serverPlay3D(empGrenadeBounceSound, %pos);
	if (!(%col.getType() & $Typemasks::PlayerObjectType))
	{
		%obj.hitNormal = %normal;
		// %obj.explode();
	}
}

function droneDeployProjectile::onExplode(%this, %obj, %pos)
{
	if (isFunction(%obj.sourceObject.getClassName(), "spawnLaserDrone")
		&& %obj.sourceObject.getDamageState() $= "Enabled")
	{
		initContainerRadiusSearch(%pos, 1, $Typemasks::StaticShapeObjectType);
		while (isObject(%next = containerSearchNext()))
		{
			if (%next.getDatablock().getName() $= "droneBotMount")
			{
				%i = new Item()
				{
					dataBlock = %obj.itemDB;
				};
				MissionCleanup.add(%i);
				%i.setScale(%obj.getScale());
				%i.setTransform(%obj.getTransform());
				%i.schedulePop();
				return;
			}
		}
		%faceVector = vectorNormalize(vectorSub(%pos, %obj.initialPosition));
		%obj.sourceObject.spawnLaserDrone(vectorAdd(%pos, vectorScale(%obj.hitNormal, 1)), 
			%obj.rightImage, 
			%obj.leftImage,
			vectorNormalize(getWords(%faceVector, 0, 1)),
			%obj.colorScale,
			%obj.itemDB);
	}
	Parent::onExplode(%this, %obj, %pos);
}