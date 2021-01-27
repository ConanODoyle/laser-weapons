//common datablocks
datablock ExplosionData(redLaserExplosion)
{
	//explosionShape = "";
	soundProfile = laserHit_Sound;

	lifeTimeMS = 400;

	particleEmitter = redLaserExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.5;

	emitter[0] = redLaserExplosionRingEmitter;

	faceViewer = true;
	explosionScale = "1 1 1";

	shakeCamera = false;
	camShakeFreq = "10.0 11.0 10.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	// Dynamic light
	lightStartRadius = 10;
	lightEndRadius = 8;
	lightStartColor = "0 0 1";
	lightEndColor = "0 0 0";
};




//handheld
datablock ProjectileData(redLaserHandheldProjectile)
{
	directDamage        = 15;
	directDamageType    = $DamageType::Pistol;
	radiusDamageType    = $DamageType::Pistol;

	brickExplosionRadius = 0;
	brickExplosionImpact = true; 
	brickExplosionForce  = 5;
	brickExplosionMaxVolume = 15;
	brickExplosionMaxVolumeFloating = 20;  

	impactImpulse	     = 0;
	verticalImpulse	  = 3;
	explosion           = redLaserExplosion;
	particleEmitter     = redLaserTracer;

	muzzleVelocity      = 100;
	velInheritFactor    = 1;

	armingDelay         = 00;
	lifetime            = 30000;
	fadeDelay           = 29500;
	bounceElasticity    = 0.99;
	bounceFriction      = 0.00;
	isBallistic         = true;
	gravityMod          = 0;

	hasLight    = true;
	lightRadius = 7.0;
	lightColor  = "1 0 0";
};

datablock ItemData(redHandheldItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/Leech_Burst_red.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Red Handheld";
	iconName = "Add-ons/Gamemode_Core_Rush/src/img/Icon_pistol";
	doColorShift = true;
	colorShiftColor = "1 0 0 1";

	image = redHandheldImage;
	canDrop = true;
};

datablock ShapeBaseImageData(redHandheldImage : SimpleChargeImageFramework_SemiAuto)
{
	// Basic Item properties
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/Leech_Burst_red.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = ""; 
	correctMuzzleVector = true;
	className = "WeaponImage";

	item = redHandheldItem;
	projectile = redLaserHandheldProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = redHandheldItem.colorShiftColor;

	// Weapon properties
	maxCharge = 60; //clip
	chargeRate = 3; //how fast to reload
	chargeTickTime = 50; //time between charge ticks, in milliseconds
	discharge = 12; //fire cost
	chargeDisableTime = 2000; //time between firing and charging resuming
	spread = 0.0002; //larger = more spread
	shellCount = 1; //projectiles per fire state

	stateTimeoutValue[1] = 0.06; //reload check state timeout override;
	stateTimeoutValue[4] = 0.01; //fire state timeout override
	stateSound[4]		 = LaserHHFire_Sound;
	stateTimeoutValue[5] = 0.01; //smoke state timeout override
};

function redHandheldImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}




//smg
datablock ProjectileData(redLaserSMGProjectile : redLaserHandheldProjectile)
{
	directDamage        = 8;
	directDamageType    = $DamageType::SMG;
	radiusDamageType    = $DamageType::SMG;
};

datablock ItemData(redSMGItem : redHandheldItem)
{
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_REPEATER_red.dts";

	//gui stuff
	uiName = "Red Leech Repeater";
	iconName = "Add-ons/Gamemode_Core_Rush/src/img/Icon_smg";
	doColorShift = true;
	colorShiftColor = "1 0 0 1";

	// Dynamic properties defined by the scripts
	image = redSMGImage;
};

datablock ShapeBaseImageData(redSMGImage : SimpleChargeImageFramework_Auto)
{

	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_REPEATER_red.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = "";

	correctMuzzleVector = true;
	className = "WeaponImage";

	item = redSMGItem;
	projectile = redLaserSMGProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = redSMGItem.colorShiftColor;

	// Weapon properties
	maxCharge = 120; //clip
	chargeRate = 2; //how fast to reload
	chargeTickTime = 65; //time between charge ticks, in milliseconds
	discharge = 6; //fire cost
	chargeDisableTime = 2200; //time between firing and charging resuming
	spread = 0.005; //larger = more spread
	shellCount = 1; //projectiles per fire state

	stateTimeoutValue[1] = 0.06; //reload check state timeout override;
	stateTimeoutValue[4] = 0.01; //fire state timeout override
	stateSound[4]		 = LaserSMGFire_Sound;
	stateTimeoutValue[5] = 0.013; //smoke state timeout override
};

function redSMGImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
	cancel(%obj.releaseSoundSchedule);
	%obj.releaseSoundSchedule = schedule(200, %obj, playSMGReleaseSound, %obj);
}



//shotgun
datablock ProjectileData(redShotgunLaserProjectile : redLaserHandheldProjectile)
{
	directDamage        = 17;
	directDamageType    = $DamageType::shotgun;
	radiusDamageType    = $DamageType::shotgun;

	particleEmitter     = redCasterTracer;

	muzzleVelocity      = 65;
	velInheritFactor    = 1;

	armingDelay         = 1000;
	lifetime            = 1000;
	fadeDelay           = 2000;
	bounceElasticity    = 0.99;
	bounceFriction      = 0.20;
	gravityMod = 1.0;

	lightRadius = 8.0;
};

datablock ItemData(redShotgunItem : redHandheldItem)
{
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_BLAST_Shotgun_red.dts";

	uiName = "Red Leech Blast";
	iconName = "Add-ons/Gamemode_Core_Rush/src/img/Icon_shotgun";
	doColorShift = true;
	colorShiftColor = "1 0 0 1";

	// Dynamic properties defined by the scripts
	image = redShotgunImage;
};

datablock ShapeBaseImageData(redShotgunImage : SimpleChargeImageFramework_SemiAuto)
{
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_BLAST_Shotgun_red.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0; //"1.0 -10.0 0"
	rotation = ""; //eulerToMatrix( "0 0 0" )

	correctMuzzleVector = true;

	className = "WeaponImage";
	item = redShotgunItem;
	ammo = " ";
	projectile = redShotgunLaserProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = redShotgunItem.colorShiftColor;

	// Weapon properties
	maxCharge = 150; //clip
	chargeRate = 5; //how fast to reload
	chargeTickTime = 100; //time between charge ticks, in milliseconds
	discharge = 50; //fire cost
	chargeDisableTime = 3000; //time between firing and charging resuming
	spread = 0.015; //larger = more spread
	shellCount = 7; //projectiles per fire state

	stateTimeoutValue[4] = 0.09; //fire state timeout override
	stateSound[4]		 = LaserShotgunFire_Sound;
	stateTimeoutValue[5] = 0.45; //smoke state timeout override
	stateEmitterTime[5] = 0.45; //smoke state emitter time override
};

function redShotgunImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}




//rifle
datablock ProjectileData(redLaserFALProjectile : redLaserHandheldProjectile)
{
	directDamage        = 40;
	directDamageType    = $DamageType::DMR;
	radiusDamageType    = $DamageType::DMR;

	muzzleVelocity = 150;
	particleEmitter     = RedLaserRifleTracer;
};

datablock ItemData(redFALItem : redHandheldItem)
{
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_RIFLE_DMR_red.dts";

	uiName = "Red Leech R.";
	iconName = "Add-ons/Gamemode_Core_Rush/src/img/Icon_mid-range";
	doColorShift = true;
	colorShiftColor = "1 0 0 1";

	image = redFALImage;
};

datablock ShapeBaseImageData(redFALImage : SimpleChargeImageFramework_SemiAuto)
{
   // Basic Item properties
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_RIFLE_DMR_red.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = ""; 
	correctMuzzleVector = true;
	className = "WeaponImage";

	////Ammo
	maxCharge = 100; //clip
	chargeRate = 30; //how fast to reload
	discharge = 30; //cost

	item = redFALItem;
	ammo = " ";
	projectile = redLaserFALProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = redFALItem.colorShiftColor;

	// Weapon properties
	maxCharge = 120; //clip
	chargeRate = 4; //how fast to reload
	chargeTickTime = 50; //time between charge ticks, in milliseconds
	discharge = 15; //fire cost
	chargeDisableTime = 3000; //time between firing and charging resuming
	spread = 0.00001; //larger = more spread
	shellCount = 1; //projectiles per fire state

	stateTimeoutValue[4] = 0.1; //fire state timeout override
	stateSound[4]		 = LaserFALFire_Sound;
	stateTimeoutValue[5] = 0.4; //smoke state timeout override
	stateEmitterTime[5] = 0.4; //smoke state emitter time override
};

function redFALImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}