# Creating Custom Instruments with CP or API
The Playable Piano Mod allows adding musical functionality to Items using Content Patcher or by opening the Mod's UI directly via the API. The Instruments can be either played in Freeplay Mode or MIDI Playback Mode.

## Content Patcher
There are two types of instruments which can be added, placeable furniture (e.g. a Piano) and portable Objects (e.g. an Ocarina).

Placeable Instruments require the player to sit down at them, in order to be playable. Internally they thus should have a furniture type with seats (chair, bench, couch, armchair).

Portable Instruments have no restrictions besides not being placeable. Ideally they should just be normal, non-edible Objects.

## Adding the sound to your Instrument
In order to play the Instrument, Playable Piano needs to know, which sound it should use. You can either use one of the sounds which comes packed in with the Mod, a sound from the base game, or add your own sound. For your own custom Sounds, **please read the section Custom *Sound* at the bottom**. To tell the Mod which sounds it should use, you have to use the `Mushroom.PlayablePiano_AddSound` Action. While it is not relevant, which Trigger is used to cause the Action, for simplicity the Mod comes with the `Mushroomy.PlayablePiano_SaveLoaded` Trigger, which (as the name suggests) triggers after the save gets loaded, making the Instrument directly playable. 
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
      "MarkActionApplied": false // This is optional, but if you don't set this to false, changes to this Trigger Action in the future won't be automatically applied.  
    }
  }
}
```
Congratulations, your Instrument is done and you can play it. For stationary Instruments sit down in front of them and press a Button, for portable hold them in your hands and left click.
# Creating Custom Instruments with the API
In case you can't create an Instrument with Content Patcher, f.e. because it is part of the map and it doesn't make sense to sit down to play it, or if you just want to have more control where and how the Mod's UI gets opened, you can use the API.

The API consists of a single method `playInstrument(string baseSoundName)`. Calling this method will open the Mod's UI for playing an Instrument, with the corresponding sound given. `baseSoundName` has to be the name of a sound Cue added either directly via adding a Sound Cue through Code, or by using Content Patcher to edit `Data/AudioChanges`.

# Custom Sound
Every Instrument needs atleast a base sound and can have 2 optional sounds for extended Pitch ranges (one high pitch sound and one low pitch). The Mod comes with a Piano sound `Mushroomy.PlayablePiano_Piano`, and additional sounds for the `toyPiano` and `flute` sounds from the base game, which you can use, if you don't want to add your own. 

## Base sound
The base sound can either be a sound from the [base game](https://stardewvalleywiki.com/Modding:Audio#Sound) or can be a custom sound added by a mod (for more information on adding custom audio visit the [Stardew Valley Wiki Page](https://stardewvalleywiki.com/Modding:Audio) on Modding Audio).
The Pitch of the sound should be a C, so the keyboard layout matches a Piano keyboard. For optimal MIDI Playback it should be C6 specifically, since the midi player assumes, that the base sound has that pitch.

## Low and High Pitched Sound
You can add a low pitched and a high pitched version of your base sound to your Content Pack. They should be pitched 2 octaves below/above the base sound and will expand the pitch range of your Instrument by 2 Octaves in the respective direction. 

The `ID` of the sounds should be identical with the Id of your base sound, with `Low` and/or `High` added to the end respectively. If you for example have a baseSound named `{{ModId}}_CatPianoSound` then the low Pitched version has to be named `{{ModId}}_CatPianoLow` and the high Pitched Version `{{ModId}}_CatPianoHigh`. **This is case-sensitive, so make sure that the L and H of Low and High are capitalized**