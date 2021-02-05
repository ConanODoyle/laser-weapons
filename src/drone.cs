$MaxDroneCount = 2;
if (!isObject($LaserDroneSimSet))
{
	$LaserDroneSimSet = new SimSet();
}

datablock StaticShapeData(droneBotMount)
{
	shapeFile = "base/data/shapes/empty.dts";
};

datablock PlayerData(droneBotArmor : PlayerStandardArmor)
{
	shapeFile = "./resources/drone.dts";
	uiName = "";

	boundingBox = vectorScale("1 1 1.2", 4);
	crouchBoundingBox = vectorScale("1 1 1.2", 4);

	disableEmotes = 1;
	disableBurn = 1;

	maxDamage = 250;
	useCustomPainEffects = 1;
	painHighImage = "";
	painMidImage = "";
	painLowImage = "";
};

datablock DebrisData(droneBotDebris)
{
	emitters = "droneDebrisTrailEmitter";

	shapeFile = "./resources/droneDebris.dts";
	lifetime = 3.0;
	minSpinSpeed = -900.0;
	maxSpinSpeed = 900.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 2;
};

datablock ExplosionData(droneBotExplosion)
{
	//explosionShape = "";
	lifeTimeMS = 180;

	soundProfile = vehicleExplosionSound;

	emitter[0] = droneExplosionEmitter;
	emitter[1] = droneExplosionEmitter2;

	debris = droneBotDebris;
	debrisNum = 1;
	debrisNumVariance = 0;
	debrisPhiMin = 0;
	debrisPhiMax = 360;
	debrisThetaMin = 90;
	debrisThetaMax = 180;
	debrisVelocity = 1;
	debrisVelocityVariance = 1;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "5.0 6.0 5.0";
	camShakeAmp = "6.0 6.0 6.0";
	camShakeDuration = 0.75;
	camShakeRadius = 8.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 20;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 15;
	impulseForce = 0;
	impulseVertical = 0;

	//radius damage
	radiusDamage        = 12;
	damageRadius        = 3.0;

	//burn the players?
	playerBurnTime = 0;
};

datablock ProjectileData(droneBotExplosionProjectile)
{
	directDamage        = 0;
	radiusDamage        = 0;
	damageRadius        = 0;
	explosion           = droneBotExplosion;

	directDamageType  = $DamageType::jeepExplosion;
	radiusDamageType  = $DamageType::jeepExplosion;

	explodeOnDeath		= 1;

	armingDelay         = 0;
	lifetime            = 10;
};


datablock ShapeBaseImageData(droneJetImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;

	mountPoint = 3;

	stateName[0]					= "Emitter1";
	stateEmitter[0]					= "droneJetEmitter";
	stateEmitterTime[0] 			= 1.2;
	stateTimeoutValue[0]			= 1;
	stateTransitionOnTimeout[0]		= "Emitter2";

	stateName[1]					= "Emitter2";
	stateEmitter[1]					= "droneJetEmitter";
	stateEmitterTime[1] 			= 1.2;
	stateTimeoutValue[1]			= 1;
	stateTransitionOnTimeout[1]		= "Emitter1";
};

