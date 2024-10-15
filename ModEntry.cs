using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;


namespace Playable_Piano
{
    enum Notes : int
    {
        // lower Octave
        Z = 0,
        S = 100,
        X = 200,
        D = 300,
        C = 400,
        V = 500,
        G = 600,
        B = 700,
        H = 800,
        N = 900,
        J = 1000,
        M = 1100,
        
        // uppper Octave
        Q  = 1200,
        D2 = 1300,
        W  = 1400,
        D3 = 1500,
        E  = 1600,
        R  = 1700,
        D5 = 1800,
        T  = 1900,
        D6 = 2000,
        Y  = 2100,
        D7 = 2200,
        U  = 2300,
        I  = 2400
    }



    internal sealed class PlayablePiano : Mod
    {
        string[] pianos = {"Dark Piano", "Upright Piano"};
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            //this.Monitor.Log($"{e.Button.ToString()}",LogLevel.Debug);
            if (Game1.player.IsSitting())
            {
                GameLocation location = Game1.currentLocation;
                Farmer player = Game1.player;
                string? tile_name = location.getObjectAtTile((int)player.Tile.X, (int)player.Tile.Y).displayName;
                if (tile_name == null) { return; }

                // check if player is sitting at a Piano
                if (Array.Exists(pianos, x => x == tile_name)) 
                {
                    Notes played_note;
                    if (Notes.TryParse(e.Button.ToString(), out played_note))
                    {
                        this.Helper.Input.Suppress(e.Button);
                        int pitch = (int) played_note;
                        location.playSound("toyPiano", player.Tile, pitch);
                    }
                }
                //this.Monitor.Log($"sitting at {location.getObjectAtTile((int)player.Tile.X, (int)player.Tile.Y).displayName}", LogLevel.Debug);
                
            }
        }
    }
}
