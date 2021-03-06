using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace LetMeRest
{
    class Secrets
    {
        public static float CheckForSecrets()
        {
            Vector2 posPlayerTile = new Vector2(Game1.player.getTileX(), Game1.player.getTileY());
            StardewValley.Object actualObject = Game1.currentLocation.getObjectAtTile((int)posPlayerTile.X, (int)posPlayerTile.Y);

            if (Game1.player.hat.Value != null && actualObject != null)
            {
                string hatName = Game1.player.hat.Value.Name;

                if (actualObject.Name == "Dark Throne" && hatName == "Wizard Hat")
                {
                    return 1.4f;
                }
                if (actualObject.Name == "Green Stool" || actualObject.Name == "Blue Stool")
                {
                    if (hatName == "Propeller Hat")
                        return 1.35f;
                }
                if (actualObject.Name == "Pink Plush Seat" && hatName == "Pink Bowl")
                {
                    return 1.20f;
                }
                if (actualObject.Name == "King Chair" && hatName == "Top Hat")
                {
                    return 1.5f;
                }
                if (actualObject.Name == "Cute Chair")
                {
                    if (hatName == "Polka Bow" || hatName == "Delicate Bow" || hatName == "Butterfly Bow")
                        return 1.20f;
                }
                if (actualObject.Name == "Tropical Chair")
                {
                    if (hatName == "Living Hat" || hatName == "Totem Mask")
                        return 1.35f;
                }
                if (actualObject.Name == "Winter Chair" && hatName == "Earmuffs")
                {
                    return 1.20f;
                }
            }

            return 1;
        }
    }
}
