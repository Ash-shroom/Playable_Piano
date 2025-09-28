# Changelog

## 1.3.0
- songs moved from  to `Playable_Piano/assets/songs`
- Game Music now gets muted when entering Freeplay or Playback Mode
- changed default Piano sound in config from `toyPiano` to the previously added custom Sound `Mushroomy.PlayablePiano_Piano`
- Added Content Patcher Integration, allowing for the creation of custom Instruments (stationary and portable)
- added flute sound
- Reduced Noise on low Notes from the custom Piano sound

## 1.4.0
- CP Instruments now get added to the mods config
- added multiplayer support

## 1.5.0
- added API for other Mods to use
- added 'reset_instrument_sounds' command for reseting instrument sounds in the config

## 1.5.1
- fixed divide by zero error caused by Midi Files where BPM and Ticks per Quarter Note are too small