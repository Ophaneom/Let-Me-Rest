using StardewModdingAPI;
using StardewValley;
using LetMeRest.Framework.Lists;

namespace LetMeRest.Framework.Common
{
    public class Status
    {
        private static int radius = 6;

        public static void IncreaseStamina(float value, float secretMultiplier)
        {
            if (Game1.player.Stamina < Game1.player.MaxStamina)
            {
                // Convert PerSecond multiplier value to PerTick
                value /= 60;

                // Get multipliers
                float decorationMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[0];
                float waterMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[1];
                float paisageMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[2];

                // Increases stamina in multiplayer
                if (Context.IsMultiplayer)
                {
                    Game1.player.Stamina += (value * ModEntry.data.Multiplier) *
                        ((decorationMultiplier * 1.25f) * ModEntry.data.Multiplier) *
                        (waterMultiplier * ModEntry.data.Multiplier) *
                        (paisageMultiplier * ModEntry.data.Multiplier) *
                        (secretMultiplier * ModEntry.data.Multiplier);
                }
                // Increases stamina in singleplayer
                else
                {
                    Game1.player.Stamina += (value * ModEntry.config.Multiplier) *
                        ((decorationMultiplier * 1.25f) * ModEntry.config.Multiplier) *
                        (waterMultiplier * ModEntry.config.Multiplier) *
                        (paisageMultiplier * ModEntry.config.Multiplier) *
                        (secretMultiplier * ModEntry.config.Multiplier);
                }

                Buffs.SetBuff("Restoring");
            }
        }
    }
}
