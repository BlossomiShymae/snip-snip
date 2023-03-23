# ✁ snip-snip

An alternate CommunityDragon Directory Downloader inspired by Ray's [cd-dd](https://github.com/Hi-Ray/cd-dd/). 💜

Made using C# and the .NET ecosystem.

## Requirements
- .NET 6 capable runtime

## Options
```shell
-c|--count    # Concurrent download queue size controlled by semaphore. A lower value will slow down the request rate while avoiding overloading the server. <3
```

## Tutorial (executable)
To make things easier, be sure to just copy and paste the exact URL of the folder you're in on CommunityDragon. Okie dokie?  :green_heart:
```shell
# Run. Limit the download semaphore queue to 10.
snip-snip -c 10 https://raw.communitydragon.org/latest/game/data/images/
```
Downloaded files and directories will be written in a relative folder called `Out`. If `Out` already exists, it will be overwritten!

Wowie it totes worked (hopefully)! Yay!

![image](https://user-images.githubusercontent.com/87099578/227379900-eefcc844-553b-4f66-8f46-889935270e5a.png)


## Download
snip-snip can be downloaded here!

## License
snip-snip is licensed under the terms of the GNU LGPL v3 license.
