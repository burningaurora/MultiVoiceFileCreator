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
        str = str.Replace("<phoneme alphabet=+ipa+ ph=+rɪ́jməs+>Remus</phoneme>'ve", "Remus've");
        str = str.Replace("<phoneme alphabet=+ipa+ ph=+ˈbʌˈtɪldəz+>Bathilda</phoneme>'s", "<phoneme alphabet=+ipa+ ph=+ˈbʌˈtɪldəz+>Bathildas</phoneme>");
        str = str.Replace("<phoneme alphabet=\"ipa\" ph=\"naˈɡini\">Nagini</phoneme>'s", "<phoneme alphabet=\"ipa\" ph=\"naˈɡiniz\">Naginis</phoneme>");
        str = Regex.Replace(str, @"\s+", " ");
        str = str.Replace(" -'", "'");
        str = str.Replace("+", "\"");
        str = str.Replace("?", ".");
        str = str.Replace("!", ".");
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