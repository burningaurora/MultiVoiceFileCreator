using HtmlAgilityPack;
using System.Text.RegularExpressions;
using static Constants;

/// <summary>
/// Common location for all reusable methods for strings
/// </summary>
public static class StringExtension
{
    #region Public Methods

    /// <summary>
    /// Trims strings that are null or have a value;
    /// Removes any leading or trailing white spaces
    /// </summary>
    /// <param name="str">String to trim</param>
    /// <returns>String.Empty if the string was null or white spaces; Trimmed string if it has a value
    /// </returns>
    public static string SafeTrim(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;
        return str.Trim();
    }

    /// <summary>
    /// Checks if a string contains a substring;
    /// Trims both strings to remove any leading or trailing white spaces prior to comparison;
    /// Ignores string case;
    /// Method called xxx.Contains(yyy)
    /// </summary>
    /// <param name="str">Original string</param>
    /// <param name="containsStr">String to check if it is part of the original string</param>
    /// <param name="ignoreCase"></param>
    /// <returns>True if the original string contains the substring; False if the original string does not contain the substring
    /// </returns>
    public static bool ContainsString(this string str, string containsStr, bool ignoreCase = true)
    {
        if (ignoreCase)
            return str?.IndexOf(containsStr, StringComparison.OrdinalIgnoreCase) >= 0;
        return str.Contains(containsStr);
    }

    /// <summary>
    /// Checks if a string starts with a substring;
    /// Ignores string case by default;
    /// Method called xxx.StartsWithString(yyy)
    /// </summary>
    /// <param name="str">Original string</param>
    /// <param name="startsStr">String to check if it is part of the original string</param>
    /// <param name="ignoreCase"></param>
    /// <returns>True if the original string contains the substring; False if the original string does not contain the substring
    /// </returns>
    public static bool StartsWithString(this string str, string startsStr, bool ignoreCase = true)
    {
        str ??= "";

        if (ignoreCase)
            return str.StartsWith(startsStr, StringComparison.OrdinalIgnoreCase);
        return str.StartsWith(startsStr);
    }

