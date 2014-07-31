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
    public class CSXFuelCell : CSXPartModule
    {
		[KSPField]
		public float reactionRate;

        private PartResource hydrogen; // Fuel cell main fuel
        private PartResource oxygen; // Fuel cell main fuel
        private PartResource water; // Cooling and Future usage for: GreenhouseMoudle
        private PartResource heat; // Future usage for: GreenhouseModule

        [KSPField(guiActive = true, guiName = "Cell Temperature ", guiUnits = "C")]
        private float cellTemp = 1000f; // Fuel Cell temperature

        public override void OnStart(PartModule.StartState state)
        {
            if(state != StartState.Editor) // When flight starts
            {
                // Set PartResource to variables
                this.hydrogen = this.part.Resources[Resources.hydrogen];
                this.oxygen = this.part.Resources[Resources.oxygen];
                this.water = this.part.Resources[Resources.byWater];
                this.heat = this.part.Resources[Resources.heat];

                // Set initial fuel cell temperature
                this.part.temperature = cellTemp;
            }
        }

        public void FixedUpdate()
        {
            if (UpdateCell()) // Update Fuel Cell and check if it's running or not
            {
                // If running, get number of parts that requires electrical charges
                List<Part> required = GetChargeRequired();

                if (required.Count > 0) // If there is at least one part that required charging
                    foreach (Part part in required) // For each part that requires charging
                        if(part.Resources[Resources.power].amount < part.Resources[Resources.power].maxAmount)
                            part.RequestResource(Resources.power, ((-0.24 * reactionRate) / (double) required.Count) * TimeWarp.fixedDeltaTime); // Distribute these powers to them

                // If water tank is not full, fill them up too
                this.part.RequestResource(Resources.byWater, -1.0 * TimeWarp.fixedDeltaTime);

                // If heat storage is not full, fill them up too
				if (this.part.RequestResource(Resources.heat, (-10.0 * reactionRate) * TimeWarp.fixedDeltaTime) == 0)
                    cellTemp += 10f * TimeWarp.fixedDeltaTime; // If heat storage is full, then fuel cell's temperature increases
            }
            else // If Fuel Cell is not running
                if (cellTemp > 1000f) // If Fuel Cell is overheated
                    cellTemp -= 1.0f * TimeWarp.fixedDeltaTime; // Cool it down over time

            if (UpdateCooling()) // If cooling is activated
                if (cellTemp > 1000f) // And cell is overheated
                    cellTemp -= 15.0f * TimeWarp.fixedDeltaTime; // Cool it down

            if (oxygen.amount <= 0 || hydrogen.amount <= 0) // If either oxygen or hydrogen tank is empty
                DeactivateCell(); // Stop the Fuel Cell

            if (water.amount <= 0 || cellTemp <= 1000f) // If there is no water in water tank or the Fuel Cell is at it's normal temperature
                DeactivateCooling(); // Stop the cooling

            this.part.temperature = (int) cellTemp; // Set Fuel Cell's temperature
        }

        private bool UpdateCell()
        {
            if (Events["DeactivateCell"].active) // If Fuel Cell is activated
                if (oxygen.amount > 0 && hydrogen.amount > 0) // If both tank have enough fuel to generate power
                {
                    // Use fuel and return true
					part.RequestResource(Resources.oxygen, (1.0 * reactionRate) * TimeWarp.fixedDeltaTime);
					part.RequestResource(Resources.hydrogen, (2.0 * reactionRate) * TimeWarp.fixedDeltaTime);
                    return true;
                }
                else return false; // Else return false; Fuel Cell has not enough fuel to generate power

            return false; // If not just return false; Fuel Cell is not activated
        }

        private bool UpdateCooling()
        {
            if (Events["DeactivateCooling"].active) // If cooling is activated
                if (water.amount > 0) // If there is enough water to run cooling
                {
                    // Use water and return true
					part.RequestResource(Resources.byWater, (2.0 * reactionRate) * TimeWarp.fixedDeltaTime);
                    return true;
                }
                else return false; // Else return false; there is not enough water to run cooling

            return false; // If cooling is not running return false; Cooling is not activated
        }

        private List<Part> GetChargeRequired()
        {
            List<Part> list = new List<Part>();

            foreach(Part part in this.vessel.parts) // For each part in entire vessel
                foreach(PartResource charge in part.Resources) // And for each resource in the part
                    if(charge.resourceName == Resources.power && (charge.amount < charge.maxAmount)) // If it is electric charge and is not full
                        list.Add(part); // Add it to the list

            return list; // Return the list
        }

        // Below for right-click options and action groups //

        [KSPEvent(guiActive = true, guiName = "Activate Fuel Cell")]
        public void ActivateCell()
        {
            ScreenMessages.PostScreenMessage("Fuel Cell : On", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            Events["ActivateCell"].active = false;
            Events["DeactivateCell"].active = true;
        }

        [KSPEvent(guiActive = true, guiName = "Deactivate Fuel Cell", active = false)]
        public void DeactivateCell()
        {
            ScreenMessages.PostScreenMessage("Fuel Cell : Off", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            Events["ActivateCell"].active = true;
            Events["DeactivateCell"].active = false;
        }

        [KSPEvent(guiActive = true, guiName = "Activate Cooling")]
        public void ActivateCooling()
        {
            Events["ActivateCooling"].active = false;
            Events["DeactivateCooling"].active = true;
        }

        [KSPEvent(guiActive = true, guiName = "Deactivate Cooling", active = false)]
        public void DeactivateCooling()
        {
            Events["ActivateCooling"].active = true;
            Events["DeactivateCooling"].active = false;
        }

        [KSPAction("Toggle Fuel Cell")]
        public void FuelCellAction(KSPActionParam param)
        {
            if (param.type == KSPActionType.Activate)
                ActivateCell();
            else if (param.type == KSPActionType.Deactivate)
                DeactivateCell();
        }

        [KSPAction("Toggle Cooling")]
        public void CoolingAction(KSPActionParam param)
        {
            if (param.type == KSPActionType.Activate)
                ActivateCooling();
            else if (param.type == KSPActionType.Deactivate)
                DeactivateCooling();
        }

		public override void CallFromTop()
		{
			Debug.Log("[CSX_Ind] CSXFuelCell.cs overriding CSXPartModule method CallFromTop()");
		}
    }
}
