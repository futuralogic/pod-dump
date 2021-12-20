# Apple Podcast Extractor (pod-dump)

Copies podcast audio files from Apple's podcast app, tags them, and puts them where you want.

A solution to the problem introduced by later versions of OSX and the dedicated Podcasts app that hides downloaded content from you.

This app is built for OSX and depends on data from Apple's podcast app.

# Background

Apple's Podcast application hides and obfuscates the podcast audio files on your system. With the death of iTunes the podcasts are no longer stored in your Music folder but in an obscure system location. The stored files are given cryptic filenames. Fortunately, the ID3 tags are still intact.

Although it is possible to manually find the files, copy them to a new location and rename them it is too much work if you regularly subscribe and save multiple podcasts.

pod-dump aims to automate the process.

# Usage

## Commands

```sh
> ./pod-dump

pod-dump 1.0.0+7198db5
Copyright (C) 2021 pod-dump

ERROR(S):
  No verb selected.

  add        Register a podcast using its title.

  remove     Remove a podcast registration using its ID (use list -x for ID).

  list       Lists registered podcasts with pending episodes that haven't been processed (extracted).

  extract    Extracts the podcasts, sets audio tags, and saves them to the specified target location.

  config     Set application defaults.

  find       Finds podcasts in Apple's library.

  help       Display more information on a specific command.

  version    Display version information.
```

## Command: Config

Set application defaults.
### The following settings can be configured with global default values:

|Setting Name|Default|Notes|
|---|---|---|
|TargetLocation|Current Working Directory (pwd)|Base path to folder location you want podcasts to be extracted to (ex: ~/Music)|
|RelativeLocation|(none)|Relative folder location under the {TargetLocation} to extract to. (ex: "{podcast}" would extract an mp3 called "Episode1.mp3" to ~/Music/A Podcast Title/Episode1.mp3). Use format specifiers to create dynamic paths|
|FilenameConvention|{podcast} - {title}{ext}|Default file naming convention of extracted podcasts

All location variables can utilize format tokens to dynamically generate paths or filenames.
### Format tokens:

|Token|Description|
|---|---|
|{podcast}|Podcast title (ex: This American Life)|
|{episode}|Episode title (ex: Driving Miss Daisy)|
|{ext}|Extension without a period (ex: mp3)|
|{episode_number}|Episode Number (ex: 102) - if this is zero, it will result in an empty string|
|{year}|Publication 4 digit year (ex: 2021)|
|{month}|Publication 2 digit month (ex: 02, 11, etc)|
|{day}|Publication 2 digit day of the month (ex: 02, 17, etc)|


## Command: Add

Register a podcast using its title.

To extract a podcast it must be registered with the app. That way you can mass extract all registered podcasts over time.

You may optionally specify a custom target location, relative path, and/or file naming convention for each registration.

> Locations that don't exist will be created during extraction.
>
> If your custom paths have spaces - escape them, or use double-quotes to contain the path.

## Command: Remove

Remove a podcast registration using its title.

This does not remove anything from Apple podcasts - simply the tracking for the app.

## Command: List

Lists registered podcasts along with the extraction status.

By default pending (extraction) registrations will be shown.

## Command: Extract

Extracts the podcasts, sets audio tags, and saves them to the specified target location.

## Command: Find

Searches your local Apple podcast database for podcasts.

# Build
Requires .NET 6.  Packaging requires "warp packer" (https://github.com/dgiagio/warp)

Clone the project.

Open in vscode.

```pwsh
# Build
> dotnet publish -c Release

# Pack (single binary)
dist> ./pack.ps1
```

---

Installing warp:

```sh
> cd /usr/local/bin

> curl -Lo warp-packer https://github.com/dgiagio/warp/releases/download/v0.3.0/macos-x64.warp-packer

> chmod +x warp-packer

```
# Contributing

This project is a custom solution to a personal problem.

If you find this app helpeful and find issues please report them.

If you code and feel compelled to share any fixes or new features feel free to send a pull request.
