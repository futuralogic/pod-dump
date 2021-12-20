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
