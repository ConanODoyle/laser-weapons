datablock ExplosionData(markerlightExplosion)
{
	//explosionShape = "";
	soundProfile = radioWaveExplosionSound;

	lifeTimeMS = 150;

	particleEmitter = "markerlightExplosionEmitter";
	particleDensity = 10;
	particleRadius = 0;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = false;
	camShakeFreq = "10.0 11.0 10.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	// Dynamic light
	lightStartRadius = 1;
	lightEndRadius = 0;
	lightStartColor = "0.5 1 1";
	lightEndColor = "0 0 0";
};

datablock ProjectileData(MarkerlightRifleProjectile : ChargeLaserPistolProjectile)
{
	directDamage        = 2;
	directDamageType    = $DamageType::DMR;
	radiusDamageType    = $DamageType::DMR;

	particleEmitter = markerlightTrailEmitter;
	explosion = markerlightExplosion;
	projectileShapeName = "./resources/markerlight.dts";
	markerlightTime = 12000;

	muzzleVelocity = 120;
	gravityMod = 0.5;
};

datablock ItemData(MarkerlightRifleItem : ChargePistolItem)
{
	shapeFile = "./resources/markerlightrifle.dts";

	uiName = "Markerlight Rifle";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	image = MarkerlightRifleImage;
};

datablock ShapeBaseImageData(MarkerlightRifleImage)
{
	shapeFile = "./resources/markerlightrifle.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = ""; 
	correctMuzzleVector = true;
	className = "WeaponImage";

	item = MarkerlightRifleItem;
	ammo = " ";
	projectile = MarkerlightRifleProjectile;
	projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 0 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	melee = false;

	armReady = true;

	doColorShift = true;
	colorShiftColor = MarkerlightRifleItem.colorShiftColor;

	// Weapon properties
	maxCharge = 60; //clip
	chargeRate = 2; //how fast to reload
	chargeTickTime = 50; //time between charge ticks, in milliseconds
	discharge = 5; //fire cost
	chargeDisableTime = 2000; //time between firing and charging resuming
	spread = 0.001; //larger = more spread
	shellCount = 1; //projectiles per fire state

	stateName[0]						= "Activate";
	stateTimeoutValue[0]				= 0.3;
	stateTransitionOnTimeout[0]			= "AmmoCheck";
	stateSound[0]						= weaponSwitchSound;
	stateSequence[0]					= "equip";

	stateName[1]						= "AmmoCheck";
	stateTimeoutValue[1]				= 0.15;
	stateScript[1]						= "checkAmmo";
	stateTransitionOnTimeout[1]			= "AmmoConfirm";

	stateName[2]						= "AmmoConfirm";
	stateTransitionOnAmmo[2]			= "Ready";
	stateTransitionOnNoAmmo[2]			= "AmmoCheck";

	stateName[3]						= "Ready";
	stateTransitionOnTriggerDown[3]		= "Fire";
	stateWaitForTimeout[3]				= false;
	stateSequence[3]					= "Ready";
	stateScript[3]						= "checkAmmo";
	stateTimeoutValue[3]				= 0.05;
	stateTransitionOnTimeout[3]			= "Ready2";

	stateName[4]						= "Fire";
	stateTransitionOnTimeout[4]			= "Fire2";
	stateTimeoutValue[4]				= 0.1;
	stateFire[4]						= true;
	stateAllowImageChange[4]			= false;
	// stateSound[4]						= MarkerlightShotSound;
	stateSequence[4]					= "Fire2";
	stateScript[4]						= "onFire";
	stateTransitionOnNoAmmo[4]			= "AmmoCheck";
	stateSequence[4]					= "cycleBolt";

	stateName[7]						= "Fire2";
	stateTransitionOnTimeout[7]			= "Smoke";
	stateTimeoutValue[7]				= 0.1;
	stateFire[7]						= true;
	stateAllowImageChange[7]			= false;
	// stateSound[7]						= MarkerlightShotSound;
	stateSequence[7]					= "Fire2";
	stateScript[7]						= "onFire";
	stateSequence[7]					= "cycleBolt";

	stateName[5]						= "Smoke";
	stateTimeoutValue[5]				= 0.3;
	stateWaitForTimeout[5]				= true;
	stateTransitionOnTimeout[5]			= "AmmoCheck";
	// stateSound[5]						= MarkerlightCycleSound;
	stateEmitter[5]						= LaserSmokeEmitter;
	stateEmitterTime[5]					= 0.2;
	// stateSequence[5]					= "cycleBolt";

	stateName[6]						= "Ready2";
	stateTransitionOnTriggerDown[6]		= "Fire";
	stateWaitForTimeout[6]				= false;
	stateSequence[6]					= "Ready";
	stateScript[6]						= "checkAmmo";
	stateTimeoutValue[6]				= 0.05;
	stateTransitionOnTimeout[6]			= "Ready";
};

function MarkerlightRifleImage::onFire(%this, %obj, %slot)
{
	serverPlay3D(MarkerlightShotSound, %obj.getMuzzlePoint(%slot));
	SimpleChargeImage::onFire(%this, %obj, %slot);
}

function MarkerlightRifleImage::onMount(%this, %obj, %slot)
{
	%obj.equipSoundSchedule = %obj.schedule(250, playAudio, 0, brickPlantSound);
}

function MarkerlightRifleImage::onUnmount(%this, %obj, %slot)
{
	cancel(%obj.equipSoundSchedule);
	if (isObject(%obj.client))
	{
		%obj.client.bottomprint("", 1, 1);
		%obj.client.centerprint("", 1);
	}
}

function MarkerlightRifleProjectile::Damage(%this, %obj, %col, %fade, %pos, %normal)
{
	if (%col.getType() & $TypeMasks::PlayerObjectType)
	{
		%col.attachMarkerlight(%this.markerlightTime);
		%col.setWhiteout(0.2);
	}
	parent::Damage(%this, %obj, %col, %fade, %pos, %normal);
}