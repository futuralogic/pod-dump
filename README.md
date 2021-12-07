# Apple Podcast Dumper (pod-dump)

Extracts Apple podcasts, tags them, and puts them where you want.

A solution to the problem introduced by later versions of OSX and the dedicated Podcasts app that hides downloaded content from you.

# Usage

## Commands

```sh
> pod-dump

Usage: pod-dump <command> [options]

Extract podcasts from Apple Podcasts and put them where you want them.

Commands:
   add        Register a podcast
   config     Show or set application defaults
   extract    Extracts podcasts
   list       Lists unextracted podcasts
   listall    Lists all podcasts
   remove     Removes a registered podcast

See README.md for full documentation.
```

## Version

```sh
> pod-dump -v
futura podcast dumper (futuralogic.com)
version 1.0.1 (commit: ce930fac)
built 12/7/2021 12:33am
la futura e mia.
```

## Command: Config

Set application defaults.

### The following settings can be configured with global default values:

|Setting Name|Default|Notes|
|---|---|---|
|TargetLocation|Current Working Directory (pwd)|Base path to folder location you want podcasts to be extracted to (ex: ~/Music)|
|RelativeLocation|(none)|Relative folder location under the {TargetLocation} to extract to. (ex: "{podcast}" would extract an mp3 called "Episode1.mp3" to ~/Music/A Podcast Title/Episode1.mp3). Use format specifiers to create dynamic paths|
|FilenameConvention|{podcast}{ - }{title}.{ext}|Default file naming convention of extracted podcasts

`RelativeLocation` and `FilenameConvention` can utilize format tokens to dynamically generate paths or filenames.
### Format tokens:

|Token|Description|
|---|---|
|{podcast}|Podcast title (ex: This American Life)|
|{title}|Episode title (ex: Driving Miss Daisy)|
|{ext}|Extension without a period (ex: mp3)|
|{episode}|Episode Number (ex: 102) - if this is zero, it will result in an empty string|
|{year}|Publication 4 digit year (ex: 2021)|
|{month}|Publication 2 digit month (ex: 02, 11, etc)|
|{day}|Publication 2 digit day of the month (ex: 02, 17, etc)|
|{ - } or {-}|If the preceding (left-hand) string is empty, do not display the hyphen. Spaces or no spaces.|

```sh
# Display application defaults
> pod-dump config
TargetLocation: ~/Music/podcasts
RelativeLocation: {podcast}
FilenameConvention: {episode}{ - }{title}.{ext}

# Set the default target: assumes no RelativeLocation set yet.
> pod-dump config TargetLocation "~/Music/podcasts"
New target location: ~/Music/podcasts

Full extraction location: ~/Music/podcasts

# Set the relative location: presumes the TargetLocation was just set.
> pod-dump config RelativeLocation "{podcast}/{year}"
New RelativeLocation: {podcast}/{year}

Full extraction location: ~/Music/podcasts/{podcast}/{year}

# Set the FilenameConvention format specifier:
> pod-dump config FilenameConvention "{year}{-}{episode}{ - }{podcast} - {title}.{ext}"
New FilenameConvention: {year}{-}{episode}{ - }{podcast} - {title}.{ext}

# Example filename using the above format specifier:
# 2021-127 - This American Life - An Episode Title.mp3

```
## Command: Add

Register a podcast using its title.

To extract a podcast it must be registered with the app. That way you can mass extract all registered podcasts over time. You may optionally specify a custom target location for a registration, or a custom relative path for a registration. The custom paths will apply to all podcasts added when this command runs - see notes about search term specificity below.

Usage: `pod-dump add <podcast_title> [-exact] [-to custom_target_location] [-rel custom_relative_location] [-file custom_filename_convention]`

- `exact` - Force exact matching of the podcast title.
- `podcast_title` - Exact or partial title match of your subscribed podcasts.
- `custom_target_location` - Override the default `TargetLocation` configuration setting for this registration.
- `custom_relative_location` - Override the default `RelativeLocation` configuration setting for this registration.
- `custom_filename_convention` - Override the default `FilenameConvention` configuration setting for this registration.

> Locations that don't exist will be created during extraction.
>
> If your custom paths have spaces - escape them, or use double-quotes to contain the path.


