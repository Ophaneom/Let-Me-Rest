﻿using System;
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
    class AmbientInformation
    {
        public static float[] Infos(int decorationRadius, Dictionary<string,float> ItemDataBase)
        {
            // Player position
            Vector2 posPlayerTile = new Vector2(Game1.player.getTileX(), Game1.player.getTileY());
            // Actual tile
            int actualTileIndex = Game1.player.currentLocation.getTileIndexAt((int)posPlayerTile.X, (int)posPlayerTile.Y, "Back");
            int actualTileIndexBuildings = Game1.player.currentLocation.getTileIndexAt((int)posPlayerTile.X, (int)posPlayerTile.Y, "Buildings");


            // Check decoration around player
            float decorationMultiplier = 1;

            int virtualRadius;
            if (Game1.player.currentLocation.Name == "FarmHouse" ||
                Game1.player.currentLocation.Name == "Cabin")
            {
                virtualRadius = decorationRadius + 10;
            }
            else virtualRadius = decorationRadius;

            for (var y = posPlayerTile.Y - virtualRadius; y < posPlayerTile.Y + virtualRadius; y++)
            {
                for (var x = posPlayerTile.X - virtualRadius; x < posPlayerTile.X + virtualRadius; x++)
                {
                    StardewValley.Object obj = Game1.currentLocation.getObjectAtTile((int)x, (int)y) ?? null;

                    if (obj != null)
                    {
                        if (obj.getCategoryName() == "Furniture" || obj.getCategoryName() == "Decoration")
                        {
                            float itemValue;
                            if (ItemDataBase.TryGetValue(obj.Name, out itemValue))
                            {
                                decorationMultiplier += itemValue;
                            }
                        }
                    }
                }
            }


            // Check water around player
            float waterMultiplier = 1;
            float waterRadius = 3;

            for (var y = posPlayerTile.Y - waterRadius; y < posPlayerTile.Y + waterRadius; y++)
            {
                for (var x = posPlayerTile.X - waterRadius; x < posPlayerTile.X + waterRadius; x++)
                {
                    if (Game1.player.currentLocation.isWaterTile((int)x, (int)y))
                    {
                        waterMultiplier = 1.5f;
                        break;
                    }
                }
            }


            // Reset paisage multiplier
            float paisageMultiplier = 1;


            // Check if player are below the pink tree in Town, Mountain and Forest
            if (Game1.player.currentLocation.Name == "Town" ||
                Game1.player.currentLocation.Name == "Mountain" ||
                Game1.player.currentLocation.Name == "Forest")
            {
                float pinkTreeRadius = 5;

                for (var y = posPlayerTile.Y - pinkTreeRadius; y < posPlayerTile.Y + pinkTreeRadius; y++)
                {
                    for (var x = posPlayerTile.X - pinkTreeRadius; x < posPlayerTile.X + pinkTreeRadius; x++)
                    {
                        int TileId = Game1.player.currentLocation.getTileIndexAt((int)x, (int)y, "Buildings");

                        if (TileId == 143 || TileId == 144 ||
                            TileId == 168 || TileId == 169)
                        {
                            paisageMultiplier = 3f;
                            startSound();
                            break;
                        }
                    }
                }
            }


            // Town information check
            if (Game1.player.currentLocation.Name == "Town")
            {
                // Check if player are above a bridge in Town
                if (actualTileIndex == 909)
                {
                    paisageMultiplier = 2.25f;
                }
                // Check if player are above little bridges in Town
                if (actualTileIndex == 779 || actualTileIndex == 780 ||
                    actualTileIndex == 781 || actualTileIndex == 782)
                {
                    paisageMultiplier = 2.25f;
                }
            }


            // Mountain information check
            if (Game1.player.currentLocation.Name == "Mountain")
            {
                // Check if player are above little bridges in Mountain
                if (actualTileIndexBuildings == 779 || actualTileIndexBuildings == 780 ||
                    actualTileIndexBuildings == 781 || actualTileIndex == 782)
                {
                    paisageMultiplier = 2.25f;
                }
                if (actualTileIndexBuildings == 809 ||
                    actualTileIndexBuildings == 834 ||
                    actualTileIndexBuildings == 884)
                {
                    paisageMultiplier = 2.25f;
                }
            }


            // Beach information check
            if (Game1.player.currentLocation.Name == "Beach")
            {
                // Check if player are above a pier in Beach
                if (actualTileIndex == 419 || actualTileIndex == 420 ||
                    actualTileIndex == 421 || actualTileIndex == 470 ||
                    actualTileIndex == 471 || actualTileIndex == 487 ||
                    actualTileIndex == 488 || actualTileIndex == 489 ||
                    actualTileIndex == 504 || actualTileIndex == 505 ||
                    actualTileIndex == 506)
                {
                    paisageMultiplier = 2.25f;
                }
            }


            // Forest information check
            if (Game1.player.currentLocation.Name == "Forest")
            {
                // Check if player are below the pier in Forest
                if (actualTileIndex == 1637 || actualTileIndex == 1638 ||
                    actualTileIndex == 1639 || actualTileIndex == 1662 ||
                    actualTileIndex == 1663 || actualTileIndex == 1664 ||
                    actualTileIndex == 1687 || actualTileIndex == 1688 ||
                    actualTileIndex == 1689)
                {
                    paisageMultiplier = 2.25f;
                }

                // Check if player are below little bridges in Forest
                if (actualTileIndexBuildings == 809 || actualTileIndexBuildings == 859 ||
                    actualTileIndexBuildings == 860 || actualTileIndexBuildings == 885 ||
                    actualTileIndexBuildings == 910 || actualTileIndexBuildings == 884)
                {
                    paisageMultiplier = 2.25f;
                }
            }


            // Woods information check
            if (Game1.player.currentLocation.Name == "Woods")
            {
                // Add multiplier in woods
                paisageMultiplier = 1.5f;

                // Check if player are around the statue in Woods
                float borderRadius = 5;

                for (var y = posPlayerTile.Y - borderRadius; y < posPlayerTile.Y + borderRadius; y++)
                {
                    for (var x = posPlayerTile.X - borderRadius; x < posPlayerTile.X + borderRadius; x++)
                    {
                        int TileId = Game1.player.currentLocation.getTileIndexAt((int)x, (int)y, "Buildings");

                        if (TileId == 1140 || TileId == 1141)
                        {
                            paisageMultiplier = 3.5f;
                            startSound();
                            break;
                        }
                    }
                }
            }


            // Desert information check
            if (Game1.player.currentLocation.Name == "Desert")
            {
                // Add multiplier in desert
                paisageMultiplier = 1.5f;
            }


            // Check if player are in the border of Island North
            if (Game1.player.currentLocation.Name == "IslandNorth")
            {
                float borderRadius = 5;

                for (var y = posPlayerTile.Y - borderRadius; y < posPlayerTile.Y + borderRadius; y++)
                {
                    for (var x = posPlayerTile.X - borderRadius; x < posPlayerTile.X + borderRadius; x++)
                    {
                        int TileId = Game1.player.currentLocation.getTileIndexAt((int)x, (int)y, "Buildings");

                        if (TileId == 832 || TileId == 833 ||
                            TileId == 834 || TileId == 835 ||
                            TileId == 864 || TileId == 869 ||
                            TileId == 896 || TileId == 900 ||
                            TileId == 901)
                        {
                            paisageMultiplier = 3f;
                            startSound();
                            break;
                        }
                    }
                }
            }


            // Island East information check
            if (Game1.player.currentLocation.Name == "IslandEast")
            {
                // Add multiplier in desert
                paisageMultiplier = 1.5f;
            }


            float[] send = new float[]
            {
                decorationMultiplier,
                waterMultiplier,
                paisageMultiplier
            };

            return send;
        }

        private static void startSound()
        {
            if (!ModEntry.playingSound)
            {
                ModEntry.playingSound = true;
                //Game1.playSound("wind");
            }
        }
    }
}
