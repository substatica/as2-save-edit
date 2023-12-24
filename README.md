## Arizona Sunshine 2 Save Editor

This is a console based save editor for Arziona Sunshine 2 on PCVR. The major benefit of this editor is the ability to access late-game weapons early, as well as the mini gun, before they are available in addition to the ability to max out ammo levels at any checkpoint.

*Usage*

Run the app from a commandline with adminstrator privileges. The app will attempt to locate your Arizona Sunshine 2 save file, usually located here,

c:\Program Files (x86)\Steam\userdata\[SteamUserId]\1540210\remote\Savegame.save

If this cannot be located you will need to specify the path to your save file with the -s parameter, like so,

./as2-save-edit.exe -s "c:\Program Files (x86)\Steam\userdata\[SteamUserId]\1540210\remote\Savegame.save"

If the app is able to parse your save you will be prompted to select a save slot. The app will then display the ammo and items in that save slot. After displaying the current loadout the app will prompt you for a new loadout.

A small weapon for each hip slot. An explosive for each sleeve slot. A long weapon for the shoulder slot, and two weapons for the companion, Buddy's, slots. Buddy is indended to carry two small weapons however you can select any two items for Buddy to carry, once removed from Buddy you will only be able to re-add small weapons. In this way you can load Buddy with two mini guns, but you will not be able to store them back on Buddy, or in any other slots.

To cancel editing press Control+C to exit at any time. Once you have selected a new loadout the app will create a backup of the save file and save the new version in the original's place.

The -d command line parameter is still in development.

http://paypal.me/substatica
