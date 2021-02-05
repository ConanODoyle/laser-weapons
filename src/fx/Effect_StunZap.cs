datablock ParticleData(electrocuteParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 50;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 1000.0;
	spinRandomMin		= -5000.0;
	spinRandomMax		= 5000.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/star1";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]     = "1 1 1 1";
	colors[1]     = "0 0 0 0.7";
	colors[2]	= "0 0 0 0.7";
	sizes[0]	= 2;
	sizes[1]	= 3;
	sizes[2]	= 2;
	times[0]	= 0.0;
	times[1]	= 0.4;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(electrocuteEmitter)
{
	ejectionPeriodMS = 10;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = electrocuteParticle;
};

datablock ParticleData(electrocuteSmogParticle)
{
	dragCoefficient      = 5;
	gravityCoefficient   = -0.2;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 1000;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "0.3 0.3 0.3 0.1";
	colors[1]     = "0.3 0.3 0.3 0.0";
	sizes[0]      = 2.13;
	sizes[1]      = 2.0;
};

datablock ParticleEmitterData(electrocuteSmogEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 2;
	velocityVariance = 0.0;
	ejectionOffset   = 0.5;
	thetaMin         = 80;
	thetaMax         = 80;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "electrocuteSmogParticle";

	useEmitterColors = true;
};

datablock ExplosionData(electrocuteSmogExplosion)
{
   //explosionShape = "";
	lifeTimeMS = 150;

	particleEmitter = electrocuteSmogEmitter;
	particleDensity = 17;
	particleRadius = 0.2;

	emitter[0] = "";

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = false;
	camShakeFreq = "10.0 11.0 10.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 1;
	lightStartColor = "0.3 0.6 0.7";
	lightEndColor = "0 0 0";
};

datablock ProjectileData(electrocuteProjectile)
{
	directDamage        = 0;
	directDamageType    = $DamageType::electrocutegun;
	radiusDamageType    = $DamageType::electrocutegun;

	brickExplosionRadius = 0;
	brickExplosionImpact = true;          //destroy a brick if we hit it directly?
	brickExplosionForce  = 0;
	brickExplosionMaxVolume = 1;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 2;  //max volume of bricks that we can destroy if they aren't connected to the ground

	impactImpulse	     = 300;
	verticalImpulse     = 20;

	muzzleVelocity      = 0;
	velInheritFactor    = 0;

	explosion             = electrocuteSmogExplosion;

	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  
	armingDelay         = 4000;
	lifetime            = 4000;
	fadeDelay           = 3500;
	bounceElasticity    = 0.999;
	bounceFriction      = 0.10;
	isBallistic         = true;
	gravityMod = 0.8;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(electroZapImage)
{
	// Basic Item properties
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;

	mountPoint = 3;
	offset = "-0.2 0 1";
	eyeOffset = 0; //"0.7 1.2 -0.5";
	rotation = eulerToMatrix( "0 0 0" );

	correctMuzzleVector = true;

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = electrocuteItem;
	ammo = " ";
	projectile = electrocuteProjectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;

	doColorShift = true;
	colorShiftColor = electrocutegunItem.colorShiftColor;

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.25;
	stateScript[0]                  = "onZappedA";
	stateTransitionOnTimeout[0]       = "ZappedB";
	stateEmitter[0]			= electrocuteEmitter;
	stateEmitterTime[0]		= 0.2;
	stateSound[0]					= radioWaveExplosionSound;

	stateName[1]                     = "ZappedB";
	stateTimeoutValue[1]             = 0.25;
	stateScript[1]                  = "onZappedB";
	stateEmitter[1]			= electrocuteEmitter;
	stateEmitterTime[1]		= 0.2;
	stateTransitionOnTimeout[1]       = "ZappedC";

	stateName[2]                     = "ZappedC";
	stateTimeoutValue[2]             = 0.25;
	stateScript[2]                  = "onZappedA";
	stateEmitter[2]			= electrocuteEmitter;
	stateEmitterTime[2]		= 0.2;
	stateTransitionOnTimeout[2]       = "ZappedD";

	stateName[3]                     = "ZappedD";
	stateTimeoutValue[3]             = 0.25;
	stateScript[3]                  = "onZappedB";
	stateEmitter[3]			= electrocuteEmitter;
	stateEmitterTime[3]		= 0.2;
	stateTransitionOnTimeout[3]       = "ZappedE";

	stateName[4]                     = "ZappedE";
	stateTimeoutValue[4]             = 0.25;
	stateScript[4]                  = "onZappedA";
	stateEmitter[4]			= electrocuteEmitter;
	stateEmitterTime[4]		= 0.2;
	stateTransitionOnTimeout[4]       = "ZappedF";

	stateName[5]                     = "ZappedF";
	stateTimeoutValue[5]             = 0.25;
	stateScript[5]                  = "onZappedB";
	stateEmitter[5]			= electrocuteEmitter;
	stateEmitterTime[5]		= 0.2;
	stateTransitionOnTimeout[5]       = "ZappedG";

	stateName[6]                     = "ZappedG";
	stateTimeoutValue[6]             = 0.25;
	stateScript[6]                  = "onDone";
	stateEmitter[6]			= electrocuteEmitter;
	stateEmitterTime[6]		= 0.2;
	stateTransitionOnTimeout[6]       = "ZappedB";
};

function electroZapImage::onMount(%this, %obj, %slot)
{
	if(%obj.zapTicks $= "" || %obj.zapTicks <= 0)
	{
		%obj.zapTicks = 1;
	}

	%obj.setMoveFactor(0.25);
	%obj.extraDamageFactor = 0.5;

	//disable recharge for 12 seconds
	for (%i = 0; %i < %obj.getDatablock().maxTools; %i++)
	{
		%obj.nextChargeTime[%i] = getSimTime() + 12000 | 0;
	}
}

function electroZapImage::onUnmount(%this, %obj, %slot)
{
	%obj.zapTicks = 0;
	%obj.setMoveFactor(1);
	%obj.extraDamageFactor = 0;
}

function electroZapImage::onZappedA(%this, %obj, %slot)
{
	zapTarget(%obj, "1 1 1 1");
}

function electroZapImage::onZappedB(%this, %obj, %slot)
{
	zapTarget(%obj, "0 0 0 1");
}

function electroZapImage::onDone(%this, %obj, %slot)
{
	if((%obj.zapTicks--) > 0)
	{
		zapTarget(%obj, "0 0 0 1");
		return;
	}

	%obj.playThread(2, Plant);
	if (isObject(radioWaveProjectile))
	{
		%obj.spawnExplosion(radioWaveProjectile, %obj.getScale() * 2);
	}

	if(isObject(%obj.client))
	{
		%obj.client.applyBodyParts();
		%obj.client.applyBodyColors();
	}
	else
	{
		GameConnection::applyBodyParts(%obj);
		GameConnection::applyBodyColors(%obj);
	}
	%obj.unMountImage(%slot);
}

function zapTarget(%obj, %color)
{
	%obj.setMoveFactor(0.25);
	%obj.setNodeColor("ALL", %color);
	%obj.playThread(2, jump);
	%obj.setWhiteout(0.5);
	if (isObject(radioWaveProjectile))
	{
		%obj.spawnExplosion(radioWaveProjectile, %obj.getScale() * 2);
	}
	for (%i = 0; %i < %obj.getDatablock().maxTools; %i++)
	{
		%obj.weaponCharge[%i] = getMax(%obj.weaponCharge[%i] - 8, 0);
	}
}