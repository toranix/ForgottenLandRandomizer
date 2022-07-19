# ForgottenLandRandomizer
A randomizer tool for Kirby and the Forgotten Land.

## Features
* Patching the game's Scn.bin Mint binary with custom code
* Numeric and text RNG seeds
* Randomizing Stage Unlocks
  * Randomizing the order in which story stages are unlocked
  * Automatic Waddle Dee requirement scaling for Boss stages
	* Can also set requirements to none (except for 6-6)

## Planned Features
* Kirby color randomization
* Randomizing Copy Abilities
* Very likely more!

## Usage
1. Dump the RomFS of Kirby and the Forgotten Land. Yuzu or the [nxdumptool](https://github.com/DarkMatterCore/nxdumptool/releases/tag/v1.1.14) homebrew application are the easiest ways to do so.
2. Launch the program and either click the elipses (...) button to select the RomFS folder or enter the path manually. This folder must contain the `basil` and `yaml` folders.
This directory is saved in `Config.xml` when the program exits and is automatically loaded when the program opens.
3. Mess with the program's options and click the `Randomize` button at the bottom of the program. This will take a small bit of time to complete.
When this has completed, the files are outputted to `OutFiles\seed_<seed>`, already formatted for use with LayeredFS with Atmosphère, Yuzu, or Ryujinx.

Seeds can be full numbers from 0 to 4,294,967,295, hexadecimal numbers from 0 to FFFFFFF or text strings such as "Super_Mario 64".

For information about the randomizer options, hover over one for a brief description.

## Credits
* Forked from Firubii's Kirby Star Allies Randomizer
* Adapted Firubii's YAML read/writing code from KirbyYAML
