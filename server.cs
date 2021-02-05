exec("./lib/Script_ObstructRadiusDamage.cs");
exec("./lib/Support_SimpleChargeWeapons.cs");
exec("./lib/Support_Markerlights.cs");
exec("./lib/Support_PickupDuplicateItems.cs");
exec("./lib/Support_ShapelinesV2/server.cs");

exec("./lib/colorUtils.cs");
exec("./lib/hexToInt.cs");
exec("./lib/speedUtils.cs");
exec("./lib/objectColors.cs");
exec("./lib/vectorUtils.cs");

if (isFunction(RTB_registerPref))
{
	RTB_registerPref("Max player drone count",			"Marker Lasers",	"$Pref::Server::MarkerLasers::MaxDroneCount",		"int 1 12",		"Server_MarkerLasers",	2,			0,			0);
	RTB_registerPref("Enable click to recover drone",	"Marker Lasers",	"$Pref::Server::MarkerLasers::ClickRecoverDrone",	"bool",			"Server_MarkerLasers",	1,			0,			0);
} else {
	if($Pref::Server::MarkerLasers::MaxDroneCount $= "")		$Pref::Server::MarkerLasers::MaxDroneCount = 2;
	if($Pref::Server::MarkerLasers::ClickRecoverDrone $= "")	$Pref::Server::MarkerLasers::ClickRecoverDrone = 1;
}

exec("./src/exec.cs");

