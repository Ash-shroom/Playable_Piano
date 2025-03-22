namespace Playable_Piano
{
    public class PianoApi
    {
        readonly internal PlayablePiano mainMod;
        internal PianoApi(PlayablePiano mod)
        {
            mainMod = mod;
        }
        public void playInstrument(string baseSoundName)
        {
            mainMod.openInstrumentMenu(baseSoundName);
        }
    }
}