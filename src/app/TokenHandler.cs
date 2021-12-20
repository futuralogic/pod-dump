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
                // Using the attribute - get the "token" to look for "year" for example
                var findString = tokenAttr.TokenKey;
                // Retrieve the data value from our model for this property. Example: Year = 2021.
                var valObj = prop.GetValue(model, null);

                // if our value is not null, try to process it.
                if (valObj != null)
                {
                    // Convert to a string for replacement
                    var val = Convert.ToString(valObj);
                    // Clean up the data value so it doesn't have illegal filenaming characters, or characters we don't want.
                    var replaced = val?.Replace("/", "-").Replace("\\", "-").Replace("#", "").Replace(":", "-");
                    // Replace the token (Ex. "{year}") in our working token string.
                    working = working.Replace($"{{{findString}}}", replaced);
                }
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