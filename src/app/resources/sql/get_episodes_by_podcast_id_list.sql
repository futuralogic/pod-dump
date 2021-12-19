-- Date calculation: https://stackoverflow.com/questions/54914123/sqlite-timestamp-field-converting-to-datetime#54914712
SELECT
    p.ZUUID As PodcastUUId,
	e.ZEPISODENUMBER As EpisodeNumber,
	p.ZAUTHOR,
	p.ZTITLE As Podcast,
    e.ZTITLE As Episode,
	datetime (e.ZPUBDATE + 978307200,
                "unixepoch",
                "utc") As EpisodePubDate,
    datetime (e.ZIMPORTDATE + 978307200,
                "unixepoch",
                "utc") As EpisodeImportDate,
    e.ZASSETURL As Url
FROM
    ZMTEPISODE e
    INNER JOIN ZMTPODCAST p
        ON p.Z_PK = e.ZPODCAST
WHERE
    -- Has a URL (i.e. file path)
	e.ZASSETURL IS NOT NULL
    -- Is in the "container" folder, meaning its downloaded.
	AND e.ZASSETURL LIKE '%Container%'
    -- Is a podcast that's registered with pod-dump
    AND p.ZUUID IN @Ids
ORDER BY
    p.ZTITLE,
    e.ZPUBDATE DESC;