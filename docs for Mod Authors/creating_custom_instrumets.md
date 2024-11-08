# Creating Custom Instruments with CP
The Playable Piano Mod allows adding musical functionality to Items using Content Patcher. The Instruments can be either played in Freeplay Mode or MIDI Playback Mode.
There are two types of instruments which can be added, placable furniture (e.g. a Piano) and portable Objects (e.g. an Ocarina).

Placable Instruments require the player to sit down at them, in order to be playable. Internally they thus should have a furniture type with seats (chair, bench, couch, armchair).

Portable Instruments have no restrictions besides not being placable. Ideally they should just be normal, non-edible Objects.

## The Sound
Every Instrument needs atleast a base sound and can have 2 optional sounds for extended Pitch ranges (one high pitch sound and one low pitch). The Mod comes with a Piano sound, and extra sounds for the `toyPiano` and `flute` from the base game, which you can use, if you don't want to add your own. 

### Base sound
The base sound can either be a sound from the [base game](https://stardewvalleywiki.com/Modding:Audio#Sound) or can be a custom sound added by a mod (for more information on adding custom audio visit the [Stardew Valley Wiki Page](https://stardewvalleywiki.com/Modding:Audio) on Modding Audio).
The Pitch of the sound should be a C, so the keyboard layout matches a Piano keyboard. For optimal MIDI Playback it should be C6 specifically, since the midi player assumes, that the base sound has that pitch.

### Low and High Pitched Sound
You can add a low pitched and a high pitched version of your base sound to your Content Pack. They should be pitched 2 octaves below/above the base sound and will expand the pitch range of your Instrument by 2 Octaves in the respective direction. 

The `ID` of the sounds should be identical with the Id of your base sound, with `Low` and/or `High` added to the end respectively. If you for example have a baseSound named `{{ModId}}_CatPianoSound` then the low Pitched version has to be named `{{ModId}}_CatPianoLow` and the high Pitched Version `{{ModId}}_CatPianoHigh`. **This is case-sensitive, so make sure that the L and H of Low and High are capitalized**

## Adding the sound to your Instrument
Once you have added your sound and your Instrument to your Pack, you have to tell the Playable Piano Mod, which sound your instrument uses. To do this you have to add the following Trigger Action to your Content Pack. 
```jsonc
{
  "Action": "EditData",
  "Target": "Data/TriggerActions",
  "Entries": {
    "{{ModId}}_LoadInstruments": {
      "Id": "{{ModId}}_LoadInstruments",
      "Trigger": "Mushroomy.PlayablePiano_SaveLoaded",
      "Actions": [ //if you only have a single Instrument you can use the "Action" syntax instead of "Actions"
        "Mushroomy.PlayablePiano_AddSound <Name/ItemId of your Instrument> <ID of your sound>"
        ... // If you have multiple Instruments you have to add a line for every one of them
      ],
      "MarkActionApplied": false // If you don't set this to false, your instrument might not properly work when loading the game after a save
    }
  }
}
```
Congratulations, your Instrument is done and you can play it. For stationary Instruments sit down in front of them and press a Button, for portable hold them in your hands and left click.
