using StardewValley;

namespace LetMeRest.Framework.Lists
{
    public class Buffs
    {
        public static void SetBuff(string buff)
        {
            switch (buff)
            {
                case "Restoring":
                    Buff restoring_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_RestoringStamina");
                    if (restoring_buff == null)
                    {
                        restoring_buff = new Buff("Restoring Stamina.", 0, "LMR_RestoringStamina", 16);
                        restoring_buff.displaySource = "Relaxing";
                        Game1.buffsDisplay.addOtherBuff(restoring_buff);
                    }
                    restoring_buff.millisecondsDuration = 0;
                    break;

                case "Afraid":
                    Buff afraid_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Afraid");
                    if (afraid_buff == null)
                    {
                        afraid_buff = new Buff("What's going on?", 120 * 1000, "LMR_Afraid", 18);
                        Game1.buffsDisplay.addOtherBuff(afraid_buff);
                    }
                    afraid_buff.millisecondsDuration = 120 * 1000;
                    break;

                case "Water":
                    Buff water_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Water");
                    if (water_buff == null)
                    {
                        water_buff = new Buff("Near Water", 0, "LMR_Water", 1);
                        water_buff.displaySource = "Water";
                        Game1.buffsDisplay.addOtherBuff(water_buff);
                    }
                    water_buff.millisecondsDuration = 0;
                    break;

                case "Decoration":
                    Buff decoration_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Decoration");
                    if (decoration_buff == null)
                    {
                        decoration_buff = new Buff("Nice Decoration!", 0, "LMR_Decoration", 24);
                        decoration_buff.displaySource = "Decoration";
                        Game1.buffsDisplay.addOtherBuff(decoration_buff);
                    }
                    decoration_buff.millisecondsDuration = 0 * 1000;
                    break;

                case "Decoration2":
                    Buff decoration2_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Decoration2");
                    if (decoration2_buff == null)
                    {
                        decoration2_buff = new Buff("Aweasome place to live!", 0, "LMR_Decoration2", 24);
                        decoration2_buff.displaySource = "Decoration";
                        Game1.buffsDisplay.addOtherBuff(decoration2_buff);
                    }
                    decoration2_buff.millisecondsDuration = 0 * 1000;
                    break;

                case "Calm":
                    Buff calm_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Calm");
                    if (calm_buff == null)
                    {
                        calm_buff = new Buff(0, 0, 0, 0, 2, 0, 2, 15, 0, 0, 2, 2, 120 * 1000, "LMR_Calm2", "A calm place");
                        calm_buff.sheetIndex = 21;
                        calm_buff.description = "So calm...";
                    }
                    calm_buff.millisecondsDuration = 120 * 1000;
                    break;

                case "Calm2":
                    Buff calm2_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Calm2");
                    if (calm2_buff == null)
                    {
                        calm2_buff = new Buff(0, 0, 0, 0, 5, 0, 5, 35, 0, 0, 5, 5, 120 * 1000, "LMR_Calm2", "An aweasome location!");
                        calm2_buff.sheetIndex = 21;
                        calm2_buff.description = "Wow that's so beautiful!";
                        Game1.buffsDisplay.addOtherBuff(calm2_buff);
                    }
                    calm2_buff.millisecondsDuration = 120 * 1000;
                    break;
            }
        }
    }
}
