// AddDamageType("Pistol",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_pist> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_pist> %1',0.2,1);
// AddDamageType("SMG",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_smg> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_smg> %1',0.2,1);
// AddDamageType("Shotgun",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_sg> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_sg> %1',0.2,1);
// AddDamageType("DMR",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_dmr> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_dmr> %1',0.2,1);

//common datablocks
datablock ExplosionData(ChargeLaserExplosion)
{
	//explosionShape = "";
	soundProfile = ChargeLaserHitSound;

	lifeTimeMS = 400;

	particleEmitter = ChargeLaserExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.5;

	emitter[0] = ChargeLaserExplosionRingEmitter;

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

exec("./pistol.cs");
exec("./rifle.cs");
exec("./smg.cs");