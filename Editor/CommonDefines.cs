namespace PhantasmicGames.CommonEditor
{
	public class CommonDefines : CustomDefines
	{
		public override bool autoEvaluate => true;

		public override DefineInfo[] defines => new DefineInfo[]
		{
			new DefineInfo()
			{
				define = "PHANTASMICGAMES_COMMON",
				condition = () => true,
			}
		};
	}
}