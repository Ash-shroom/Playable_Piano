# Creating Custom Instruments with CP
The Playable Piano Mod allows adding musical functionality to Items using Content Patcher. The Instruments can be either played in Freeplay Mode or MIDI Playback Mode.
There are two types of instruments which can be added, placable furniture (e.g. a Piano) and portable Objects (e.g. an Ocarina).

Placable Instruments require the player to sit down at them, in order to be playable. Internally they thus should have a furniture type with seats (chair, bench, couch, armchair).

Portable Instruments have no restrictions besides not being placable. Ideally they should just be normal, non-edible Objects.

## The Sound
Every Instrument needs atleast a base sound and can have 2 optional sounds for extended Pitch ranges (one high pitch sound and one low pitch).

### Base sound
The base sound can either be a sound from the [base game](https://stardewvalleywiki.com/Modding:Audio#Sound) or can be a custom sound added by a mod (for more information on adding custom audio visit the [Stardew Valley Wiki Page](https://stardewvalleywiki.com/Modding:Audio) on Modding Audio).
The sound can have any Pitch, however for optimal MIDI Playback, the sound should be pitched at C6. 
