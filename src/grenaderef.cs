function SimObject::IsA(%obj, %type) { return %obj.getClassName() $= %type; }

function ProjectileFire(%db, %pos, %vec, %spd, %amt, %srcSlot, %srcObj, %srcCli)
{
	%projectile = %db;
	%spread = %spd / 1000;
	%shellcount = %amt;

	%shells = -1;

	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%velocity = VectorScale(%vec, %projectile.muzzleVelocity); // fuck velocity inheritance :DD
		%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
		%velocity = MatrixMulVector(%mat, %velocity);

		%p = new Projectile()
		{
			dataBlock = %projectile;
			initialVelocity = %velocity;
			initialPosition = %pos;
			sourceObject = %srcObj;
			sourceSlot = %srcSlot;
			client = %srcCli;
		};
		MissionCleanup.add(%p);

		%shells = %shells TAB %p;
	}

	return removeField(%shells, 0);
}

function Projectile::FuseExplode(%proj) //this function fixes fuse time at the cost of discarding any non default fields
{
	%db = %proj.getDatablock();
	%vel = %proj.getVelocity();
	%pos = %proj.getPosition();
	%sObj = %proj.sourceObject;
	%sSlot = %proj.sourceSlot;
	%cli = %proj.client;

	%proj.delete();

	if(vectorLen(%vel) == 0)
		%vel = "0 0 0.1";

	%p = new Projectile()
	{
		dataBlock = %db;
		initialVelocity = %vel;
		initialPosition = %pos;
		sourceObject = %sObj;
		sourceSlot = %sSlot;
		client = %cli;
	};
	
	MissionCleanup.add(%p);

	%p.explode();
}

function Player::cookPrint(%pl, %img)
{
	if(!isObject(%pl) || !isObject(%cl = %pl.client))
		return;
	
	cancel(%pl.cookSched);

	if(%pl.chargeStartTime[%img] $= "" || !isObject(%pl.getMountedImage(0)) || %pl.getMountedImage(0).getID() != %img.getID())
		return;
	
	%cl.centerPrint("<color:7F4FA8>" @ mFloatLength(((%img.Projectile.lifeTime * 32) - (getSimTime() - %pl.chargeStartTime[%img])) / 1000, 1) @ "s left!", 1);

	%pl.cookSched = %pl.schedule(100, cookPrint, %img);
}

package AudioRandomPitch
{
	function GameConnection::Play3D(%cl, %sound, %pos)
	{
		%pitchDev = %sound.pitchRange;
		if(%pitchDev $= "") return Parent::Play3D(%cl, %sound, %pos);
		
		%maxPitch = 100 + %pitchDev;
		%minPitch = 100 - %pitchDev;
		
		%pitch = (getRandom(%minPitch, %maxPitch)) / 100;

		%oldTimescale = getTimescale();
		setTimescale(%pitch);
		
		Parent::Play3D(%cl, %sound, %pos);
		
		setTimescale(%oldTimescale);
	}

	function GameConnection::Play2D(%cl, %sound)
	{
		%pitchDev = %sound.pitchRange;
		if(%pitchDev $= "") return Parent::Play2D(%cl, %sound);
		
		%maxPitch = 100 + %pitchDev;
		%minPitch = 100 - %pitchDev;
		
		%pitch = (getRandom(%minPitch, %maxPitch)) / 100;

		%oldTimescale = getTimescale();
		setTimescale(%pitch);
		
		Parent::Play2D(%cl, %sound);
		
		setTimescale(%oldTimescale);
	}
};
activatePackage(AudioRandomPitch);

function GameConnection::Play2DSpeed(%cl, %sound, %speed)
{
	%pitch = %speed;// / 100;

	%oldTimescale = getTimescale();
	setTimescale(%pitch);
	
	%cl.Play2D(%sound);
	
	setTimescale(%oldTimescale);
}












