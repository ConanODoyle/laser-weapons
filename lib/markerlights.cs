if (!isObject($MarkerlightSimSet))
{
	$MarkerlightSimSet = new SimSet();
}

datablock StaticShapeData(MarkerlightShape)
{
	shapeFile = "base/data/shapes/empty.dts";
};

function ShapeBase::attachMarkerlight(%obj, %time)
{
	%timeRemaining = getTimeRemaining(%obj.removeMarkerlightSchedule);
	cancel(%obj.removeMarkerlightSchedule);

	if (%obj.getDamageState() !$= "Enabled")
	{
		return;
	}

	if (!$MarkerlightSimSet.isMember(%obj))
	{
		$MarkerlightSimSet.add(%obj);
	}

	if (%time < 0)
	{
		%obj.permanentMarkerlight = 1;
	}

	if (!%obj.permanentMarkerlight)
	{
		%obj.removeMarkerlightSchedule = %obj.schedule(%timeRemaining + %time, removeMarkerlight);
	}
}

function ShapeBase::removeMarkerlight(%obj)
{
	$MarkerlightSimSet.remove(%obj);

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
		if (%obj.getDamageState() !$= "Enabled" || !minigameCanDamage(%obj, %searcher))
		{
			continue;
		}

		//validity checks
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

		//best target selection
		//change if one of the following:
		//no value set
		//best vs current angle is bigger than %angleIgnore, AND best angle is more than current angle
		//best distance is more than current target distance

		%angleIgnore = 0.1;
		%angleDiff = mAbs(%bestAngle - %angle);

		if (%bestAngle $= "" 
			|| (%bestAngle > %angle && %angleDiff > %angleIgnore)
			|| (%bestDistance > %dist))
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
	if (%maxAngle <= 0) { %maxAngle = 0.78; }
	if (%muzzleVector $= "") { %muzzleVector = %searcher.getMuzzleVector(0); }
	if (%muzzlePos $= "") { %muzzlePos = %searcher.getMuzzlePoint(0); }

	%obj = getClosestMarkerlight(%searcher, %maxRange, %maxAngle, %muzzleVector, %muzzlePos);

	if (!isObject(%obj))
	{
		return %muzzleVector TAB 0;
	}
	else //need to lead target
	{
		%targetPos = %obj.getPosition();
		%targetVel = %obj.getVelocity();
		%muzzleSpeed = %projDB.muzzleVelocity;

		%iterFinalPos = iterativeSolution(%muzzlePos, %targetPos, %muzzleSpeed, %targetVel);
		return vectorNormalize(vectorSub(%iterFinalPos, %muzzlePos)) TAB %obj;
	}
}


function iterativeSolution(%pos0, %pos1, %speed0, %vel1)
{
	%currTime = vectorDist(%pos0, %pos1) / %speed0;
	%finalPos = vectorAdd(%pos1, vectorScale(%vel1, %currTime));

	// talk("iter: 0 time: " @ mFloatLength(%currTime, 2));
	for (%i = 1; %i <= 16; %i++)
	{
		%nextTime = vectorDist(%finalPos, %pos0) / %speed0;
		%nextFinalPos = vectorAdd(%pos1, vectorScale(%vel1, %nextTime));
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