package ChargeLaserDrones
{
	function Player::activateStuff(%pl)
	{
		%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType;
		%start = %pl.getEyePoint();
		%vec = %pl.getEyeVector();
		%end = vectorAdd(%start, vectorScale(%vec, 8 * getWord(%pl.getScale(), 2)));
		%search = containerRayCast(%start, %end, %mask, %pl);
		%victim = getWord(%search, 0);
		if (isObject(%victim) && %victim.isLaserTurret && %victim.sourceObject == %pl)
		{
			%i = new Item()
			{
				dataBlock = %victim.itemDB;
			};
			MissionCleanup.add(%i);
			%i.setTransform(%pl.getHackPosition());
			%i.schedulePop();

			%victim.spawnExplosion(spawnProjectile, 1);
			%victim.delete();
		}
		return parent::activateStuff(%pl);
	}

	function AIPlayer::setNodeColor(%obj, %node, %color)
	{
		if (%obj.isLaserTurret && !%obj.allowColorChange)
		{
			return;
		}
		parent::setNodeColor(%obj, %node, %color);
	}

	function Armor::onDisabled(%this, %obj, %state)
	{
		if (isObject(%obj.laserDroneSet))
		{
			%obj.explodeLaserDrones();
			%obj.laserDroneSet.clear();
			%obj.laserDroneSet.delete();
		}

		if (isObject(%obj.staticShapeMount))
		{
			%obj.staticShapeMount.delete();
		}

		if (%this.getName() $= "droneBotArmor")
		{
			%obj.client = "";
			%obj.spawnExplosion(droneBotExplosionProjectile, 1);
			%obj.schedule(50, delete);
			return;
		}

		return parent::onDisabled(%this, %obj, %state);
	}

	function Armor::onRemove(%this, %obj)
	{
		if (isObject(%obj.laserDroneSet))
		{
			%obj.explodeLaserDrones();
			%obj.laserDroneSet.clear();
			%obj.laserDroneSet.delete();
		}

		if (isObject(%obj.staticShapeMount))
		{
			%obj.staticShapeMount.delete();
		}

		return parent::onRemove(%this, %obj);
	}

	function Armor::damage(%this, %obj, %sourceObj, %pos, %damage, %damageType)
	{
		%pre = %obj.getDamageLevel();
		%ret = parent::damage(%this, %obj, %sourceObj, %pos, %damage, %damageType);
		if (%obj.getDamageLevel() > %pre && %this.getName() $= "droneBotArmor")
		{
			%obj.spawnExplosion(radioWaveProjectile, getWord(%obj.getScale(), 2) * 2);
		}
		return %ret;
	}

	function Player::emote(%obj, %emote)
	{
		if (%obj.getDatablock().disableEmotes)
		{
			return;
		}
		return parent::emote(%obj,%emote);
	}

	function Player::burn(%obj, %time)
	{
		if (%obj.getDatablock().disableBurn)
		{
			return;
		}
		return parent::burn(%obj, %time);
	}

	function minigameCanDamage(%client, %victimObject)
	{
		if (%client.isLaserTurret)
		{
			%client = %client.sourceObject;
		}
		if (%victimObject.isLaserTurret)
		{
			%victimObject = %victimObject.sourceObject;
		}
		return parent::minigameCanDamage(%client, %victimObject);
	}

	function getBrickgroupFromObject(%obj)
	{
		if (%obj.getType() & $Typemasks::PlayerObjectType && %obj.getDatablock().getName() $= "droneBotArmor")
		{
			return %obj.sourceClient.brickGroup;
		}
		return parent::getBrickgroupFromObject(%obj);
	}

	function getMinigameFromObject(%obj)
	{
		if (isObject(%obj) && %obj.getType() & $Typemasks::PlayerObjectType && %obj.getDatablock().getName() $= "droneBotArmor")
		{
			return %obj.sourceClient.minigame;
		}
		return parent::getMinigameFromObject(%obj);
	}
};
schedule(33, 0, activatePackage, ChargeLaserDrones);



function laserDroneAILoop(%index)
{
	cancel($LaserDroneCheckSchedule);

	if (!isObject($LaserDroneSimSet))
	{
		return;
	}

	%count = $LaserDroneSimSet.getCount();
	if (%index >= %count) { %index = %count - 1; }
	
	while (%iter++ <= 32 && %count > 0)
	{
		if (%index < 0)
		{
			break;
		}

		%obj = $LaserDroneSimSet.getObject(%index);
		droneAICheck(%obj);
		%index--;
	}

	if (%index < 0) { %index = $LaserDroneSimSet.getCount(); }
	$LaserDroneCheckSchedule = schedule(33, $LaserDroneSimSet, laserDroneAILoop, %index);
}
laserDroneAILoop(0);

function droneAICheck(%drone)
{
	%drone.setVelocity("0 0 1");
	%target = getClosestMarkerlight(%drone, 
		%drone.getMountedImage(0).markerLightMaxRange, 
		3.14 * 2, 
		%drone.getMuzzleVector(0), 
		%drone.getMuzzlePoint(0),
		3.14 * 2);

	if (isObject(%target))
	{
		if (!isObject(%drone.target))
		{
			%gainedTarget = 1;
		}

		%drone.target = %target;
		%drone.setAimObject(%target);
		%drone.setImageTrigger(0, 1);
		%drone.setImageTrigger(1, 1);
	}
	else
	{
		if (isObject(%drone.target))
		{
			%lostTarget = 1;
			%drone.setAimVector(%drone.getEyeVector());
		}
		%drone.target = "";
		%drone.setImageTrigger(0, 0);
		%drone.setImageTrigger(1, 0);
	}

	if (%lostTarget)
	{
		%drone.playThread(2, passive);
		serverPlay3D(droneIdleSound, %drone.getPosition());
		%drone.lastPlayedPassiveSound = getSimTime();
		%drone.playedSound = 0;
	}
	if (%gainedTarget)
	{
		%drone.playThread(2, root);
		serverPlay3D(droneTargetFoundSound, %drone.getPosition());
	}

	if (!isObject(%target))
	{
		if (%drone.lastPlayedPassiveSound + 1666 < getSimTime())
		{
			if (!%drone.playedSound)
			{
				serverPlay3D(droneIdleSound, %drone.getPosition());
				%drone.playedSound = 1;
			}
			if (%drone.lastPlayedPassiveSound + (1666 * 2) < getSimTime())
			{
				serverPlay3D(droneIdleSound, %drone.getPosition());
				%drone.playedSound = 0;
				%drone.playThread(2, passive);
				%drone.lastPlayedPassiveSound = getSimTime();
			}
		}
	}
	return (%drone.target + 0) SPC (%lostTarget + 0) SPC (%gainedTarget + 0);
}

