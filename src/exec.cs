// AddDamageType("Pistol",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_pist> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_pist> %1',0.2,1);
// AddDamageType("SMG",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_smg> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_smg> %1',0.2,1);
// AddDamageType("Shotgun",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_sg> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_sg> %1',0.2,1);
// AddDamageType("DMR",   '<bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_dmr> %1',    '%2 <bitmap:Add-ons/Gamemode_Core_Rush/src/img/ci_dmr> %1',0.2,1);

//common datablocks
exec("./fx/emitters.cs");
exec("./fx/sounds.cs");
exec("./fx/shapes.cs");
exec("./fx/Effect_StunZap.cs");

datablock ExplosionData(BigRecoilExplosion)
{
   	explosionShape = "";

   	lifeTimeMS = 150;

   	faceViewer	 = true;
   	explosionScale = "1 1 1";

   	shakeCamera = true;
  	camShakeFreq = "5 5 5";
  	camShakeAmp = "0.3 0.4 0.35";
	camShakeDuration = 1.5;
	camShakeRadius = 10.0;

	lightstartradius = 2;
	lightEndRadius = 2;
	lightstartColor = "1.0 0.7 0.7";
	lightEndColor = "0 0 0";
};

datablock ProjectileData(BigRecoilProjectile)
{
	lifetime						= 10;
	fadeDelay						= 10;
	explodeondeath						= true;
	explosion						= BigRecoilExplosion;
};

datablock ExplosionData(ChargeLaserExplosion)
{
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
	lightStartRadius = 4;
	lightEndRadius = 2;
	lightStartColor = "1 0 0";
	lightEndColor = "0 0 0";
};

exec("./pistol.cs");
exec("./rifle.cs");
exec("./smg.cs");
exec("./markerlight.cs");
exec("./empgrenade.cs");
exec("./drone.cs");
exec("./droneGuns.cs");
exec("./droneDeploy.cs");