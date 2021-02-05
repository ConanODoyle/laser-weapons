datablock ShapeBaseImageData(droneBurstGunImage : SimpleChargeImageFramework_Auto)
{
	shapeFile = "./resources/droneBurstGun.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = "";

	correctMuzzleVector = true;
	className = "WeaponImage";

	projectile = ChargeLaserSMGProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	maxCharge = 10000; //clip
	chargeRate = 10000; //how fast to reload
	chargeTickTime = 100; //time between charge ticks, in milliseconds
	discharge = 0; //fire cost
	chargeDisableTime = 1000; //time between firing and charging resuming
	spread = 0.0012; //larger = more spread
	shellCount = 1; //projectiles per fire state
	
	markerLightSupport = 1;
	markerLightMaxRange = 64;
	markerLightSpread = 0.0012; //defaults to .spread, defined above
	markerLightAngleIgnore = 3.1415 * 2;

	stateSound[4]						= ChargeLaserQuadshotSound;
	stateTimeoutValue[5]				= 0.9;
	stateEmitter[5]						= LaserSmokeEmitter;
	stateEmitterTime[5]					= 0.7;
};
datablock ShapeBaseImageData(droneBurstGunImageLeft : droneBurstGunImage) { mountPoint = 1; };

function droneBurstGunImage::onFire(%this, %obj, %slot) { drone_SMGSubFire(%this, %obj, %slot, 0, 0); }
function droneBurstGunImageLeft::onFire(%this, %obj, %slot) { drone_SMGSubFire(%this, %obj, %slot, 0, 1); }

function drone_SMGSubFire(%this, %obj, %slot, %count, %left)
{
	if (%obj.getDamageState() !$= "Enabled" || %count >= 4 || %obj.getMountedImage(%slot) != %this)
	{
		return;
	}

	if (!%left)
	{
		%obj.playThread(0, shiftAway);
	}
	else
	{
		%obj.playThread(1, leftRecoil);
	}

	SimpleChargeImage::onFire(%this, %obj, %slot);
	cancel(%obj.subFireSchedule[%left]);
	%obj.subFireSchedule[%left] = schedule(80, %obj, drone_SMGSubFire, %this, %obj, %slot, %count + 1, %left);
}









datablock ShapeBaseImageData(droneRifleGunImage : SimpleChargeImageFramework_Auto)
{
	shapeFile = "./resources/droneRifleGun.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = "";

	correctMuzzleVector = true;
	className = "WeaponImage";

	projectile = ChargeLaserRifleProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	maxCharge = 10000; //clip
	chargeRate = 10000; //how fast to reload
	chargeTickTime = 100; //time between charge ticks, in milliseconds
	discharge = 0; //fire cost
	chargeDisableTime = 1000; //time between firing and charging resuming
	spread = 0.0002; //larger = more spread
	shellCount = 1; //projectiles per fire state
	
	markerLightSupport = 1;
	markerLightMaxRange = 256;
	markerLightSpread = 0.0012; //defaults to .spread, defined above
	markerLightAngleIgnore = 3.1415 * 2;

	stateSound[4]						= ChargeLaserRifleBlastSound;
	stateTimeoutValue[5]				= 0.9;
	stateEmitter[5]						= LaserSmokeEmitter;
	stateEmitterTime[5]					= 0.7;
};
datablock ShapeBaseImageData(droneRifleGunImageLeft : droneRifleGunImage) { mountPoint = 1; };

function droneRifleGunImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(0, shiftAway);
	SimpleChargeImage::onFire(%this, %obj, %slot);
}
function droneRifleGunImageLeft::onFire(%this, %obj, %slot)
{
	%obj.playThread(0, leftRecoil);
	SimpleChargeImage::onFire(%this, %obj, %slot);
}
