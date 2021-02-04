//rifle
datablock ProjectileData(ChargeLaserRifleProjectile : ChargeLaserPistolProjectile)
{
	directDamage        = 40;
	directDamageType    = $DamageType::DMR;
	radiusDamageType    = $DamageType::DMR;

	muzzleVelocity = 200;
	particleEmitter     = HeavyChargeLaserTracer;
};

datablock ItemData(ChargeRifleItem : ChargePistolItem)
{
	shapeFile = "./resources/rifle.dts";

	uiName = "Charge Rifle";
	iconName = "./resources/icon_rifle";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	image = ChargeRifleImage;
};

datablock ShapeBaseImageData(ChargeRifleImage : SimpleChargeImageFramework_SemiAuto)
{
	shapeFile = "./resources/rifle.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = ""; 
	correctMuzzleVector = true;
	className = "WeaponImage";

	item = ChargeRifleItem;
	ammo = " ";
	projectile = ChargeLaserRifleProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = ChargeRifleItem.colorShiftColor;

	// Weapon properties
	maxCharge = 100; //clip
	chargeRate = 2; //how fast to reload
	chargeTickTime = 85; //time between charge ticks, in milliseconds
	discharge = 10; //fire cost
	chargeDisableTime = 2000; //time between firing and charging resuming
	spread = 0.0005; //larger = more spread
	shellCount = 1; //projectiles per fire state

	markerLightSupport = 1;
	markerLightMaxRange = 256;
	markerLightMaxAngle = 3.14159 / 16;
	markerLightSpread = 0.0002; //defaults to .spread, defined above

	stateTimeoutValue[4] = 0.1; //fire state timeout override
	stateTimeoutValue[5] = 0.5; //smoke state timeout override
	
	stateSound[4]			= ChargeLaserRifleBlastSound;
	stateEmitter[5]			= LaserSmokeEmitter;
	stateEmitterTime[5]		= 0.5;
};

function ChargeRifleImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}

function ChargeRifleImage::onUnmount(%this, %obj, %slot)
{
	if (isObject(%obj.client))
	{
		%obj.client.bottomprint("", 1, 1);
		%obj.client.centerprint("", 1);
	}
}