public class TokenHandler
{
	object? _model = null;

	public TokenHandler(object model)
	{
		_model = model;
	}


	/*
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
	*/

}