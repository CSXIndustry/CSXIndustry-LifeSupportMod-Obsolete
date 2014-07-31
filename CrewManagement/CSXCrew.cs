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

namespace CSXIndustry.CrewManagement
{
    public class CSXCrew
    {
		private double eatTimerMax = 8 * 3600;
        private double eatTimer = 8 * 3600; // Timer that controls eating
		private double wasteTimer = 4 * 3600;
        private bool hasEaten = true; // Boolean that checks if this crew has eaten

        private double killTimer = (8 * 3600) * (30 * 3); // Timer that checks if this Kerman needs to die
		// 8 hours, 3 meals per day, 30 days, total 90

        private ProtoCrewMember crew; // Detail of this crew

        public CSXCrew(ProtoCrewMember crew)
        {
            // Set this crew
            this.crew = crew;
        }

        public void Update()
        {
            // Deduct eat timer
            eatTimer -= 1.0 * TimeWarp.fixedDeltaTime;
			wasteTimer -= 1.0 * TimeWarp.fixedDeltaTime;

            // If it's time to eat
            if(eatTimer < 1)
            {
                hasEaten = false; // Set eaten to false
            }
        }

        // All Get & Set functions
		public bool Eaten
		{
			get { return this.hasEaten; }
			set { this.hasEaten = value; }
		}

		public double Eat
		{
			get { return this.eatTimer; }
			set { this.eatTimer = value; }
		}

		public double MaxEat
		{
			get { return this.eatTimerMax; }
		}

		public double Kill
		{
			get { return this.killTimer; }
			set
			{
				this.killTimer = this.killTimer - (this.eatTimerMax - (this.eatTimerMax * value));
			}
		}

		public double Waste
		{
			get { return this.wasteTimer; }
			set { this.wasteTimer = 4 * 3600; }
		}

        public ProtoCrewMember Crew
		{
			get { return this.crew; }
			set { this.crew = value; }
		}

		public void SetKillTimer(double value)
		{
			this.killTimer = value;
		}
    }
}
