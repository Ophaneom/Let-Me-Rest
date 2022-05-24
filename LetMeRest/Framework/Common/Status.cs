using System;
using StardewModdingAPI;
using StardewValley;
using LetMeRest.Framework.Lists;

namespace LetMeRest.Framework.Common
{
    public class Status
    {
        private static int radius = 6;
        public static bool canUpdateQuantity;
        private static float actualQuantity;
        private static int divideByCaveValues = 1;

        public static void IncreaseStamina(float value, float secretMultiplier)
        {
            if (Game1.player.Stamina < Game1.player.MaxStamina)
            {
                value /= 60;

                if (Context.IsMultiplayer)
                {
                    if (canUpdateQuantity)
                    {
                        float decorationMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[0];
                        float waterMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[1];
                        float paisageMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[2];

                        actualQuantity = ((value * ModEntry.data.Multiplier) *
                        ((decorationMultiplier * 1.25f) * ModEntry.data.Multiplier) *
                        (waterMultiplier * ModEntry.data.Multiplier) *
                        (paisageMultiplier * ModEntry.data.Multiplier) *
                        (secretMultiplier * ModEntry.data.Multiplier)) / divideByCaveValues;

                        if (InCave()) divideByCaveValues = 2;
                        else divideByCaveValues = 1;

                        canUpdateQuantity = false;
                    }

                    Game1.player.Stamina += actualQuantity;
                }
                else
                {
                    if (canUpdateQuantity)
                    {
                        float decorationMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[0];
                        float waterMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[1];
                        float paisageMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[2];

                        if (InCave()) divideByCaveValues = 2;
                        else divideByCaveValues = 1;

                        actualQuantity = ((value * ModEntry.config.Multiplier) *
                        ((decorationMultiplier * 1.25f) * ModEntry.config.Multiplier) *
                        (waterMultiplier * ModEntry.config.Multiplier) *
                        (paisageMultiplier * ModEntry.config.Multiplier) *
                        (secretMultiplier * ModEntry.config.Multiplier)) / divideByCaveValues;

                        canUpdateQuantity = false;
                    }
                    
                    Game1.player.Stamina += actualQuantity;
                }

                if (Context.IsMultiplayer)
                {
                    
                    if (ModEntry.data.EnableBuffs)
                    {
                        Buffs.SetBuff("Restoring");
                        if (InCave()) Buffs.SetBuff("Afraid");
                    }
                }
                else
                {
                    if (ModEntry.config.EnableBuffs)
                    {
                        Buffs.SetBuff("Restoring");
                        if (InCave()) Buffs.SetBuff("Afraid");
                    }
                }
            }
        }

        public static bool InCave()
        {
            string s = Game1.player.currentLocation.Name;

            string[] possibleLocationNames = new string[]
            {
                "UndergroundMine",
                "VolcanoDungeon",
            };

            foreach (string locationName in possibleLocationNames)
            {
                if (s.Contains(locationName))
                {
                    string[] location = s.Split(new string[] { locationName }, StringSplitOptions.None);

                    return true;
                }
            }

            return false;
        }
    }
}
