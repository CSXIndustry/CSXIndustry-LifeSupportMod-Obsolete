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
using System;
using System.Collections.Generic;

using CSXIndustry.CrewManagement;
using CSXIndustry.ModuleManagement;

namespace CSXIndustry.EnvManagement
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class CSXECLSS : MonoBehaviour
    {
        private Vessel activeVessel;
        private List<CSXCrew> crews;

		private List<CSXPartModule> parts;

        public CSXECLSS()
        {
            Debug.Log("[CSX_Ind] CSX Industry ECLSS Control Plugin Detected.");
        }

        public void Start()
        {
            this.activeVessel = FlightGlobals.ActiveVessel;
            Initialize();
        }

        public void FixedUpdate()
        {
            if(crews.Count > 0) // If there IS at least one crew
            {
                CrewUpdate();
            }

			PartUpdate();
        }

        private void CrewUpdate()
        {
            // For every update, update each crew
            foreach (CSXCrew crew in crews)
            {
                crew.Update();

                // Consume oxygen
                double inhaled = activeVessel.rootPart.RequestResource(Resources.oxygen, 0.1 * TimeWarp.fixedDeltaTime);
                // Produce carbon dioxide
                activeVessel.rootPart.RequestResource(Resources.koo, -inhaled * TimeWarp.fixedDeltaTime);

				double toxicLevel = inhaled / (0.1 * TimeWarp.fixedDeltaTime);
				crew.SetKillTimer(crew.Kill - (100 - (100 * toxicLevel)));
				if (crew.Kill < 0) // If this Kerman's time has come
				{
					KillKerman(crew.Crew, "low oxygen level");
				}

                // If this Kerman hasn't eaten yet
                if(!crew.Eaten)
                {
                    // Check with food
                    double amount = activeVessel.rootPart.RequestResource(Resources.food, 1.0 * TimeWarp.fixedDeltaTime);
					double waterAmount = activeVessel.rootPart.RequestResource(Resources.pureWater, 1.0 * TimeWarp.fixedDeltaTime);
                    double factor = amount / (1.0 * TimeWarp.fixedDeltaTime);

					crew.Kill = factor;

					crew.Eaten = true;
					crew.Eat = crew.MaxEat;

					if (crew.Kill < 0) // If this Kerman's time has come
					{
						KillKerman(crew.Crew, "hunger");
					}
                }

                if(crew.Crew == null)
                    crews.Remove(crew);

				if(crew.Waste < 0)
				{
					activeVessel.rootPart.RequestResource(Resources.waste, -2.0 * TimeWarp.fixedDeltaTime);
					activeVessel.rootPart.RequestResource(Resources.wasteWater, -0.5 * TimeWarp.fixedDeltaTime);
					crew.Waste = 0;
				}
            }
        }

		private void PartUpdate()
		{
			foreach(CSXPartModule module in parts)
			{
				if (module.partType != (int)CSXPartType.NONE)
				{
					Debug.Log("[CSX_Ind] Calling virtual method CallFromTop()");
					module.CallFromTop();
				}
				else if(module.partType == (int)CSXPartType.NONE)
				{
					Debug.Log("[CSX_Ind] Calling non-related part");
				}
			}
		}

        private void Initialize()
        {
            Debug.Log("[CSX] Initializing Crew List...");
            crews = new List<CSXCrew>();
            foreach(ProtoCrewMember crew in activeVessel.GetVesselCrew())
            {
                crews.Add(new CSXCrew(crew));
                Debug.Log("[CSX_Ind] " + crew.name + " has been added to crew list");
            }

			parts = new List<CSXPartModule>();
			foreach (Part part in activeVessel.Parts)
				foreach (PartModule module in part.Modules)
				{
					CSXPartModule type = GetModule(module);

					if (type != null && (type.partType != (int)CSXPartType.NONE))
					{
						type.CSXPart = part;
						type.CSXModuleName = module.moduleName;
						type.CSXModule = module;
						parts.Add(type);
						Debug.Log("[CSX_Ind] The part " + part.name + " has been added to part List");
					}
					else if (type != null && (type.partType == (int)CSXPartType.NONE))
						Debug.Log("[CSX_Ind] Module " + module.moduleName + " has been skipped");
				}
        }

		private void KillKerman(ProtoCrewMember target, string reason)
		{
			ScreenMessages.PostScreenMessage(target.name + " has died due to " + reason, 5.0f, ScreenMessageStyle.UPPER_CENTER);
			activeVessel.GetVesselCrew().Remove(target);
		}

		private CSXPartModule GetModule(PartModule module)
		{
			try
			{
				return (CSXPartModule)module;
			}
			catch(Exception e)
			{
				Debug.Log("[CSX_Ind] Skipped " + module.moduleName);
				e.ToString();
				return null;
			}
		}
    }
}
