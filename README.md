# ✁ snip-snip

An alternate CommunityDragon Directory Downloader inspired by Ray's [cd-dd](https://github.com/Hi-Ray/cd-dd/). 💜

Made using C# and the .NET ecosystem.

## Requirements
- .NET 6 capable runtime
- x64 Windows, Linux, or Mac

## Download
snip-snip can be downloaded [here](https://github.com/BlossomiShymae/snip-snip/releases)!

## Options
```shell
-c|--count    # Concurrent download queue size controlled by semaphore. A lower value will 
              # slow down the request rate while avoiding overloading the server. <3
-r|--retry    # Retry attempts before giving up on a file.
-f|--failfast # Fail fast if HTTP GET bytes request is not successful. 
              # Overrides -r|--retry.
-p|--pull     # Pull file listing from the files exported text file so only one request is
              # needed for the directories of files. Requires a bigger initial to load
              # listing.     
```

## Tutorial (executable)
To make things easier, be sure to just copy and paste the exact URL of the folder you're in on CommunityDragon. Okie dokie?  :green_heart:

Your binary executable may be different so keep that in mind. (°▽°)
```shell
# Run. Limit the download semaphore queue to 16.
./snip-snip.exe -c 16 https://raw.communitydragon.org/latest/game/data/images/
```
Downloaded files and directories will be written in a relative folder called `Out`. If `Out` already exists, it will be overwritten!

Wowie it totes worked (hopefully)! Yay!

![image](https://user-images.githubusercontent.com/87099578/227379900-eefcc844-553b-4f66-8f46-889935270e5a.png)

## "Why would I ever use this over cd-dd? If you had just realized..." 

![image](https://user-images.githubusercontent.com/87099578/227405649-ccb5ef20-54b8-462e-b02c-ae0afe72e039.png)

## License
snip-snip is licensed under the terms of the GNU LGPL v2.1 license.
