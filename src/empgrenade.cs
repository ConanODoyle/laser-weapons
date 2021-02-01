datablock ExplosionData(empGrenadeExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	lifeTimeMS = 150;

	soundProfile = empGrenadeExplosionSound;

	emitter[0] = empGrenadeExplosionCloudEmitter;
	emitter[1] = empGrenadeExplosionDebris2Emitter;

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
	impulseRadius = 6;
	impulseForce = 1200;

	damageRadius = 8;
	radiusDamage = 0;

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

	muzzleVelocity      = 30;
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
	bounceElasticity    = 0.6;
	bounceFriction      = 0.3;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	markerlightTime = 16000;

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
	colorShiftColor = "1 1 1 1";
	doColorShift = false;

	image = empGrenadeImage;
	canDrop = true;
	canPickupMultiple = 1;
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
	stateSound[0]							= weaponSwitchSound;
	stateSequence[0]						= "root";
	stateTransitionOnTriggerDown[0]			= "Charge";

	stateName[1]							= "Charge";
	stateTransitionOnTimeout[1]				= "Cancel";
	stateScript[1]							= "onChargeStart";
	stateSequence[1]						= "pinOut";
	stateSound[1]							= brickChangeSound;
	stateTimeoutValue[1]					= (empGrenadeProjectile.lifeTime * 32) / 1000;
	stateTransitionOnTriggerUp[1]			= "Fire";
	stateWaitForTimeout[1]					= false;

	stateName[4]							= "Cancel";
	stateScript[4]							= "onChargeStop";
	stateSequence[4]						= "pinOut";
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

function empGrenadeImage::onUnmount(%this, %obj, %slot)
{
	%obj.playThread(0, root);
	cancel(%obj.spearReadySched);
}

function empGrenadeImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	//%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function empGrenadeImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartToolSlot = %obj.currTool;
	%obj.chargeStartTime[%obj.chargeStartToolSlot] = getSimTime();
	%obj.cookPrint(%obj.currTool);
	%obj.playthread(0, shiftRight);
	%obj.spearReadySched = %obj.schedule(500, playThread, 0, spearReady);
}

function empGrenadeImage::onFire(%this, %obj, %slot)
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

	%p.cookDeath = %p.schedule((%this.projectile.lifeTime * 32) - (getSimTime() - %obj.chargeStartTime[%obj.chargeStartToolSlot]), FuseExplode);
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
	%obj.collideCount++;

	if (%obj.collideCount > 3)
	{
		%obj.FuseExplode();
	}
}

function empGrenadeProjectile::onExplode(%this, %obj, %pos)
{
	initContainerRadiusSearch(%pos, %this.explosion.damageRadius, $TypeMasks::PlayerObjectType);
	while (isObject(%col = ContainerSearchNext()))
	{
		if (minigameCanDamage(%obj, %col) == 1)
		{
			%dist = vectorDist(%pos, %col.getHackPosition());

			%visible = obstructRadiusDamageCheck(%pos, %col);

			if (%visible)
			{
				%col.zapTicks = 3;
				%col.mountImage(electroZapImage, 1);
				%col.attachMarkerlight(%this.markerlightTime);
			}
		}
	}
	%count = getRandom(12, 20);
	for (%i = 0; %i < %count; %i++)
	{
		%shape = new StaticShape(lightningshape)
		{
			dataBlock = empLightningShape;
		};
		
		%rand = getRandom();
		%x = mSin(%rand * 3.141592 * 2); %y = mCos(%rand * 3.141592 * 2);
		%rand = getRandom();
		if (getRandom() > 0.5) { %neg = 1; } else { %neg = -1; }
		%z = mPow(1 - mPow(%rand, 2), 0.5) * %neg;
		%xy = vectorScale(vectorNormalize(%x SPC %y SPC 0), %rand);
		%xyz = vectorNormalize(getWords(%xy, 0, 1) SPC %z);
		%shape.setTransform(%pos SPC %xyz SPC (getRandom() - 0.5) * $pi * 4);

		%rand = getRandom() * 0.5 + 0.5;
		%shape.setScale(%rand SPC %rand SPC %rand);

		%shape.schedule(getRandom() * 100 + 50, delete);
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
	
	%maxTime = %pl.tool[%toolSlot].image.projectile.lifeTime * 32;
	%timeLeft = %maxTime - (getSimTime() - %pl.chargeStartTime[%toolSlot]);
	
	%diff = %timeLeft / %maxTime;
	if (%diff > 0.5)
	{
		%r = (1 - %diff) * 2;
		%g = 1;
	}
	else
	{
		%r = 1;
		%g = %diff * 2;
	}
	%color = %r SPC %g SPC 0;


	%bar = getCookBar(%maxTime, %timeLeft, 12, "-");
	%cl.centerPrint("<font:Consolas:20><color:" @ hexFromRGB(%color) @ ">[" @ mFloatLength(%timeLeft / 1000, 2) @ "s] <br><font:Impact:36>" @ %bar, 1);

	%pl.cookSched = %pl.schedule(33, cookPrint, %toolSlot);
}

function getCookBar(%maxcharge, %currCharge, %totalBars, %char)
{
	%numColoredBars = mCeil(%currCharge/%maxcharge*%totalBars);
	%char = %char $= "" ? "=" : %char;
	%bars = "\c0";
	%i = 0;

	for (%i = 0; %i < %totalBars - %numColoredBars; %i++)
	{
		%bars = %bars @ %char;
	}

	%threshold = %totalBars; //red threshold
	if (%i < %threshold)
	{
		%bars = %bars @ "\c2";
		for (%j = %i; %j < %threshold; %j++)
		{
			%bars = %bars @ %char;
			%i += 1;
		}
	}
	return %bars;
}