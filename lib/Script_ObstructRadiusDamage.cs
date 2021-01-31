//import needed function from script_obstructradiusdamage, as host may not want obstructradiusdamage enabled

function obstructRadiusDamageCheck(%pos, %col) {
	%b = %col.getHackPosition();
	%half = vectorSub(%b, %col.position);

	%a = vectorAdd(%col.position, vectorScale(%half, 0.1));
	%c = vectorAdd(%col.position, vectorScale(%half, 1.9));

	%mask = $TypeMasks::FxBrickObjectType;

	if (containerRayCast(%pos, %a, %mask) !$= 0) {
		if (containerRayCast(%pos, %b, %mask) !$= 0) {
			if (containerRayCast(%pos, %c, %mask) !$= 0) {
				return 0;
			}
		}
	}

	return 1;
}
