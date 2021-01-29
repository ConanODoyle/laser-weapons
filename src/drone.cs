
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
	shapeFile = "./resources/droneBot.dts";
	uiName = "";

	boundingBox = vectorScale("1 1 1.2", 4);
	crouchBoundingBox = vectorScale("1 1 1.2", 4);

	disableEmotes = 1;
	disableBurn = 1;

	maxDamage = 400;
};

package ChargeLaserDrones
{
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
			%obj.spawnExplosion(spawnExplosion, 1);
			%obj.schedule(50, delete);
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
};
activatePackage(ChargeLaserDrones);



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
		%index--;

		if ((%obj.LaserDroneExpireTime >= 0 && %obj.LaserDroneExpireTime < getSimTime()) 
			|| %obj.getDamageState() !$= "Enabled")
		{
			%removeList[%removeCount++ - 1] = %obj;
			continue;
		}

		//TODO: run AI function here
	}

	for (%i = 0; %i < %removeCount; %i++)
	{
		if (isObject(%removeList[%i])) { $LaserDroneSimSet.remove(%removeList[%i]); }
	}

	if (%index < 0) { %index = $LaserDroneSimSet.getCount(); }
	$LaserDroneCheckSchedule = schedule(33, $LaserDroneSimSet, laserDroneAILoop, %index);
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
	%set = %pl.getLaserDroneSet;
	while (%set.getCount() > 0)
	{
		%obj = %set.getObject(0);
		%obj.spawnExplosion(PlayerSpawnProjectile, 1);
		%obj.delete();
	}
}

function Player::spawnLaserDrone(%pl, %position)
{
	%droneSet = %pl.getLaserDroneSet();

	%mount = new StaticShape(){ dataBlock = droneBotMount; };
	%bot = new AIPlayer() { dataBlock = droneBotArmor; };

	%bot.staticShapeMount = %mount;
	%bot.sourceObject = %pl;
	%bot.client = %pl.client;

	%mount.mountObject(%bot, 0);
	%mount.setTransform(%position);

	$LaserDroneSimSet.add(%bot);
}