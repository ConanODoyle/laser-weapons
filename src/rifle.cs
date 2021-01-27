//rifle
datablock ProjectileData(ChargeLaserFALProjectile : ChargeLaserPistolProjectile)
{
	directDamage        = 40;
	directDamageType    = $DamageType::DMR;
	radiusDamageType    = $DamageType::DMR;

	muzzleVelocity = 150;
	particleEmitter     = ChargeLaserRifleTracer;
};

datablock ItemData(ChargeFALItem : ChargePistolItem)
{
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_RIFLE_DMR_Charge.dts";

	uiName = "Charge Leech R.";
	iconName = "Add-ons/Gamemode_Core_Rush/src/img/Icon_mid-range";
	doColorShift = true;
	colorShiftColor = "1 0 0 1";

	image = ChargeFALImage;
};

datablock ShapeBaseImageData(ChargeFALImage : SimpleChargeImageFramework_SemiAuto)
{
   // Basic Item properties
	shapeFile = "Add-ons/Gamemode_Core_Rush/src/weps/res/LEECH_RIFLE_DMR_Charge.dts";
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

	item = ChargeFALItem;
	ammo = " ";
	projectile = ChargeLaserFALProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = ChargeFALItem.colorShiftColor;

	// Weapon properties
	maxCharge = 120; //clip
	chargeRate = 4; //how fast to reload
	chargeTickTime = 50; //time between charge ticks, in milliseconds
	discharge = 15; //fire cost
	chargeDisableTime = 1500; //time between firing and charging resuming
	spread = 0.00001; //larger = more spread
	shellCount = 1; //projectiles per fire state

	stateTimeoutValue[4] = 0.1; //fire state timeout override
	stateTimeoutValue[5] = 0.4; //smoke state timeout override
	
	stateSound[4]			= ChargeLaserRifleBlastSound;
	stateEmitter[5]			= LaserSmokeEmitter;
	stateEmitterTime[5]		= 0.4;
};

function ChargeFALImage::onFire(%this, %obj, %slot)
{
	SimpleChargeImage::onFire(%this, %obj, %slot);
}