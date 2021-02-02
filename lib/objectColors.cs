package RecordObjectColors
{
	function Player::setShapeNameColor(%pl, %color)
	{
		%pl.lastShapeNameColor = %color;
		return parent::setShapeNameColor(%pl, %color);
	}

	function Player::setNodeColor(%pl, %node, %color)
	{
		%pl.lastNodeColor[getSafeVariableName(%node)] = %color;
		return parent::setNodeColor(%pl, %node, %color);
	}
};
activatePackage(RecordObjectColors);