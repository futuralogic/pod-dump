SELECT
    p.ZUUID As Id,
	p.ZAUTHOR As Author,
	p.ZTITLE As Podcast
FROM ZMTPODCAST p
where
	p.ZTITLE LIKE @Search
    --p.ZTITLE LIKE '%resident%'
ORDER BY
    p.ZTITLE;