    /// <summary>
    /// Compares two string to determine if they are the same;
    /// Trims both strings to remove any leading or trailing white spaces prior to comparison;
    /// Ignores string case;
    /// Method called xxx.Compare(yyy)
    /// </summary>
    /// <param name="str">Original string</param>
    /// <param name="compareStr">String to compare to the original string</param>
    /// <returns>True if the strings are identical; False if the strings are different
    /// </returns>
    public static bool Compare(this string str, string compareStr)
    {
        return string.Equals(str.SafeTrim(), compareStr.SafeTrim(), StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Returns if a string is within a list of strings
    /// </summary>
    /// <param name="str"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool InList(this string str, params string[] list)
    {
        var strList = list.Select(SafeTrim);
        return strList.Contains(str);
    }

    /// <summary>
    /// Takes an input string containing HTML markup and returns a clean string of character content.
    /// </summary>
    /// <param name="str">Input text</param>
    /// <returns>Cleaned output text as string</returns>
    public static string StripOutHtml(this string str)
    {
        var objRegExp = new System.Text.RegularExpressions.Regex("<(.|\n)*?>");
        return objRegExp.Replace(str, "").Replace("&nbsp;", "").Replace("$amp;", "");
    }

    /// <summary>
    /// Cast the string to a boolean
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Boolean ToBoolean(this string str)
    {
        return Convert.ToBoolean(str);
    }

    /// <summary>
    /// Uses regex '\b' as suggested in https://stackoverflow.com/questions/6143642/way-to-have-string-replace-only-hit-whole-words
    /// </summary>
    /// <param name="original"></param>
    /// <param name="wordToFind"></param>
    /// <param name="replacement"></param>
    /// <param name="regexOptions"></param>
    /// <returns></returns>
    public static string ReplaceWholeWord(this string original, string wordToFind, string replacement, RegexOptions regexOptions = RegexOptions.None)
    {
        string pattern = String.Format(@"\b{0}\b", wordToFind);
        if (wordToFind.EndsWith("'") || wordToFind.EndsWith(".") || wordToFind.EndsWith("-"))
            pattern = String.Format(@"\b{0}\B", wordToFind);
        string ret = Regex.Replace(original, pattern, replacement, regexOptions);
        return ret;
    }

    /// <summary>
    /// Gets rid of nodes where the html is empty
    /// </summary>
    /// <param name="html"></param>
    /// <param name="unwantedTags"></param>
    /// <returns></returns>
    public static string RemoveUnwantedHtmlTags(this string html, List<string> unwantedTags)
    {
        if (string.IsNullOrWhiteSpace(html))
            return html;

        var document = new HtmlDocument();
        document.LoadHtml(html);

        HtmlNodeCollection tryGetNodes = document.DocumentNode.SelectNodes("./*|./text()");

        if (!tryGetNodes.SafeAny())
            return html;

        var nodes = new Queue<HtmlNode>(tryGetNodes);

        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            var parentNode = node.ParentNode;
            var childNodes = node.SelectNodes("./*|./text()");

            if (childNodes != null)
                foreach (var child in childNodes)
                    nodes.Enqueue(child);

            if (unwantedTags.Any(tag => tag == node.Name))
            {
                if (childNodes != null)
                    foreach (var child in childNodes)
                        parentNode.InsertBefore(child, node);

                parentNode.RemoveChild(node);
            }
        }

        return document.DocumentNode.InnerHtml;
    }

    /// <summary>
    /// Format each line as an html paragraph
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string AddPTags(this string str)
    {
        try
        {
            if (str.Contains(Constants.VOICE_SPLIT))
                return str;
            if (char.IsPunctuation(str.ToCharArray().First()) && str.Length == 1)
                return string.Empty;
            if (char.IsPunctuation(str.ToCharArray().First()) && str.Substring(1, 1) == " ")
                str = str.Substring(2);
            return $"<p>{str.SafeTrim()}</p>";
        }
        catch (Exception ex)
        {
            var test = ex;
            throw;
        }
    }

    /// <summary>
    /// Remove all space and punctuation from string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToPunctuationlessString(this string str)
    {
        return new string(str.ToCharArray().Where(c => !char.IsPunctuation(c) && c != ' ').ToArray());
    }

    /// <summary>
    /// Replace words based on dictionaries for TTS
    /// </summary>
    /// <param name="str"></param>
    /// <param name="generationMethod"></param>
    /// <returns></returns>
    public static string DictionaryReplacer(this string str, Method generationMethod)
    {
        if (str.Contains(VOICE_SPLIT))
            return str;
        ReadDictionaries(generationMethod).ToList().ForEach(f => str = str.ReplaceWholeWord(f.Key, f.Value, RegexOptions.IgnoreCase));
        str = str.Replace("<phoneme alphabet=+ipa+ ph=+rɪ́jməs+>Remus</phoneme>'s", "Remus's");
        str = str.Replace("<phoneme alphabet=+ipa+ ph=+rɪ́jməs+>Remus</phoneme>'ll", "Remus'll");
        str = str.Replace("<phoneme alphabet=\"ipa\" ph=\"naˈɡini\">Nagini</phoneme>'s", "<phoneme alphabet=\"ipa\" ph=\"naˈɡiniz\">Naginis</phoneme>");
        str = Regex.Replace(str, @"\s+", " ");
        str = str.Replace(" -'", "'");
        str = str.Replace("+", "\"");
        str = str.Replace("?", ".");
        return str;
    }

    /// <summary>
    /// Get rid of empty lines where possible
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string EmptyLineRemover(this string str)
    {
        str = Regex.Replace(str, @"\s+", " ");
        str = Regex.Replace(str, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        return str;
    }

    /// <summary>
    /// Clean up HTML where possible
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string HTMLCleanUp(this string str)
    {
        str = str.EmptyLineRemover();
        str = str.Replace("\"+", "+");
        str = str.Replace("+\"", "+");
        str = str.Replace("<p>, ", "<p>");
        str = str.Replace("<p>. ", "<p>");
        str = str.Replace("<p>' ", "<p>");
        str = str.Replace("<p>\"  ", "<p>\"");
        return str;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Read the replacements we need to apply to the file
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, string> ReadDictionaries(Method generationMethod)
    {
        var dict = new Dictionary<string, string>();
        var files = Directory.GetFiles($@"C:\sandbox\MultiVoiceFileCreator\MultiVoiceFileCreator\Dictionaries");
        var test = files.OrderBy(f => f);
        foreach (var file in files.OrderBy(f => f))
        {
            if (generationMethod == Method.Edge && file.Contains("-IPA"))
                continue;
            if (generationMethod != Method.Edge && file.Contains("-Pronunciation"))
                continue;
            var fileContents = File.ReadAllLines(file).Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
            fileContents.ForEach(f => dict.Add(f.Split('|')[0], f.Split('|')[1]));
        }
        return dict;
    }

    #endregion
}