function Player::getLaserDroneSet(%pl)
{
	if (!isObject(%pl.laserDroneSet))
	{
		%pl.laserDroneSet = new SimSet();
	}
	return %pl.laserDroneSet;
}

function Player::explodeLaserDrones(%pl)
{
	%set = %pl.getLaserDroneSet();
	while (%set.getCount() > 0)
	{
		%obj = %set.getObject(0);
		%obj.spawnExplosion(droneBotExplosionProjectile, 1);
		%obj.delete();
	}
}

function Player::spawnLaserDrone(%pl, %position, %rightImage, %leftImage, %faceVector, %colorScale, %itemDB)
{
	%droneSet = %pl.getLaserDroneSet();

	%mount = new StaticShape(){ dataBlock = droneBotMount; };
	%bot = new AIPlayer() { dataBlock = droneBotArmor; };

	%bot.staticShapeMount = %mount;
	%bot.sourceObject = %pl;
	%bot.sourceClient = %pl.client;
	%bot.client = %bot;
	%bot.isBot = 1;
	%bot.isLaserTurret = 1;
	%bot.maxYawSpeed = 50;
	%bot.maxPitchSpeed = 50;
	%bot.itemDB = %itemDB;
	%bot.timeAdded = getSimTime();

	%bot.allowColorChange = 1;
	if (%pl.lastShapeNameColor $= "")
	{
		%pl.lastShapeNameColor = "1 1 1";
	}
	%bot.setNodeColor("ALL", vectorScale(%pl.lastShapeNameColor, %colorScale) SPC 1);
	%bot.setShapeName(%pl.client.name @ "'s Drone", 8564862);
	%bot.setShapeNameDistance(30);
	%bot.setShapeNameColor(%pl.lastShapeNameColor);

	%mount.setTransform(%position);
	%mount.mountObject(%bot, 1);
	%bot.setTransform("0 0 0 0 0 1 " @ getRandom() * 3.14159 * 2);
	%bot.mountImage(droneJetImage, 2);

	if (isObject(%rightImage)) { %bot.mountImage(%rightImage, 0); }
	if (isObject(%leftImage)) { %bot.mountImage(%leftImage, 1); }

	%bot.setAimVector(%faceVector);
	%bot.playThread(2, passive);
	%bot.spawnExplosion(radioWaveProjectile, 3);

	$LaserDroneSimSet.add(%bot);
	%droneSet.add(%bot);

	if (%droneSet.getCount() > $MaxDroneCount)
	{
		%oldestTime = getSimTime();
		%oldest = %bot;
		for (%i = 0; %i < %droneSet.getCount(); %i++)
		{
			%next = %droneSet.getObject(%i);
			if (%next.timeAdded < %oldestTime)
			{
				%oldestTime = %next.timeAdded;
				%oldest = %next;
			}
		}
		if (isObject(%oldest))
		{
			%oldest.kill();
		}
	}
}

function serverCmdClearDrones(%cl)
{
	if (isObject(%cl.player))
	{
		%cl.player.explodeLaserDrones();
	}
	messageClient(%cl, '', "\c3Your drones are cleared!");
}

function serverCmdClearAllDrones(%cl)
{
	if (!%cl.isAdmin)
	{
		return;
	}

	while ($LaserDroneSimSet.getCount() > 0)
	{
		$LaserDroneSimSet.getObject(0).spawnExplosion(droneBotExplosionProjectile, 1);
		$LaserDroneSimSet.getObject(0).delete();
	}
	messageAll('MsgClearBricks', "\c2" @ %cl.name @ " cleared all drones.");
}