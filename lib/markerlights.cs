//image.markerlightSupport = 1
//image.markerlightMaxRange = 100
//image.markerlightMaxAngle = 0.3927 (pi/8)
//image.markerlightSpread
//image.markerlightProjectile
//image.markerlightShellCount

if (!isObject($MarkerlightSimSet))
{
	$MarkerlightSimSet = new SimSet();
}

datablock ParticleData(MarkerlightParticleA : CameraParticleA)
{
	textureName = "Add-ons/Projectile_Pong/square";

	spinRandomMin = 360 * 5;
	spinRandomMax = 360 * 5;
	spinSpeed = 0;
	colors[0] = "0.2 0.2 0.5 0.2";
	colors[1] = "0.2 0.2 0.5 0.5";
	colors[2] = "0.2 0.2 0.5 0";
	colors[3] = "0.2 0.2 0.5 0";

	sizes[0] = "1.4";
	sizes[1] = "0.8";
	sizes[2] = "0.8";
	sizes[3] = "0.8";

	times[0] = 0;
	times[1] = 0.8;
	times[2] = 1;
	times[3] = 1;
};

datablock ParticleData(MarkerlightParticleB : CameraParticleA)
{
	textureName = "base/data/particles/thinring";

	lifetimeMS = 200;
	lifetimeVarianceMS = 60;

	spinRandomMax = 0;
	spinRandomMin = 0;
	spinSpeed = 0;
	colors[0] = "0.7 0.7 1 0.1";
	colors[1] = "0 1 1 0.25";
	colors[2] = "0.7 0.7 1 0";
	colors[3] = "0.7 0.7 1 0";

	sizes[0] = 1.6;
	sizes[1] = 3;
	sizes[2] = 0.8;
	sizes[3] = 0.8;

	times[0] = 0;
	times[1] = 0.1;
	times[2] = 1;
	times[3] = 1;
};

datablock ParticleEmitterData(MarkerlightEmitterA : CameraEmitterA)
{
	ejectionPeriodMS = 20;
	ejectionOffset = 0.02;
	particles = "MarkerlightParticleA";
	uiName = "Markerlight Emitter A";
};

datablock ParticleEmitterData(MarkerlightEmitterB : CameraEmitterA)
{
	ejectionPeriodMS = 20;
	ejectionOffset = 0.02;
	particles = "MarkerlightParticleB";
	uiName = "Markerlight Emitter B";
};

datablock ShapeBaseImageData(MarkerlightImage : CameraImage)
{
	stateEmitter[1] = "MarkerlightEmitterA";
	stateEmitter[2] = "MarkerlightEmitterB";
	
	mountPoint = 2;
};

datablock StaticShapeData(MarkerlightShape)
{
	shapeFile = "base/data/shapes/empty.dts";
};




package MarkerlightPackage
{
	function Player::emote(%pl, %image)
	{
		%pl.lastEmoted = getSimTime();

		return parent::emote(%pl, %image);
	}

	function serverCmdLight(%cl)
	{
		if (isObject(%pl = %cl.player) && %pl.getMountedImage(0).markerlightSupport)
		{
			%pl.markerlightDisabled = !%pl.markerlightDisabled;
			%cl.play3D(brickChangeSound, %pl.getHackPosition());
			if (!%pl.markerlightDisabled)
			{
				%pre = "\c7[[ AUTOTARGET ON ]]";
			}
			else
			{
				%pre = "\c7[[ AUTOTARGET OFF ]]";
			}
			%cl.centerprint("<font:Consolas:16>" @ %pre @ " <br>\c6- Light key to toggle -", 1);
			%cl.skipOverride = getSimTime() + 1000 | 0;
			return;
		}

		return parent::serverCmdLight(%cl);
	}

	function WeaponImage::checkAmmo(%this, %obj, %slot)
	{
		if (%this.markerlightSupport && isFunction(getMarkerlightVector) && isObject(%obj.client))
		{
			%searchProj = isObject(%this.markerlightProjectile) ? %this.markerlightProjectile : %this.projectile;

			%muzzleVector = getMarkerlightVector(%obj, %searchProj, %this.markerlightMaxRange, 
				%this.markerlightMaxAngle, %obj.getMuzzleVector(%slot), %obj.getMuzzlePoint(%slot));
			%foundTarget = getField(%muzzleVector, 1);
			%muzzleVector = getField(%muzzleVector, 0);

			if (%foundTarget)
			{
				if (!%obj.markerlightDisabled)
				{
					%pre = "\c2[[ AUTOTARGET ON ]]";
				}
				else
				{
					%pre = "\c0[[ AUTOTARGET OFF ]]";
				}
				%obj.client.centerprint("<font:Consolas:16>" @ %pre @ " <br>\c6- Light key to toggle -", 1);
				%obj.client.skipOverride = 0;
			}
			else if (%obj.client.skipOverride < getSimTime())
			{
				%obj.client.centerprint("", 1);
			}
		}

		return parent::checkAmmo(%this, %obj, %slot);
	}
};
activatePackage(MarkerlightPackage);



