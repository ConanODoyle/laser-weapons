//laser trails
datablock ParticleData(ChargeLaserTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.1;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 600;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/dot";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]	= "1 0 0 1";
	colors[1]	= "1 0.6 0.6 0.5";
	colors[2]	= "1 0 0 0";
	sizes[0]	= 0.5;
	sizes[1]	= 0.15;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1] = 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(ChargeLaserTracer)
{
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = ChargeLaserTrailParticle;

	useEmitterColors = true;
};

datablock ParticleData(HeavyChargeLaserTrailParticle : ChargeLaserTrailParticle)
{
	lifetimeMS     = 1200;
	sizes[1] = 0.25;
	times[1] = 0.5;
};

datablock ParticleEmitterData(HeavyChargeLaserTracer : ChargeLaserTracer)
{
	particles = HeavyChargeLaserTrailParticle;
};



//laser fire smoke
datablock ParticleData(LaserSmokeParticle)
{
	dragCoefficient	  = 3;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0;
	constantAcceleration = 0.0;
	lifetimeMS			= 100;
	lifetimeVarianceMS	= 0;
	textureName		  = "base/data/particles/cloud";
	spinSpeed		= 9000.0;
	spinRandomMin		= -5000.0;
	spinRandomMax		= 5000.0;

	colors[0]	 = "1 1 1 0";
	colors[1]	 = "1 1 1 0.1";
	colors[2]	 = "1 1 1 0.1";
	colors[3]	 = "1 1 1 0";

	sizes[0]	  = 0.0;
	sizes[1]	  = 0.4;
	sizes[2]	  = 0.1;
	sizes[3]	  = 0.05;

	times[0] = 0.0;
	times[1] = 0.1;
	times[2] = 0.5;
	times[3] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(LaserSmokeEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 64.0;
	velocityVariance = 0.0;
	ejectionOffset	= 0.0;
	thetaMin		 = 0;
	thetaMax		 = 1;
	phiReferenceVel  = 0;
	phiVariance	  = 360;
	overridewarance = false;

	particles = "LaserSmokeParticle";
};



//laser explosion
datablock ParticleData(ChargeLaserExplosionParticle)
{
	dragCoefficient      = 0;
	gravityCoefficient   = 0;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0;
	lifetimeMS           = 100;
	lifetimeVarianceMS   = 50;
	textureName          = "base/data/particles/dot";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "1 0 0 1";
	colors[1]     = "1 0.3 0.1 0";
	sizes[0]      = 0.18;
	sizes[1]      = 0.09;

	useInvAlpha = false;
};

datablock ParticleEmitterData(ChargeLaserExplosionEmitter)
{
	ejectionPeriodMS = 50;
	periodVarianceMS = 30;
	ejectionVelocity = 2;
	velocityVariance = 1.0;
	ejectionOffset   = 0;
	thetaMin         = 50;
	thetaMax         = 60;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "ChargeLaserExplosionParticle";

	useEmitterColors = true;
	uiName = "ChargeLaserExplosion";
};

datablock ParticleData(ChargeLaserExplosionRingParticle)
{
	dragCoefficient      = 8;
	gravityCoefficient   = 0;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 50;
	lifetimeVarianceMS   = 35;
	textureName          = "base/data/particles/ring";
	spinSpeed		= 500.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "1 0 0 1";
	colors[1]     = "1 0 0 1";
	sizes[0]      = 1;
	sizes[1]      = 0;

	useInvAlpha = false;
};

datablock ParticleEmitterData(ChargeLaserExplosionRingEmitter)
{
	lifeTimeMS = 50;

	ejectionPeriodMS = 25;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.0;
	thetaMin         = 89;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "ChargeLaserExplosionRingParticle";

	useEmitterColors = true;
	uiName = "ChargeLaserExplosionRing";
};



//emp grenade explosion
datablock ParticleData(empGrenadeExplosionDebris2Particle)
{
	dragCoefficient		= 1.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.4;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 2000;
	lifetimeVarianceMS	= 1990;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	
	textureName		= "base/data/particles/dot";

	colors[0]     = "1 0.5 0.0 1";
	colors[1]     = "0.9 0.5 0.0 0.9";
	colors[2]     = "1 1 1 0.0";

	sizes[0]	= 0.3;
	sizes[1]	= 0.3;
	sizes[2]	= 0.3;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(empGrenadeExplosionDebris2Emitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	lifetimeMS       = 140;
	ejectionVelocity = 15;
	velocityVariance = 12.0;
	ejectionOffset   = 1.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "empGrenadeExplosionDebris2Particle";
};

datablock ParticleData(empGrenadeExplosionHazeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0;
	windCoefficient		= 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 900;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "0.7 0.7 0.9 0.8";
	colors[1]     = "0.4 0.4 0.7 0.1";
	colors[2]     = "0.1 0.1 0.4 0.0";
	times[0]	= 0.0;
	times[1]	= 0.3;
	times[2]	= 1.0;

	sizes[0]      = 3.0;
	sizes[1]      = 3.85;
	sizes[2]      = 4.35;

	useInvAlpha = false;
};

datablock ParticleEmitterData(empGrenadeExplosionHazeEmitter)
{
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;
	ejectionVelocity = 6;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "empGrenadeExplosionHazeParticle";

	useEmitterColors = true;
};

datablock ParticleData(empGrenadeExplosionCloudParticle)
{
	dragCoefficient		= 0.5;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 900;
	lifetimeVarianceMS	= 00;
	spinSpeed		= 0.0;
	spinRandomMin		= -90.0;
	spinRandomMax		= 90.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/star1";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
	colors[0]     = "0.1 0.3 1.0 0.8";
	colors[1]     = "0.1 0.1 0.1 0.0";
	sizes[0]      = 2;
	sizes[1]      = 0.5;
};

datablock ParticleEmitterData(empGrenadeExplosionCloudEmitter)
{
	ejectionPeriodMS = 4;
	periodVarianceMS = 0;
	ejectionVelocity = 7;
	velocityVariance = 1.0;
	ejectionOffset   = 3.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "empGrenadeExplosionCloudParticle";
};



//drone jet
datablock ParticleData(DroneJetParticle : playerJetParticle)
{
	dragCoefficient = 3;
	gravityCoefficient = -10;
	times[1] = "0.5";
};

datablock ParticleEmitterData(DroneJetEmitter : playerJetEmitter)
{
	particles = "DroneJetParticle";
	ejectionVelocity = 0;
};



//drone explosion
datablock ParticleData(droneExplosionParticle : vehicleExplosionParticle)
{
	dragCoefficient = 12;
	gravityCoefficient = -0.1;
	lifetimeMS = 900;
	lifetimeVarianceMS = 300;
	colors[1] = "0.3 0.3 0.4 0";
	colors[0] = "0.0 0.0 0.0 0.9";
	sizes[0] = 3;
	sizes[1] = 5;
};

datablock ParticleEmitterData(droneExplosionEmitter : vehicleExplosionEmitter)
{
	ejectionPeriodMS = 2;
	thetaMin = 0;
	thetaMax = 180;
	lifetimeMS = 31;
	ejectionVelocity = 26;
	velocityVariance = 1;
	particles = "droneExplosionParticle";
	uiName = "";
};

datablock ParticleData(droneExplosionParticle2 : vehicleExplosionParticle2)
{
	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
	dragCoefficient = 5;
	gravityCoefficient = 0;
	lifetimeMS = 500;
	lifetimeVarianceMS = 250;
	textureName = "./resources/bolt";
	colors[0] = "1.0 1.0 0.0 1.0";
	colors[1] = "1.0 1.0 0.0 0.0";
	sizes[0] = 0.8;
	sizes[1] = 0.5;
};

datablock ParticleEmitterData(droneExplosionEmitter2 : vehicleExplosionEmitter2)
{
	ejectionPeriodMS = 3;
	thetaMin = 0;
	thetaMax = 180;
	lifetimeMS = 1000;
	ejectionVelocity = 8;
	velocityVariance = 2;
	particles = "droneExplosionParticle2";
	uiName = "";
	emitterNode = "";
};

datablock ParticleData(droneDebrisTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= -0.5;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 150;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
	colors[0]	= "0.0 0.0 0.0 0";
	colors[1]	= "0.0 0.0 0.0 1";
	colors[2]	= "0.0 0.0 0.0 0.0";

	sizes[0]	= 0.5;
	sizes[1]	= 1.5;
	sizes[2]	= 2;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(droneDebrisTrailEmitter)
{
	ejectionPeriodMS = 20;
	periodVarianceMS = 0;
	ejectionVelocity = 0.0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.5;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "droneDebrisTrailParticle";
};



//markerlight explosion
datablock ParticleEmitterData(markerlightExplosionEmitter)
{
	ejectionPeriodMS = 7;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "markerlightParticleB";

	useEmitterColors = true;

	uiName = "Markerlight Explosion";
};



//markerlight trail
datablock ParticleData(markerlightTrailParticle)
 {
   dragCoefficient      = 3;
   gravityCoefficient   = -0.0;
   inheritedVelFactor   = 1.0;
   constantAcceleration = 0.0;
   lifetimeMS           = 250;
   lifetimeVarianceMS   = 50;
   textureName          = "./resources/bolt";
   spinSpeed		   = 0.0;
   spinRandomMin		= 0.0;
   spinRandomMax		= 0.0;
   colors[0]     = "0.2 1 1 0.9";
   colors[1]     = "0.1 1 1 0.5";
   colors[2]     = "0 1 1 0";

   sizes[0]      = 0.1;
   sizes[1]      = 0.1;
   sizes[2]      = 0.05;

   times[0] = 0.0;
   times[1] = 0.8;
   times[2] = 1.0;

   useInvAlpha = false;
};

datablock ParticleEmitterData(markerlightTrailEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 1;
   ejectionVelocity = 0.15;
   velocityVariance = 0.1;
   ejectionOffset   = 0.1;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "markerlightTrailParticle";

   useEmitterColors = true;

   uiName = "Markerlight Trail";
};