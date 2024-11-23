# snip-snip

The alternate CommunityDragon directory downloader.

This app is currently compatible with .NET 8 and higher.

## Contributors

<a href="https://github.com/BlossomiShymae/snip-snip/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=BlossomiShymae/snip-snip" />
</a>

## Usage

`./snip-snip.exe https://raw.communitydragon.org/latest/game/data/images/`

### Download

snip-snip can be downloaded [here](https://github.com/BlossomiShymae/snip-snip/releases) here.

#### Linux

If you're running a Ubuntu distro, add the runtime if not installed:

```sudo apt-get install dotnet-runtime-8.0```

Set executable permissions:

```chmod +x ./snip-snip```

### Help

```shell
Arguments:
  0: url                          # Starting with https:// (Required)

Options:
  --filter <String>               # Filter the list. Non-matching paths are skipped.
  -c, --count <Int32>             # Concurrent download queue size controlled by semaphore (Default: 20)
  --max-depth <Int32>             # The maximum depth for files and directories. 0 is recursive. (Default: 0)
  -o, --output <String>           # Output folder for downloaded files. (Default: out)
  -f, --overwrite=<true|false>    # Force to delete the output folder if it exists. (Default: True)
  -s, --skip                      # Skip existing files. When disabled, overwrite existing files.
  -r, --retry <Int32>             # Retry attempts before giving up on a file. (Default: 3)
  -h, --help                      # Show help message
  --version                       # Show version
```

## Credits

### Community Dragon Directory Downloader

The original directory downloader that inspired this project.
- [Repository](https://github.com/Hi-Ray/cd-dd/)

### BlossomiShymae.Smolder

A package for interfacing with CommunityDragon.
- [Repository](https://github.com/BlossomiShymae/BlossomiShymae.Smolder)

## License

snip-snip is licensed under the terms of the MIT license.
