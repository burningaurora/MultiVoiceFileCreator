using HtmlAgilityPack;

public static class HtmlNodeExtension
{
    /// <summary>
    /// Get the title to append at the top of the file
    /// </summary>
    /// <param name="containerNode"></param>
    /// <returns></returns>
    public static string FanFictionTitle(this HtmlNode containerNode)
    {
        try
        {
            var title = containerNode.SelectNodes("//h1")?.First()?.InnerText;
            var byLine = containerNode.SelectNodes("//div")?.Where(f => f?.Attributes["class"] != null && (f.Attributes["class"].Value.Compare("byline")))?.First()?.InnerText;
            return $"{title}, {byLine}";
        }
        catch (Exception)
        {
            return string.Empty;
        };
    }

    /// <summary>
    /// Gets rid of nodes with no content
    /// </summary>
    /// <param name="containerNode"></param>
    public static void RemoveEmptyNodes(this HtmlNode containerNode)
    {
        if (!containerNode.Attributes.SafeAny() && string.IsNullOrWhiteSpace(containerNode.InnerText))
            containerNode.Remove();
        else
            for (int i = containerNode.ChildNodes.Count - 1; i >= 0; i--)
                RemoveEmptyNodes(containerNode.ChildNodes[i]);
    }
}