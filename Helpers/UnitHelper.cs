﻿using Game.SceneFlow;
using Game.Settings;

namespace DetailedDescriptions.Helpers
{
    public static class UnitHelper
    {
        /// <summary>
        /// Convert length units (8 meters) to the user selected unit system
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatLength(int value)
        {
            if (IsMetric())
            {
                return $"{8*value}m";
            }

            var feet = MetersToFeet(8*value);
            // 2 decimal places
            string roundedFeet = $"{feet:0.00}ft";
            return roundedFeet;
        }

        public static string FormatSpeedLimit(float value)
        {
            if (IsMetric())
            {
                return $"{MsToKph(value)} km/h";
            }
            return $"{MsToMph(value)} mph";
        }
        
        public static int MsToKph(float value)
        {
            return (int)(value * 3.6);
        }
        
        public static int MsToMph(float value)
        {
            return (int)(value * 2.23694);
        }
    
        public static double FeetToMeters(double value)
        {
            return value * 0.3048;
        }

        public static double MetersToFeet(double value)
        {
            return value / 0.3048;
        }
    
        private static bool IsMetric()
        {
            return GameManager.instance?.settings?.userInterface?.unitSystem is null or InterfaceSettings.UnitSystem.Metric;
        }
    }
}