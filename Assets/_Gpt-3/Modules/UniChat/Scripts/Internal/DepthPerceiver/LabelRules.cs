namespace Modules.UniChat.Internal.DepthPerceiver
{
	public static class LabelRules
	{
		public static readonly string[] RemoveFromNames = {
			"SM_Generic_",
			"SM_Prop_",
			"SM_Veh_",
			"SM_Env_",
			"SM_Wep_",
			"SM_Bld_"
		};

		public static readonly string[] Blacklist = {
			"SM_Generic_SkyDome_01",
			"VolumeSensor",
			"Capsule",
			"Cube"
		};
	}
}