function vectorAngle(%vec1, %vec2)
{
	%vec1 = vectorNormalize(%vec1);
	%vec2 = vectorNormalize(%vec2);

	return mACos(vectorDot(%vec1, %vec2));
}

function distanceFromVector(%start, %end, %pos) {
	//calculate parallelogram area of the three points and divide by base to get height
	%startToEnd = vectorSub(%end, %start);
	%startToPos = vectorSub(%pos, %start);
	%normal = vectorNormalize(%startToEnd);

	//projection of startToPos onto normal to determine location on line defined by startToEnd
	%dot = vectorDot(%startToPos, %normal);
	//behind the starting point of the light beam, or past the ending point of the vector
	if (%dot < 0 || %dot / vectorLen(%startToEnd) > 1) {
		%dist = -1;
	} else {
		//calculate parallelogram area of the three points and divide by base to get height eg dist from beam
		%area = vectorLen(vectorCross(%startToEnd, %startToPos));
		%dist = %area / vectorLen(%startToEnd);		
	}
	return %dist;
}