//smg
datablock ProjectileData(ChargeLaserSMGProjectile : ChargeLaserPistolProjectile)
{
	directDamage        = 10;
	directDamageType    = $DamageType::LaserSMG;
	radiusDamageType    = $DamageType::LaserSMG;
};

datablock ItemData(ChargeSMGItem : ChargePistolItem)
{
	shapeFile = "./resources/smg.dts";

	//gui stuff
	uiName = "Charge SMG";
	iconName = "./resources/icon_smg";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	// Dynamic properties defined by the scripts
	image = ChargeSMGImage;
};

datablock ShapeBaseImageData(ChargeSMGImage : SimpleChargeImageFramework_Auto)
{
	shapeFile = "./resources/smg.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = "";

	correctMuzzleVector = true;
	className = "WeaponImage";

	item = ChargeSMGItem;
	projectile = ChargeLaserSMGProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = ChargeSMGItem.colorShiftColor;

	// Weapon properties
	maxCharge = 240; //clip
	chargeRate = 6; //how fast to reload
	chargeTickTime = 85; //time between charge ticks, in milliseconds
	discharge = 10; //fire cost
	chargeDisableTime = 1800; //time between firing and charging resuming
	spread = 0.002; //larger = more spread
	shellCount = 1; //projectiles per fire state
	
	markerLightSupport = 1;
	markerLightMaxRange = 64;
	markerLightSpread = 0.0015; //defaults to .spread, defined above

	stateTimeoutValue[1] = 0.06; //reload check state timeout override;
	stateTimeoutValue[4] = 0.01; //fire state timeout override
	stateTimeoutValue[5] = 0.013; //smoke state timeout override

	stateSound[4]			= ChargeLaserBlastSound;
	stateEmitter[5]			= LaserSmokeEmitter;
	stateEmitterTime[5]		= 0.20;
};

function ChargeSMGImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}

function ChargeSMGImage::onUnmount(%this, %obj, %slot)
{
	if (isObject(%obj.client))
	{
		%obj.client.bottomprint("", 1, 1);
		%obj.client.centerprint("", 1);
	}
}










datablock ProjectileData(ChargeLaserSMGProjectileBlue : ChargeLaserPistolProjectileBlue)
{
	directDamage        = 10;
	directDamageType    = $DamageType::LaserSMG;
	radiusDamageType    = $DamageType::LaserSMG;
};

datablock ItemData(ChargeSMGItemBlue : ChargeSMGItem)
{
	shapeFile = "./resources/smgBlue.dts";
	uiName = "Charge SMG (B)";
	iconName = "./resources/icon_smgBlue";
	image = ChargeSMGImageBlue;
};

datablock ShapeBaseImageData(ChargeSMGImageBlue : ChargeSMGImage)
{
	// Basic Item properties
	shapeFile = "./resources/smgBlue.dts";
	item = ChargeSMGItemBlue;
	projectile = ChargeLaserSMGProjectileBlue;
	colorShiftColor = ChargeSMGItemBlue.colorShiftColor;
};

function ChargeSMGImageBlue::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}

function ChargeSMGImageBlue::onUnmount(%this, %obj, %slot)
{
	if (isObject(%obj.client))
	{
		%obj.client.bottomprint("", 1, 1);
		%obj.client.centerprint("", 1);
	}
}