### Examples:

```sh
# Uses global TargetLocation (~/Music/podcasts), default RelativeLocation (blank), and default FilenameConvention
> pod-dump add "hardcore history"
2 podcasts matched, will extract to ~/Music/podcasts
Adding "Dan Carlin's Hardcore History"
Adding "Dan Carlin's Hardcore History: Addendum"

# Uses global TargetLocation (~/Music/podcasts), default RelativeLocation (blank), and default FilenameConvention
# Demonstrates exact search:
> pod-dump add "Dan Carlin's Hardcore History" -exact
1 podcast matched (exact), will extract to ~/Music/podcasts
Adding "Dan Carlin's Hardcore History"

# Overrides TargetLocation, default RelativeLocation (blank), and default FilenameConvention
> pod-dump add "Dan Carlin's Hardcore History" -exact -to "~/Music/podcasts/HH"
1 podcast matched (exact), will extract to ~/Music/podcasts/HH
Adding "Dan Carlin's Hardcore History"

# Overrides RelativeLocation, assumes global TargetLocation (~/Music/podcasts)
> pod-dump add "Dan Carlin's Hardcore History" -exact -rel "HH/{year}/{month}"
1 podcast matched (exact), will extract to ~/Music/podcasts/HH/{year}/{month}
Adding "Dan Carlin's Hardcore History"

# Overrides TargetLocation and RelativeLocation
> pod-dump add "Dan Carlin's Hardcore History" -exact -rel "HH/{year}/{month}" -to "~/Music/podcasts/DanCarlin"
1 podcast matched (exact), will extract to ~/Music/podcasts/DanCarlin/HH/{year}/{month}
Adding "Dan Carlin's Hardcore History"

# Overrides TargetLocation and RelativeLocation and FilenameConvention
> pod-dump add "Dan Carlin's Hardcore History" -exact -rel "HH/{year}/{month}" -to "~/Music/podcasts/DanCarlin" -file "HH - {episode}.{ext}"
1 podcast matched (exact), will extract to ~/Music/podcasts/DanCarlin/HH/{year}/{month}
Adding "Dan Carlin's Hardcore History"

# Uses global TargetLocation (~/Music/podcasts), overrides RelativeLocation, and default FilenameConvention
> pod-dump add "Dan Carlin's Hardcore History" -exact -rel "DanCarlin/{year}/{month}" -file "HH - {episode}.{ext}"
1 podcast matched (exact), will extract to ~/Music/podcasts/DanCarlin/{year}/{month}
Adding "Dan Carlin's Hardcore History"


```

## Command: Remove

Remove a podcast registration using its title.

This does not remove anything from Apple podcasts - simply the tracking for the app.

- `exact` - Force exact matching of the podcast title.

```sh
> pod-dump remove "hardcore history"
2 podcasts matched
Removing "Dan Carlin's Hardcore History"
Removing "Dan Carlin's Hardcore History: Addendum"

> pod-dump remove "Dan Carlin's Hardcore History" -exact
1 podcast matched (exact)
Removing "Dan Carlin's Hardcore History"

```

## Command: ListAll (lsa)

Lists all registered podcasts along with the extraction status.

```sh

> pod-dump lsa
Podcast                 Available    Last Processed       Extraction Target

My Podcast               2           11/1/2021            ~/Music/podcasts/mypodcast/{year}/{month}/{podcast} - {title}.{ext}
This American Life       -           10/25/2021           ~/Music/podcasts/TAL/{year}/{month}/{podcast} - {title}.{ext}

```

## Command: List (ls)

Lists registered podcasts that haven't been extracted.

```sh

> pod-dump ls
Podcast                 Available    Last Processed       Extraction Target

My Podcast               2           11/1/2021            ~/Music/podcasts/mypodcast/{year}/{month}/{podcast} - {title}.{ext}

```


## Command: Extract

Extracts the podcasts, sets audio tags, and saves them to the specified target location.

```sh
> pod-dump extract
Extracting podcasts:

My Podcast - 2 episodes to ~/Music/podcasts/mypodcast/2021/12 ....done.

2 episodes extracted in 4.6s
```