function updateMarkerlights(%index)
{
	cancel($MarkerlightCheckSchedule);

	if (!isObject($MarkerlightSimSet))
	{
		return;
	}


	%count = $MarkerlightSimSet.getCount();
	if (%index >= %count) { %index = %count - 1; }
	
	while (%iter++ <= 32 && %count > 0)
	{
		if (%index < 0)
		{
			break;
		}

		%obj = $MarkerlightSimSet.getObject(%index);
		%index--;

		if ((%obj.markerlightExpireTime >= 0 && %obj.markerlightExpireTime < getSimTime()) 
			|| %obj.getDamageState() !$= "Enabled")
		{
			%removeList[%removeCount++ - 1] = %obj;
			continue;
		}

		if (%obj.getMountedImage(3).getName() !$= "MarkerlightImage" 
			&& (%obj.lastEmoted + 250 | 0) < getSimTime())
		{
			%obj.mountImage("MarkerlightImage", 3);
		}
	}

	for (%i = 0; %i < %removeCount; %i++)
	{
		if (isObject(%removeList[%i])) { %removeList[%i].removeMarkerlight(); }
	}

	if (%index < 0) { %index = $MarkerlightSimSet.getCount(); }
	$MarkerlightCheckSchedule = schedule(33, $MarkerlightSimSet, updateMarkerlights, %index);
}

updateMarkerlights(0);

function ShapeBase::attachMarkerlight(%obj, %time)
{
	if (%obj.getDamageState() !$= "Enabled")
	{
		return;
	}

	if (!$MarkerlightSimSet.isMember(%obj))
	{
		$MarkerlightSimSet.add(%obj);
	}

	if (%time < 0 || %obj.markerlightExpireTime < 0)
	{
		%obj.markerlightExpireTime = -1;
	}
	else
	{
		if (%obj.markerlightExpireTime < getSimTime()) { %obj.markerlightExpireTime = getSimTime(); }

		%obj.markerlightExpireTime = ((%obj.markerlightExpireTime | 0) + (%time | 0)) | 0;
	}
	%obj.mountImage("MarkerlightImage", 3);
}

function ShapeBase::removeMarkerlight(%obj)
{
	$MarkerlightSimSet.remove(%obj);

	%obj.unmountImage(3);

	if (%obj.isFixedMarkerlight)
	{
		%obj.schedule(100, delete);
	}
}

function attachFixedMarkerlight(%pos, %time)
{
	//search nearby for static shapes
	%mask = $TypeMasks::StaticShapeObjectType;
	initContainerRadiusSearch(%pos, 0.5, %mask);
	while (isObject(%next = containerSearchNext()))
	{
		if (%next.isFixedMarkerlight)
		{
			%shape = %next;
			break;
		}
	}

	if (!isObject(%shape))
	{
		%shape = new StaticShape()
		{
			dataBlock = MarkerlightShape;
			isFixedMarkerlight = 1;
		};
	}

	%shape.setTransform(%pos);
	%shape.attachMarkerlight(%time);
}

function getClosestMarkerlight(%searcher, %maxRange, %maxAngle, %muzzleVector, %muzzlePos)
{
	//minigameCanDamage: -1 if neither object is in a minigame, 0 if cannot, 1 if can
	//valid target if -1 or 1
	for (%i = 0; %i < $MarkerlightSimSet.getCount(); %i++)
	{
		%obj = $MarkerlightSimSet.getObject(%i);
		if (%obj.getDamageState() !$= "Enabled" || %obj == %searcher || !minigameCanDamage(%obj, %searcher))
		{
			continue;
		}

		//validity checks - range, angle, line of sight
		%objPos = %obj.getPosition();
		%dist = vectorDist(%objPos, %muzzlePos);
		if (%dist > %maxRange)
		{
			continue;
		}

		%searchToObj = vectorSub(%objPos, %muzzlePos);
		%angle = vectorAngle(%muzzleVector, %searchToObj);
		if (%angle > %maxAngle)
		{
			continue;
		}

		if (isFunction(%obj.getClassName(), "getHackPosition")) { %targetPos = %obj.getHackPosition(); }
		else { %targetPos = %obj.getWorldBoxCenter(); }

		%ray = containerRaycast(%muzzlePos, %targetPos, $Typemasks::fxBrickObjectType | $Typemasks::StaticObjectType);
		%hit = getWord(%ray, 0);
		if (isObject(%hit) && %hit != %obj)
		{
			continue;
		}

		//best target selection
		//change if one of the following:
		//no value set
		//best vs current angle is bigger than %angleIgnore, AND best angle is more than current angle
		//best distance is more than current target distance, AND angle difference is more than %angleIgnore

		%angleIgnore = 0.08;
		%angleDiff = mAbs(%bestAngle - %angle);
		// talk((%angleDiff > %angleIgnore) @ " | ba:" @ %bestAngle @ " | a:" @ %angle @ " | ad:" @ %angleDiff);

		if (%bestAngle $= "" 
			|| (%bestAngle > %angle && %angleDiff > %angleIgnore)
			|| (%bestDistance > %dist && %angleDiff < %angleIgnore))
		{
			%bestAngle = %angle;
			%bestDistance = %dist;
			%bestTarget = %obj;
		}
	}
	return %bestTarget;
}

