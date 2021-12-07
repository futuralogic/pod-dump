# Technical Planning

1. Run on-demand.
2. Scans for unprocessed podcasts that are downloaded.
3. Copies MP3 to target location(s). Adds tags based on meta-data found in sqlite db. Renames file.
4. Need to track what has been processed and what hasn't.
5. Need to have some kind of configuration about where to put files in target location. Podcast -> target configuration.

- Store config in user-specific area.
	- Config:
  	- File per podcast - just a random guid or something
    	- Inside: identifier to link it to the apple podcast
    	- display name (friendly)
    	- last processed (timestamp)
    	- target location base  /mnt/music/mypodcast
    	- target location sub-path (possibly an expression, like a date breakdown or something) (./mypodcast)/{year}/{month}
- Able to list configured podcasts with meta-data displayed on target and last processed
