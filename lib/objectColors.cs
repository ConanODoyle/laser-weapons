package RecordObjectColors
{
	function Player::setShapeNameColor(%pl, %color)
	{
		%pl.lastShapeNameColor = %color;
		parent::setShapeNameColor(%pl, %color);
	}

	function Player::setNodeColor(%pl, %node, %color)
	{
		%pl.lastNodeColor[getSafeVariableName(%node)] = %color;
		parent::setNodeColor(%pl, %node, %color);
	}
};
activatePackage(RecordObjectColors);