function getMarkerlightVector(%searcher, %projDB, %maxRange, %maxAngle, %muzzleVector, %muzzlePos)
{
	//defaults
	if (%maxRange <= 0) { %maxRange = 100; }
	if (%maxAngle <= 0) { %maxAngle = 3.14159265 / 8; }
	if (%muzzleVector $= "") { %muzzleVector = %searcher.getMuzzleVector(0); }
	if (%muzzlePos $= "") { %muzzlePos = %searcher.getMuzzlePoint(0); }

	%obj = getClosestMarkerlight(%searcher, %maxRange, %maxAngle, %muzzleVector, %muzzlePos);

	if (!isObject(%obj))
	{
		return %muzzleVector TAB 0;
	}
	else //need to lead target
	{
		if (isFunction(%obj.getClassName(), "getHackPosition")) { %targetPos = %obj.getHackPosition(); }
		else { %targetPos = %obj.getWorldBoxCenter(); }

		%basePos = %obj.getPosition();
		%diff = vectorSub(%targetPos, %basePos);
		%targetVel = %obj.getVelocity();
		%muzzleSpeed = %projDB.muzzleVelocity;

		%iterFinalPos = calculateLeadLocation_Iterative(%obj, %muzzlePos, %basePos, %muzzleSpeed, %targetVel);
		%iterFinalPos = vectorAdd(%iterFinalPos, %diff);
		return vectorNormalize(vectorSub(%iterFinalPos, %muzzlePos)) TAB %obj;
	}
}


function calculateLeadLocation_Iterative(%obj, %pos0, %pos1, %speed0, %vel1)
{
	%currTime = vectorDist(%pos0, %pos1) / %speed0;
	%finalPos = calculateFutureGravityPosition(%pos1, %vel1, %currTime);

	// talk("iter: 0 time: " @ mFloatLength(%currTime, 2));
	for (%i = 1; %i <= 16; %i++)
	{
		%nextTime = vectorDist(%finalPos, %pos0) / %speed0;
		%nextFinalPos = calculateFutureGravityPosition(%obj, %pos1, %vel1, %nextTime);
		%nextDelta = mAbs(%nextTime - %currTime);

		// talk("iter: " @ %i @ " time: " @ mFloatLength(%nextTime, 2));
		if (%nextDelta < 0.01)
		{
			%currTime = %nextTime;
			%finalPos = %nextFinalPos;
			break;
		}
		else if (%i > 6 && %nextDelta > %lastDelta)
		{
			break;
		}
		%currTime = %nextTime;
		%finalPos = %nextFinalPos;
		%lastDelta = %nextDelta;
	}
	return %finalPos;
}

function calculateFutureGravityPosition(%obj, %pos, %vel, %time)
{
	%xy = getWords(%vel, 0, 1);
	%z = getWords(%vel, 2);

	%xyPos = vectorAdd(vectorScale(%xy, %time), %pos);
	%zDelta = (%z * %time) - (9.8 * %time * %time);
	%finalPos = vectorAdd(%xyPos, "0 0 " @ %zDelta);

	%masks = $TypeMasks::fxBrickObjectType | $Typemasks::StaticObjectType | $Typemasks::PlayerObjectType;
	%ray = containerRaycast(%pos, %finalPos, %masks, %obj);
	%hit = getWord(%ray, 0);
	%hitloc = getWords(%ray, 1, 3);
	if (isObject(%hit) && (%hit.getClassName() !$= "fxDTSBrick" || %hit.isColliding()))
	{
		%finalPos = %hitloc;
	}
	// echo(%hit SPC " | " @ %hitloc @ " | " @ %finalPos);
	return %finalPos;
}

registerOutputEvent("Player", "attachMarkerlight", "int -1 100000 1000");
registerOutputEvent("Bot", "attachMarkerlight", "int -1 100000 1000");