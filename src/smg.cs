//smg
datablock ProjectileData(ChargeLaserSMGProjectile : ChargeLaserPistolProjectile)
{
	directDamage        = 8;
	directDamageType    = $DamageType::SMG;
	radiusDamageType    = $DamageType::SMG;
};

datablock ItemData(ChargeSMGItem : ChargePistolItem)
{
	shapeFile = "./resources/smg.dts";

	//gui stuff
	uiName = "Charge SMG";
	iconName = "Add-ons/Gamemode_Core_Rush/src/img/Icon_smg";
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
	spread = 0.0022; //larger = more spread
	shellCount = 1; //projectiles per fire state

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
	cancel(%obj.releaseSoundSchedule);
	%obj.releaseSoundSchedule = schedule(200, %obj, playSMGReleaseSound, %obj);
}