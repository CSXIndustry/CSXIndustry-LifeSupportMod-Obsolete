/************************************************************************
 * CSX Industry - Life Support Part+Plugin Pack for Kerbal Space Program*
 *                                                                      *
 * Initial Alpha Release Version 0.6a                                   *
 *                                                                      *
 * Created by Charlie S.                                                *
 * Built on July 26th, 2014                                             *
 * Initial Built on July 12th, 2014                                     *
 ************************************************************************/

using CSXIndustry.ModuleManagement;
using UnityEngine;

namespace CSXIndustry
{
    public class CSXResource : CSXPartModule
    {
        public override string GetInfo()
        {
            return "CSX Industry Resource Manager";
        }

		public override void CallFromTop()
		{
			Debug.Log("[CSX_Ind] CSXResource.cs overriding CSXPartModule method CallFromTop()");
		}
    }
}
