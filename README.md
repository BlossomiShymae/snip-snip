# snip-snip

An alternate CommunityDragon Directory Downloader inspired by Ray's [cd-dd](https://github.com/Hi-Ray/cd-dd/). 💗

Made using C# and the .NET ecosystem.

## Requirements
- .NET 6 capable runtime

## Options
```shell
-c|--count    # Concurrent download queue size controlled by semaphore. A lower value will slow down the request rate while avoiding overloading the server. <3
```

## Usage (executable)
```shell
# Run. Limit the download semaphore queue to 10.
snip-snip -c 10 https://raw.communitydragon.org/latest/game/data/images/
```
Downloaded files and directories will be written in a relative folder called `Out`.

## Download
snip-snip can be downloaded here!

## License
snip-snip is licensed under the terms of the GNU LGPL v3 license.
