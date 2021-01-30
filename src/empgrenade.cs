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

	damageRadius = 0;
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
	%obj.chargeStartTool = %obj.currTool;
	%obj.chargeStartTime[%obj.chargeStartTool] = getSimTime();
	serverPlay3D(empGrenadePinSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function empGrenadeImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftDown);
	serverPlay3D(weaponSwitchSound, %obj.getMuzzlePoint(%slot));
	%projs = ProjectileFire(%this.projectile, %obj.getEyePoint(), 
		%obj.getMuzzleVector(%slot), 0, 1, 
		%slot, %obj, %obj.client);

	for(%i = 0; %i < getFieldCount(%projs); %i++)
	{
		%proj = getField(%projs, %i);
		%proj.cookDeath = %proj.schedule((%proj.getDatablock().lifeTime * 32) - (getSimTime() - %obj.chargeStartTime[%this]), FuseExplode);
	}

	%obj.chargeStartTime[%obj.chargeStartTool] = "";
	
	//removal from inventory
	%obj.unMountImage(%slot);
	%obj.tool[%obj.chargeStartTool] = "";
	if (isObject(%obj.client))
	{
		messageClient(%obj.client, 'MsgItemPickup', "", 0, %obj.chargeStartTool);
	}
}

function empGrenadeProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	serverPlay3D(grenade_bounceSound,%pos); 
}

function empGrenadeProjectile::onExplode(%this, %obj, %pos)
{
	initContainerRadiusSearch(%pos, 16, $TypeMasks::PlayerObjectType);
	while(isObject(%col = ContainerSearchNext()))
	{
		if(minigameCanDamage(%obj, %col) == 1)
		{
			%dist = vectorDist(%pos, %col.getHackPosition());

			if(!isObject(firstWord(containerRayCast(%pos,%col.getHackPosition(),$TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType, %col))))
			{
				%col.zapTicks = 3;
				%col.mountImage(electroZapImage, 2);
			}
		}
	}
	Parent::onExplode(%this, %obj, %pos);
}