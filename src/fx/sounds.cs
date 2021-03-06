%error = ForceRequiredAddOn("Projectile_Radio_Wave");

if (!isObject(radioWaveExplosionSound))
{
	datablock AudioProfile(radioWaveExplosionSound)
	{
	   filename    = "./resources/radioWaveExplosion.wav";
	   description = AudioClosest3d;
	   preload = true;
	};
}

datablock AudioProfile(ChargeLaserBlastSound)
{
	filename = "./resources/laserblast1.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ChargeLaserRifleBlastSound)
{
	filename = "./resources/laserrifleblast1.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ChargeLaserQuadshotSound)
{
	filename = "./resources/quadshot.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ChargeLaserHitSound)
{
	filename = "./resources/laserHit.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(MarkerlightShotSound)
{
	filename = "./resources/markerlightshot.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(MarkerlightCycleSound)
{
	filename = "./resources/boltcycle.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(empGrenadeBounceSound)
{
	filename    = "./resources/bounce1.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(empGrenadeExplosionSound)
{
	filename    = "./resources/blast_electric.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(droneTargetFoundSound)
{
	filename    = "./resources/sentryalert.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(droneIdleSound)
{
	filename    = "./resources/sentryLoop.wav";
	description = AudioDefault3D;
	preload = true;
};