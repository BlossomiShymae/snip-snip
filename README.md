# ✁ snip-snip

An alternate CommunityDragon Directory Downloader inspired by Ray's [cd-dd](https://github.com/Hi-Ray/cd-dd/). 💜

Made using C# and the .NET ecosystem.

## Requirements

- .NET 6 capable runtime
- x64 Windows, Linux, or Mac

## Download

snip-snip can be downloaded [here](https://github.com/BlossomiShymae/snip-snip/releases)!

### Linux

If you're running a Ubuntu distro, add the runtime if not installed:

```sudo apt-get install dotnet-runtime-6.0```

Set executable permissions:

```chmod +x ./snip-snip```

## Options

```shell
# Arguments:
  URL                      # Starting with https://

# Options:
  -c|--count <COUNT>       # Concurrent download queue size controlled by semaphore. A lower value will slow down the
                           # request rate while avoiding overloading the server. <3
                           # Default value is: 20.
  --fail-fast              # Fail fast if HTTP GET file bytes request is not successful. Overrides -r|--retry.
  --filter <FILTER>        # Filter the list. Non-matching paths are skipped.
  -f|--force               # Force to overwrite existing files instead of skipping.
  --max-depth <MAX_DEPTH>  # The maximum depth for files and directories. 0 is recursive.
                           # Default value is: 0.
  -o|--output <OUTPUT>     # Output folder for exported files.
                           # Default value is: Out.
  -p|--pull                # Pull file listing from files.exported.txt to use instead of JSON listing. Requires an initial
                           # download to load listing.
  -r|--retry <RETRY>       # Retry attempts before giving up on a file.
                           # Default value is: 2.
  -?|-h|--help             # Show help information.   
```

## Tutorial (executable)
To make things easier, be sure to just copy and paste the exact URL of the folder you're in on CommunityDragon. Okie dokie?  :green_heart:

Your binary executable may be different so keep that in mind. (°▽°)
```shell
# Run. Limit the download semaphore queue to 16.
./snip-snip.exe -c 16 https://raw.communitydragon.org/latest/game/data/images/
```
Downloaded files and directories will be written in a relative folder called `Out` by default. If a file already exists, 
it will be skipped by default.

Wowie it totes worked (hopefully)! Yay!

![image](https://user-images.githubusercontent.com/87099578/227379900-eefcc844-553b-4f66-8f46-889935270e5a.png)

## "Why would I ever use this over cd-dd? If you had just realized..." 

![image](https://user-images.githubusercontent.com/87099578/227405649-ccb5ef20-54b8-462e-b02c-ae0afe72e039.png)

## License
snip-snip is licensed under the terms of the GNU LGPL v2.1 license.