datablock ParticleData(grenade_concExplosionDebris2Particle)
{
	dragCoefficient		= 1.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.4;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 2000;
	lifetimeVarianceMS	= 1990;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/dot";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]     = "1 0.5 0.0 1";
	colors[1]     = "0.9 0.5 0.0 0.9";
	colors[2]     = "1 1 1 0.0";

	sizes[0]	= 0.3;
	sizes[1]	= 0.3;
	sizes[2]	= 0.3;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(grenade_concExplosionDebris2Emitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	lifetimeMS       = 140;
	ejectionVelocity = 12;
	velocityVariance = 12.0;
	ejectionOffset   = 1.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "grenade_concExplosionDebris2Particle";
};

datablock ParticleData(grenade_electroExplosionHazeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0;
	windCoefficient		= 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 900;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
   	colors[0]     = "0.7 0.7 0.9 0.8";
   	colors[1]     = "0.4 0.4 0.7 0.1";
   	colors[2]     = "0.1 0.1 0.4 0.0";
	times[0]	= 0.0;
	times[1]	= 0.3;
  times[2]	= 1.0;
		
	sizes[0]      = 3.0;
	sizes[1]      = 3.85;
	sizes[2]      = 4.35;

	useInvAlpha = false;
};
datablock ParticleEmitterData(grenade_electroExplosionHazeEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 6;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_electroExplosionHazeParticle";

   useEmitterColors = true;
};

datablock ParticleData(grenade_electroExplosionCloudParticle)
{
	dragCoefficient		= 0.5;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.9;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 900;
	lifetimeVarianceMS	= 00;
	spinSpeed		= 0.0;
	spinRandomMin		= -90.0;
	spinRandomMax		= 90.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/star1";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
   colors[0]     = "0.1 0.3 1.0 0.8";
   colors[1]     = "0.1 0.1 0.1 0.0";
	sizes[0]      = 2;
	sizes[1]      = 0.5;
};

datablock ParticleEmitterData(grenade_electroExplosionCloudEmitter)
{
   ejectionPeriodMS = 4;
   periodVarianceMS = 0;
   ejectionVelocity = 7;
   velocityVariance = 1.0;
   ejectionOffset   = 1.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "grenade_electroExplosionCloudParticle";
};

datablock AudioProfile(grenade_electroExplosionSound)
{
	filename    = "Add-ons/Gamemode_Core_Rush/src/snd/blast_electric.wav";
	description = AudioDefault3D;
	preload = true;

	pitchRange = 7;
};














exec("./weapon_electro_datablocks.cs");

datablock ExplosionData(grenade_electroExplosion)
{
   explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
   lifeTimeMS = 150;

   soundProfile = grenade_electroExplosionSound;
   
   emitter[0] = grenade_electroExplosionCloudEmitter;
	 emitter[1] = grenade_concExplosionDebris2Emitter;

   particleEmitter = grenade_electroExplosionHazeEmitter;
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

datablock ProjectileData(grenade_electroProjectile)
{
	projectileShapeName = "Add-ons/Gamemode_Core_Rush/src/weps/res/electro_projectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::electroDirect;
	radiusDamageType  = $DamageType::electroDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = grenade_electroExplosion;
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

datablock ItemData(grenade_electroItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/electro_item.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Electro Nade";
	iconName = "";
	doColorShift = false;

	image = grenade_electroImage;
	canDrop = true;
};

datablock ShapeBaseImageData(grenade_electroImage)
{
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/electro_image.dts";
	emap = true;

	item = grenade_electroItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = grenade_electroItem.doColorShift;
	colorShiftColor = grenade_electroItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = grenade_electroProjectile;

	stateName[0]                     = "Ready";
	stateScript[0]								= "onReady";
	stateSequence[0]			 = "root";
	stateTransitionOnTriggerDown[0]  = "Charge";

	stateName[1]                     = "Charge";
	stateTransitionOnTimeout[1]      = "Cancel";
	stateScript[1]                   = "onChargeStart";
	stateSequence[1]			 = "noSpoon";
	stateTimeoutValue[1]		   = 4.0;
	stateTransitionOnTriggerUp[1] = "Fire";
	stateWaitForTimeout[1] = false;

	stateName[4] 				= "Cancel";
	stateScript[4]                   = "onChargeStop";
	stateSequence[4]			 = "noSpoon";
	stateTransitionOnTimeout[4] = "Ready";
	stateTimeoutValue[4]				= 0.1;

	stateName[3]                     = "Next";
	stateTimeoutValue[3]		   = 0.1;
	stateTransitionOnTriggerUp[3]      = "Ready";
	stateTransitionOnNoAmmo[3] = "Fire";

	stateName[2]                     = "Fire";
	stateTransitionOnTimeout[2]      = "Ready";
	stateScript[2]                   = "onFire";
	stateEjectShell[2] 				= true;
	stateTimeoutValue[2]		   = 0.3;
};

function grenade_electroImage::onReady(%this, %obj, %slot)
{
	%obj.weaponAmmoStart();
}

function grenade_electroImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%obj.damage(%obj, %obj.getHackPosition(), 33, $DamageType::Suicide);
	%this.onFire(%obj, %slot);
}

function grenade_electroImage::onChargeStart(%this, %obj, %slot)
{
	%obj.chargeStartTime[%this] = getSimTime();
	serverPlay3D(grenade_pinLightSound, %obj.getMuzzlePoint(%slot));
	%obj.cookPrint(%this);
}

function grenade_electroImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftDown);
	%obj.weaponAmmoUse();
	serverPlay3D(grenade_throwSound, %obj.getMuzzlePoint(%slot));
	%projs = ProjectileFire(%this.Projectile, %obj.getEyePoint(), %obj.getMuzzleVector(%slot), 0, 1, %slot, %obj, %obj.client);
	for(%i = 0; %i < getFieldCount(%projs); %i++)
	{
		%proj = getField(%projs, %i);
		%proj.cookDeath = %proj.schedule((%proj.getDatablock().lifeTime * 32) - (getSimTime() - %obj.chargeStartTime[%this]), FuseExplode);
	}

	%obj.chargeStartTime[%this] = "";
	%obj.unMountImage(%slot);
}

function grenade_electroProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) { serverPlay3D(grenade_bounceSound,%pos); }

function grenade_electroProjectile::onExplode(%this, %obj, %pos)
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