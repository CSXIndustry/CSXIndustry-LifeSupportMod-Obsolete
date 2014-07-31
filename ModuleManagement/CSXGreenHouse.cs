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
using System.Collections.Generic;

namespace CSXIndustry.ModuleManagement
{
    public class CSXGreenHouse : PartModule
    {
		// .CFG File
		[KSPField]
		public float powerUsage;
		[KSPField]
		public float waterUsage;
		[KSPField]
		public float heatUsage;
		[KSPField]
		public float kooUsage;
		[KSPField]
		public float foodRate;

		private float growRate = 1.0f;

        // Ticks for food production
        private double tick = 0;
        private double tickMax = 120;
        private double growRateActual = 0;

        public override void OnUpdate()
        {
            if(Events["DeactivateGHModule"].active) // If module is being managed
                UpdateGHModule(); // Check for resources
        }

        private void UpdateGHModule()
        {
            if (isPowered()) // If the module is powered
            {
				if (!(part.RequestResource(Resources.heat, heatUsage * TimeWarp.fixedDeltaTime) < heatUsage * TimeWarp.fixedDeltaTime)) // If total heat energy in this vessel is enough to use...
                {
                    growRateActual += 0.5; // Add full grow rate
                }
                else growRateActual += 0.1; // Receive reduced grow rate

                if (!(part.RequestResource(Resources.pureWater, waterUsage * TimeWarp.fixedDeltaTime) < waterUsage * TimeWarp.fixedDeltaTime)) // If there is enough water source in this vessel...
                {
                    growRateActual += 0.5; // Add full grow rate

					if (!(part.RequestResource(Resources.koo, kooUsage * TimeWarp.fixedDeltaTime) < kooUsage * TimeWarp.fixedDeltaTime)) // If there is enough KO2 source in this vessel...
                    {
                        growRateActual += 0.5; // Greatly increases grow rate
                        part.RequestResource(Resources.oxygen, -kooUsage * TimeWarp.fixedDeltaTime); // Produces oxygen as by-product
                    }
                }
                else growRateActual = 0; // If there is no water, plants cannot grow
            }
            else growRateActual = 0; // If not powered set grow rate to zero

            // Calculate Grow Rate and add it to the tick
            tick += 1.0 * (growRateActual / growRate);
            if(tick > tickMax)
            {
                // Reset the tick, store produced food and oxygen byproduct
                tick = 0;
                this.part.RequestResource(Resources.food, -foodRate * TimeWarp.fixedDeltaTime);
            }

            growRateActual = 0; // Reset the growth rate
        }

        private bool isPowered()
        {
            if (!(part.RequestResource(Resources.power, powerUsage * TimeWarp.fixedDeltaTime) < powerUsage * TimeWarp.fixedDeltaTime))
            {
                return true;
            }
            else
            {
                DeactivateGHModule();
                return false;
            }
        }

        // Below for right-click options and action groups //

        [KSPEvent(guiActive = true, guiName = "Manage Greenhouse")]
        public void ActivateGHModule()
        {
            Events["ActivateGHModule"].active = false;
            Events["DeactivateGHModule"].active = true;
        }

        [KSPEvent(guiActive = true, guiName = "Stop Managing", active = false)]
        public void DeactivateGHModule()
        {
            Events["ActivateGHModule"].active = true;
            Events["DeactivateGHModule"].active = false;
        }
    }
}
