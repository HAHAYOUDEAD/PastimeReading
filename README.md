## Pastime Reading
Is a mod that allows you to [read a book](https://www.youtube.com/watch?v=MIUvKW89SgI) when there's nothing else to do in your 382-day sandbox.
![Poster](Images/Poster.png)
## Controls
`5`(by default) to bring up the book. Press it againg to open the book. 

`Q`(rotate left) and `E`(rotate right) to change pages. 

`H`(holster/put away) to close it.
## Installation
You'll need [ModSettings](https://github.com/zeobviouslyfakeacc/ModSettings/releases). 
Place `pastimeReading.dll` and `pastimeReading` folder inside your Mods folder.
## Customization
There are 4 book covers to choose from. I didn't put too much effort in them, but hey - you can change them to your liking. There's even a text color control, more on that in textures folder Readme.
## Known Issues
* I don't know much about encoding, so this might be incorrect. But, if you see squares instead of some special unicode symbols, try converting your book to UTF-8. To do that using Notepad++, open your book, select your encoding(likely Windows-1250) and convert to UTF-8. If you don't have Notepad++, you can try using any online tool. For example, [this one](https://subtitletools.com/convert-text-files-to-utf8-online) should work fine.
* ~~Loading books takes time and memory. This is because I'm using TMPro to accurately split text into pages. For reference, 300kb text takes about 10 seconds to load. And something like 2mb text will hang until you run out of memory and crash the game. If you want to read long books - split them into smaller pieces before loading. For the time being, the mod can only read book.txt, so you'll have to replace it manually when it ends. There are reasons why I didn't make it so it would continue with book2.txt and so on, and laziness is not the main one.~~ Fixed with latest update of the game. 
* Fov change when switching from book to lantern(probably some othe tools aswell). To avoid this - don't switch from book to lantern :smirk:. Holster the book first.
* Not all cover designs are created equal.
## Shoutout
[@DigitalzombieTLD](https://github.com/DigitalzombieTLD) for [FoxCompanion](https://github.com/DigitalzombieTLD/FoxCompanion) mod, I used it as a template. Probably nothing of it left in the code though.

[@ds5678](https://github.com/ds5678) for helping fix some stuff and revisioning my broken code.
