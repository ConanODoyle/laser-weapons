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
