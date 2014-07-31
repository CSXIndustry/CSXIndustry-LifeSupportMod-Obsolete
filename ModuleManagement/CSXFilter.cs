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

namespace CSXIndustry.EnvManagement
{
    public class CSXFilter : PartModule
    {
		[KSPField]
		public float filterRate;

		private bool convertTo = true;

        public void FixedUpdate()
        {
			if(IsPowered())
				FilterKOO();
        }

		private bool IsPowered()
		{
			if (part.RequestResource(Resources.power, (0.2 * filterRate) * TimeWarp.fixedDeltaTime) < (0.2 * filterRate) * TimeWarp.fixedDeltaTime)
				return false;
			else return true;
		}

		private void FilterKOO()
		{
			double koo = part.RequestResource(Resources.koo, 0.1 * filterRate * TimeWarp.fixedDeltaTime) / (0.1 * filterRate * TimeWarp.fixedDeltaTime);
			double hydrogen = part.RequestResource(Resources.hydrogen, 0.4 * filterRate * TimeWarp.fixedDeltaTime) / (0.4 * filterRate * TimeWarp.fixedDeltaTime);
			double heat = part.RequestResource(Resources.heat, 8.0 * filterRate * TimeWarp.fixedDeltaTime) / (8.0 * filterRate * TimeWarp.fixedDeltaTime);

			double factorFinal = koo * hydrogen * heat;

			part.RequestResource(Resources.byWater, -0.2 * factorFinal * filterRate * TimeWarp.fixedDeltaTime);

			if (convertTo)
				part.RequestResource(Resources.kethane, -0.1 * factorFinal * filterRate * TimeWarp.fixedDeltaTime);
			else
				part.RequestResource(Resources.karbonite, -0.1 * factorFinal * filterRate * TimeWarp.fixedDeltaTime);
		}

		// KSP Events, Action Groups, and others

		[KSPEvent(guiActive = true, guiName = "Convert to Kethane")]
		public void ToKethane()
		{
			convertTo = true;
			ScreenMessages.PostScreenMessage("Filter produces Kethane", 5.0f, ScreenMessageStyle.UPPER_CENTER);

			Events["ToKarbonite"].active = true;
			Events["ToKethane"].active = false;
		}

		[KSPEvent(guiActive = true, guiName = "Convert to Karbonite", active = false)]
		public void ToKarbonite()
		{
			convertTo = false;
			ScreenMessages.PostScreenMessage("Filter produces Karbonite", 5.0f, ScreenMessageStyle.UPPER_CENTER);

			Events["ToKarbonite"].active = false;
			Events["ToKethane"].active = true;
		}

		public override string GetInfo()
		{
			return "Filter rate : " + filterRate;
		}
    }
}
