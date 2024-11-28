# snip-snip

The alternate CommunityDragon directory downloader. Get directories up to 10x faster than cd-dd*.

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

## * Footnote

| Command | Mean [s] | Min [s] | Max [s] | Relative |
|:---|---:|---:|---:|---:|
| `cd-dd-windows.exe -k nothing -r https://raw.communitydragon.org/latest/game/data/images/` | 1.685 ± 0.036 | 1.638 | 1.728 | 1.05 ± 0.09 |
| `snip-snip.exe https://raw.communitydragon.org/latest/game/data/images/` | 1.607 ± 0.130 | 1.451 | 1.784 | 1.00 |
| `cd-dd-windows.exe -k nothing -r https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champions/` | 25.056 ± 0.571 | 24.277 | 25.780 | 15.59 ± 1.31 |
| `snip-snip.exe https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champions/` | 2.708 ± 0.164 | 2.523 | 2.930 | 1.69 ± 0.17 |
| `cd-dd-windows.exe -k nothing -r https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-icons/` | 26.368 ± 0.974 | 25.285 | 27.787 | 16.41 ± 1.46 |
| `snip-snip.exe https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-icons/` | 2.014 ± 0.143 | 1.929 | 2.267 | 1.25 ± 0.13 |
| `cd-dd-windows.exe -k nothing -r https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/assets/characters/annie/` | 3.141 ± 0.614 | 2.581 | 4.169 | 1.95 ± 0.41 |
| `snip-snip.exe https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/assets/characters/annie/` | 4.204 ± 1.425 | 3.091 | 6.428 | 2.62 ± 0.91 |

### Summary

Benchmarks were done using `hyperfine` with three warmups, five runs, and preparing and cleanup. **snip-snip** can run up to 10x faster than **cd-dd** 
in cases where the directory is flat with numerous files. Worse-case scenarios include nested directory of directories or if directories only contain one file. 
It is important to note that the results of these benchmarks may not carry over to all systems, especially where cores/threads are limited for concurrency usage.

### Details

- OS: Windows 10
- CPU: AMD FX-8320
- cd-dd v2.0.8
- snip-snip v4.0.1

## Credits

### Community Dragon Directory Downloader

The original directory downloader that inspired this project.
- [Repository](https://github.com/Hi-Ray/cd-dd/)

### BlossomiShymae.Smolder

A package for interfacing with CommunityDragon.
- [Repository](https://github.com/BlossomiShymae/BlossomiShymae.Smolder)

## License

snip-snip is licensed under the terms of the MIT license.
