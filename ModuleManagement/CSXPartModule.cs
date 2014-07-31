using UnityEngine;
using KSP.IO;

namespace CSXIndustry.ModuleManagement
{
	public class CSXPartModule : PartModule
	{
		[KSPField]
		public int partType = 0;

		private Part parentPart = null;
		private PartModule csxModule = null;

		private string csxModuleName = "PartModuleName";

		public void SetPart(Part part, PartModule module)
		{
			this.parentPart = part;
			this.csxModule = module;

			csxModuleName = module.moduleName;
		}

		public Part CSXPart
		{
			get { return parentPart; }
			set { this.parentPart = value; }
		}

		public PartModule CSXModule
		{
			get { return csxModule; }
			set { this.csxModule = value; }
		}

		public string CSXModuleName
		{
			get { return csxModuleName; }
			set { this.csxModuleName = value; }
		}

		public virtual void CallFromTop()
		{
			Debug.Log("[CSX_Ind] Part module manager");
		}
	}

	public class CSXParts
	{
		public const int NONE = 0;
		public const int fuelCell = 1;
		public const int filter = 2;
		public const int container = 3;
		public const int greenHouse = 4;

		public const int crewQuarters = 5;
	}

	public enum CSXPartType
	{
		NONE = 0,
		Power = 1,
		Filter = 2,
		Produce = 3,
		Collector = 4
	}
}
