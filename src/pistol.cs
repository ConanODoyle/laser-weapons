//Pistol
datablock ProjectileData(ChargeLaserPistolProjectile)
{
	directDamage        = 14;
	directDamageType    = $DamageType::Pistol;
	radiusDamageType    = $DamageType::Pistol;

	brickExplosionRadius = 0;
	brickExplosionImpact = true;
	brickExplosionForce  = 0;
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	impactImpulse	     = 0;
	verticalImpulse	  = 3;
	explosion           = ChargeLaserExplosion;
	particleEmitter     = ChargeLaserTracer;

	muzzleVelocity      = 100;
	velInheritFactor    = 0;

	armingDelay         = 00;
	lifetime            = 30000;
	fadeDelay           = 29500;
	bounceElasticity    = 0.99;
	bounceFriction      = 0.00;
	isBallistic         = true;
	gravityMod          = 0;

	hasLight    = true;
	lightRadius = 2.0;
	lightColor  = "1 0 0";
};

datablock ItemData(ChargePistolItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./resources/pistol.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Charge Pistol";
	iconName = "Add-ons/Gamemode_Core_Rush/src/img/Icon_pistol";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	image = ChargePistolImage;
	canDrop = true;
};

datablock ShapeBaseImageData(ChargePistolImage : SimpleChargeImageFramework_SemiAuto)
{
	// Basic Item properties
	shapeFile = "./resources/pistol.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = ""; 
	correctMuzzleVector = true;
	className = "WeaponImage";

	item = ChargePistolItem;
	projectile = ChargeLaserPistolProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = ChargePistolItem.colorShiftColor;

	// Weapon properties
	maxCharge = 60; //clip
	chargeRate = 6; //how fast to reload
	chargeTickTime = 50; //time between charge ticks, in milliseconds
	discharge = 10; //fire cost
	chargeDisableTime = 800; //time between firing and charging resuming
	spread = 0.001; //larger = more spread
	shellCount = 1; //projectiles per fire state

	stateTimeoutValue[1] = 0.08; //reload check state timeout override;
	stateTimeoutValue[4] = 0.02; //fire state timeout override
	stateTimeoutValue[5] = 0.05; //smoke state timeout override

	stateSound[4]			= ChargeLaserBlastSound;
	stateEmitter[5]			= LaserSmokeEmitter;
	stateEmitterTime[5]		= 0.20;
};

function ChargePistolImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}

function ChargePistolImage::onUnmount(%this, %obj, %slot)
{
	if (isObject(%obj.client))
	{
		%obj.client.bottomprint("", 1, 1);
		%obj.client.centerprint("", 1);
	}
}