exec("./lib/Script_ObstructRadiusDamage.cs");
exec("./lib/Support_SimpleChargeWeapons.cs");
exec("./lib/Support_Markerlights.cs");
exec("./lib/Support_PickupDuplicateItems.cs");

exec("./lib/colorUtils.cs");
exec("./lib/hexToInt.cs");
exec("./lib/speedUtils.cs");
exec("./lib/objectColors.cs");
exec("./lib/vectorUtils.cs");

%errorA = ForceRequiredAddOn("Vehicle_Jeep");

if(%errorA == $Error::AddOn_Disabled)
{
	JeepVehicle.uiName = "";
}

exec("./src/exec.cs");