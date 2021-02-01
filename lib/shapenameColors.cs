package RecordShapeNameColors
{
	function Player::setShapeNameColor(%pl, %color)
	{
		%pl.lastShapeNameColor = %color;
		return parent::setShapeNameColor(%color);
	}
};
activatePackage(RecordShapeNameColors);