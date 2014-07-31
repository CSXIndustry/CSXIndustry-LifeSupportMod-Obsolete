/************************************************************************
 * CSX Industry - Life Support Part+Plugin Pack for Kerbal Space Program*
 *                                                                      *
 * Initial Alpha Release Version 0.6a                                   *
 *                                                                      *
 * Created by Charlie S.                                                *
 * Built on July 26th, 2014                                             *
 * Initial Built on July 12th, 2014                                     *
 ************************************************************************/

using UnityEngine;
using KSP.IO;

namespace CSXIndustry.ModuleManagement
{
	public class CSXWasteContainer : PartModule
	{
		[KSPField]
		public float convertRate;
		[KSPField(guiActive = true, guiName = "Filter Efficiency")]
		public float filterEfficiency;

		private double efficiencyCounter = (24 * 3600) * 30;

		public void FixedUpdate()
		{
			if (IsPowered())
			{
				UpdateFilter();

				efficiencyCounter -= 1.0 * TimeWarp.fixedDeltaTime;
				if(efficiencyCounter < 0)
				{
					filterEfficiency *= 0.998f;
					efficiencyCounter = (24 * 3600) * 30;
				}
			}
		}

		private void UpdateFilter()
		{
			double waterAcquired = part.RequestResource(Resources.byWater, 1.0 * convertRate * TimeWarp.fixedDeltaTime);
			part.RequestResource(Resources.pureWater, -waterAcquired * filterEfficiency * TimeWarp.fixedDeltaTime);

			waterAcquired = part.RequestResource(Resources.wasteWater, 1.0 * convertRate * TimeWarp.fixedDeltaTime);
			part.RequestResource(Resources.pureWater, -waterAcquired * filterEfficiency * TimeWarp.fixedDeltaTime);
		}

		private bool IsPowered()
		{
			if (part.RequestResource(Resources.power, 2.0 * TimeWarp.fixedDeltaTime) < 2.0 * TimeWarp.fixedDeltaTime)
				return false;
			else return true;
		}

		// Right click options and more

		[KSPEvent(guiActive = true, guiName = "Dump waste water")]
		public void DumpWasteWater()
		{
			Events["DumpWasteWater"].active = false;
			Events["StopDumpping"].active = true;
		}

		[KSPEvent(guiActive = true, guiName = "Stop dumpping", active = false)]
		public void StopDumpping()
		{
			Events["DumpWasteWater"].active = true;
			Events["StopDumpping"].active = false;
		}
	}
}
