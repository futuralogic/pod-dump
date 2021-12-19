using System.Text;
using System.Reflection;

namespace futura.pod_dump;
public static class TokenHandler
{

    /// <summary>
    /// Given a tokenized string and a data model return a parsed string with tokens replaced with values from the model.
    /// </summary>
    /// <param name="tokenized"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public static string ParseTokenizedString(string tokenized, TokenMeta model)
    {
        string working = tokenized;

        Type TM = typeof(TokenMeta);

        foreach (var prop in TM.GetProperties())
        {
            var tokenAttr = prop.GetCustomAttribute<TokenAttribute>();
            if (tokenAttr != null)
            {
                var findString = tokenAttr.TokenKey;
                var val = prop.GetValue(model) as string;

                working = working.Replace($"{{{findString}}}", val);
            }
        }

        // Replace any additional

        return working;
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