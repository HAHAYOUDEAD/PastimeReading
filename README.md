## Public beta
The DLC update broke text loading times once again. With help of a few fellow community members I found out that this only affects "large" unicode characters(read non-latin), so I decided to release a public beta for those who can read in english or whose alpabet consists mostly of latin characters

Added:
- 2 new book covers
- experimental time slowdown effect while reading(check options)
- disable interactions while reading. Look over the book to interact(can be reverted to old behavior, check options)

Check [issues](#known-issues) for more info


## Pastime Reading
Is a mod that allows you to [read a book](https://www.youtube.com/watch?v=MIUvKW89SgI) when there's nothing else to do in your 382-day sandbox.
![Poster](Images/Poster.png)
## Controls
`5`(by default) to bring up the book. Press it againg to open the book. 

`Q`(rotate left) and `E`(rotate right) to change pages. 

`H`(holster/put away) to close it.
## Installation
You'll need [ModSettings](https://github.com/zeobviouslyfakeacc/ModSettings/releases). 
Place `pastimeReading.dll` and `pastimeReading` folder inside your `../Mods/` folder.

To load your book, go to `../Mods/pastimeReading/` and paste your book in `book.txt`.

## Usage
If you are not sure how to add your books, or where to get them - check this neat [tutorial](https://github.com/HAHAYOUDEAD/PastimeReading/blob/main/Import%20Your%20Own%20Books/README.md) by [@Ancient Gatekeeper](https://github.com/GamingWubba93) 

## Customization
There are 7 book covers to choose from. And you can create your own. There's even a text color control, more on that in textures folder Readme.

## Known Issues
- Loading times are nuts with non-latin texts. Can't fix it yet, let's wait for another engine update

- There are no sounds anymore. Fix will require switching to [AudioManager](https://github.com/DigitalzombieTLD/AudioManager), which will take a bit of time

- Bug when going through loading screen while the book is out. I'll fix it, just not feeling like it at the moment :sweat_smile:

- Doesn't work with [Personality](https://github.com/HAHAYOUDEAD/Personality), because it uses a separate arms mesh. Extremely annoying to fix since I need to redo all animations, but I should fix in eventually



## Shoutout
[@DigitalzombieTLD](https://github.com/DigitalzombieTLD) for [FoxCompanion](https://github.com/DigitalzombieTLD/FoxCompanion) mod, I used it as a template. Probably nothing of it left in the code though.

[@ds5678](https://github.com/ds5678) for helping fix some stuff and revisioning my broken code.
