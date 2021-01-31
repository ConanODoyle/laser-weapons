datablock ExplosionData(empGrenadeExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	lifeTimeMS = 150;

	soundProfile = empGrenadeExplosionSound;

	emitter[0] = empGrenadeExplosionCloudEmitter;
	emitter[1] = grenade_concExplosionDebris2Emitter;

	particleEmitter = empGrenadeExplosionHazeEmitter;
	particleDensity = 100;
	particleRadius = 1.0;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "7.0 8.0 7.0";
	camShakeAmp = "4.0 4.0 4.0";
	camShakeDuration = 2.3;
	camShakeRadius = 18.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 12;
	impulseForce = 3500;

	damageRadius = 12;
	radiusDamage = 10;

	uiName = "";
};

datablock ProjectileData(empGrenadeProjectile)
{
	projectileShapeName = "./resources/empGrenadeProjectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::electroDirect;
	radiusDamageType  = $DamageType::electroDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = empGrenadeExplosion;
	particleEmitter     = "";

	muzzleVelocity      = 33;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 3980; 
	lifetime            = 4000;
	fadeDelay           = 3980;
	bounceElasticity    = 0.3;
	bounceFriction      = 0.2;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock ItemData(empGrenadeItem)
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

	uiName = "EMP Grenade";
	iconName = "";
	doColorShift = false;

	image = empGrenadeImage;
	canDrop = true;
};

datablock ShapeBaseImageData(empGrenadeImage)
{
	shapeFile = "./resources/empGrenade.dts";
	emap = true;

	item = empGrenadeItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = empGrenadeItem.doColorShift;
	colorShiftColor = empGrenadeItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = empGrenadeProjectile;

	stateName[0]							= "Ready";
//	stateScript[0]							= "onReady";
	stateSequence[0]						= "root";
	stateTransitionOnTriggerDown[0]			= "Charge";

	stateName[1]							= "Charge";
	stateTransitionOnTimeout[1]				= "Cancel";
	stateScript[1]							= "onChargeStart";
	stateSequence[1]						= "noSpoon";
	stateTimeoutValue[1]					= 4.0;
	stateTransitionOnTriggerUp[1]			= "Fire";
	stateWaitForTimeout[1]					= false;

	stateName[4]							= "Cancel";
	stateScript[4]							= "onChargeStop";
	stateSequence[4]						= "noSpoon";
	stateTransitionOnTimeout[4]				= "Ready";
	stateTimeoutValue[4]					= 0.1;

	stateName[3]							= "Next";
	stateTimeoutValue[3]					= 0.1;
	stateTransitionOnTriggerUp[3]			= "Ready";
	stateTransitionOnNoAmmo[3]				= "Fire";

	stateName[2]							= "Fire";
	stateTransitionOnTimeout[2]				= "Ready";
	stateScript[2]							= "onFire";
	stateEjectShell[2]						= true;
	stateTimeoutValue[2]					= 0.3;
};

function empGrenadeImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	//%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function empGrenadeImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartToolSlot = %obj.currTool;
	%obj.chargeStartTime[%obj.chargeStartToolSlot] = getSimTime();
	serverPlay3D(empGrenadePinSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%obj.currTool);
}

function empGrenadeImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftDown);
	serverPlay3D(weaponSwitchSound, %obj.getMuzzlePoint(%slot));

	%velocity = VectorScale(%vec, %projectile.muzzleVelocity);
	
	%p = new Projectile()
	{
		dataBlock = %this.projectile;
		initialVelocity = %velocity;
		initialPosition = %obj.getEyePoint();
		sourceObject = %obj;
		sourceSlot = %slot;
		client = %obj.client;
	};

	%p.cookDeath = %p.schedule((%p.getDatablock().lifeTime * 32) - (getSimTime() - %obj.chargeStartTime[%this]), FuseExplode);
	%obj.chargeStartTime[%obj.chargeStartToolSlot] = "";
	
	//removal from inventory
	%obj.unMountImage(%slot);
	%obj.tool[%obj.chargeStartToolSlot] = "";
	if (isObject(%obj.client))
	{
		messageClient(%obj.client, 'MsgItemPickup', "", %obj.chargeStartToolSlot, 0);
	}
}

function empGrenadeProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	serverPlay3D(empGrenadeBounceSound, %pos); 
}

function empGrenadeProjectile::onExplode(%this, %obj, %pos)
{
	initContainerRadiusSearch(%pos, %this.damageRadius, $TypeMasks::PlayerObjectType);
	while (isObject(%col = ContainerSearchNext()))
	{
		if (minigameCanDamage(%obj, %col) == 1)
		{
			%dist = vectorDist(%pos, %col.getHackPosition());

			%obstructed = obstructRadiusDamageCheck(%pos, %col);

			if (!%obstructed)
			{
				%col.zapTicks = 3;
				%col.mountImage(electroZapImage, 1);
				if (isObject("HatDizzyData"))
				{
					%col.mountImage(HatDizzyData, 2);
				}
			}
		}
	}
	Parent::onExplode(%this, %obj, %pos);
}

function Projectile::FuseExplode(%proj) //this function fixes fuse time at the cost of discarding any non default fields
{
	%db = %proj.getDatablock();
	%vel = %proj.getVelocity();
	%pos = %proj.getPosition();
	%sObj = %proj.sourceObject;
	%sSlot = %proj.sourceSlot;
	%cl = %proj.client;

	%proj.delete();

	if (vectorLen(%vel) == 0)
		%vel = "0 0 0.1";

	%p = new Projectile()
	{
		dataBlock = %db;
		initialVelocity = %vel;
		initialPosition = %pos;
		sourceObject = %sObj;
		sourceSlot = %sSlot;
		client = %cl;
	};
	
	MissionCleanup.add(%p);

	%p.explode();
}

function Player::cookPrint(%pl, %toolSlot)
{
	if (!isObject(%pl) || !isObject(%cl = %pl.client))
	{
		return;
	}
	
	cancel(%pl.cookSched);

	if (%pl.chargeStartTime[%toolSlot] $= "" 
		|| !isObject(%pl.getMountedImage(0)) 
		|| %pl.getMountedImage(0).getID() != %pl.tool[%toolSlot].image.getID())
	{
		return;
	}
	
	%time = mFloatLength(((%pl.tool[%toolSlot].image.Projectile.lifeTime * 32) - (getSimTime() - %pl.chargeStartTime[%toolSlot])) / 1000, 1);
	%cl.centerPrint("<color:7F4FA8>" @ %time @ "s left!", 1);

	%pl.cookSched = %pl.schedule(100, cookPrint, %toolSlot);
}