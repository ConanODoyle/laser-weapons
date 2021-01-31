function Player::setMoveFactor(%pl, %factor)
{
	%db = %pl.getDatablock();

	%pl.setMaxForwardSpeed(%db.maxForwardSpeed * %factor);
	%pl.setMaxSideSpeed(%db.maxSideSpeed * %factor);
	%pl.setMaxBackwardSpeed(%db.maxBackwardSpeed * %factor);

	%pl.setMaxCrouchForwardSpeed(%db.maxForwardCrouchSpeed * %factor);
	%pl.setMaxCrouchSideSpeed(%db.maxSideCrouchSpeed * %factor);
	%pl.setMaxCrouchBackwardSpeed(%db.maxBackwardCrouchSpeed * %factor);
}