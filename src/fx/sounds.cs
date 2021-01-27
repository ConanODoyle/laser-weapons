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

datablock AudioProfile(ChargeLaserBlast1Sound)
{
	filename = "./resources/laserblast1.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ChargeLaserRifleBlast1Sound)
{
	filename = "./resources/laserifleblast1.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ChargeLaserQuadshotSound)
{
	filename = "./resources/laserquadshot.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ChargeLaserHitSound)
{
	filename = "./resources/laserHit.wav";
	description = AudioClose3d;
	preload = true;
};