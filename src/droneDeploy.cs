//drone deploy item
datablock ProjectileData(droneDeployProjectile)
{
	projectileShapeName = "./resources/empGrenadeProjectile.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::electroDirect;
	radiusDamageType  = $DamageType::electroDirect;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = "";
	particleEmitter     = ChargeLaserTracer;

	muzzleVelocity      = 15;
	velInheritFactor    = 0;
	explodeOnPlayerImpact = false;
	explodeOnDeath        = true;  

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;

	armingDelay         = 19800;
	lifetime            = 20000;
	fadeDelay           = 19800;
	bounceElasticity    = 0.1;
	bounceFriction      = 0.8;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};

datablock ItemData(droneDeployItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./resources/empGrenade.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Drone Deployer";
	iconName = "";
	colorShiftColor = "1 0 0 1";
	doColorShift = true;

	image = droneDeployImage;
	canDrop = true;
	canPickupMultiple = 1;
};

datablock ShapeBaseImageData(droneDeployImage)
{
	shapeFile = "./resources/empGrenade.dts";
	emap = true;

	item = droneDeployItem;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	className = "WeaponImage";
	armReady = true;

	doColorShift = droneDeployItem.doColorShift;
	colorShiftColor = droneDeployItem.colorShiftColor;

	weaponUseCount = 1;
	weaponReserveMax = 3;

	projectileType = Projectile;
	projectile = droneDeployProjectile;

	stateName[0]							= "Ready";
	stateSound[0]							= weaponSwitchSound;
	stateSequence[0]						= "root";
	stateTransitionOnTriggerDown[0]			= "Charge";

	stateName[1]							= "Charge";
	stateScript[1]							= "onChargeStart";
	stateSequence[1]						= "pinOut";
	stateSound[1]							= brickChangeSound;
	stateTransitionOnTriggerUp[1]			= "Fire";
	stateWaitForTimeout[1]					= false;

	stateName[2]							= "Fire";
	stateTransitionOnTimeout[2]				= "Ready";
	stateScript[2]							= "onFire";
	stateEjectShell[2]						= true;
	stateTimeoutValue[2]					= 0.3;
};

function droneDeployImage::onUnmount(%this, %obj, %slot)
{
	%obj.playThread(0, root);
	cancel(%obj.spearReadySched);
	%obj.droneDeploySlot = "";
}

function droneDeployImage::onChargeStop(%this, %obj, %slot) // overcooked!
{
	%this.onFire(%obj, %slot);
}

function droneDeployImage::onChargeStart(%this, %obj, %slot)
{
	%obj.droneDeploySlot = %obj.currTool;
	%obj.playThread(0, shiftRight);
	%obj.spearReadySched = %obj.schedule(500, playThread, 0, spearReady);
}

function droneDeployImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftDown);
	%obj.playThread(0, spearThrow);
	serverPlay3D(weaponSwitchSound, %obj.getMuzzlePoint(%slot));

	%velocity = VectorScale(%obj.getMuzzleVector(%slot), %this.projectile.muzzleVelocity);
	
	%p = new Projectile()
	{
		dataBlock = %this.projectile;
		initialVelocity = %velocity;
		initialPosition = %obj.getEyePoint();
		sourceObject = %obj;
		sourceSlot = %slot;
		client = %obj.client;
	};

	//removal from inventory
	%obj.tool[%obj.droneDeploySlot] = "";
	if (isObject(%obj.client))
	{
		messageClient(%obj.client, 'MsgItemPickup', "", %obj.droneDeploySlot, 0);
	}
	%obj.unMountImage(%slot);
}

function droneDeployProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	serverPlay3D(empGrenadeBounceSound, %pos);
	if (isFunction(%obj.sourceObject.getClassName(), "spawnLaserDrone")
		&& %obj.sourceObject.getDamageState() $= "Enabled")
	{
		%faceVector = vectorNormalize(vectorSub(%pos, %obj.initialPosition));
		%obj.sourceObject.spawnLaserDrone(vectorAdd(%pos, vectorScale(%normal, 2)), 
			droneBurstGunImage, 
			droneBurstGunImageLeft,
			getWords(%faceVector, 0, 1));
	}
	%obj.delete();
}