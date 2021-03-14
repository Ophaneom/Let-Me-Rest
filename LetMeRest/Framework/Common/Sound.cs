using StardewValley;

namespace LetMeRest.Framework.Common
{
    public class Sound
    {
        public static void PlaySound(string soundName)
        {
            if (!Check.playingSound)
            {
                Check.playingSound = true;
                Game1.playSound(soundName);
            }
        }
    }
}
