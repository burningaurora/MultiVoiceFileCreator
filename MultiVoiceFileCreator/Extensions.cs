using HtmlAgilityPack;
using static Constants;

public static class Extensions
{
    #region Public Methods

    /// <summary>
    /// Remove nodes that aren't needed
    /// </summary>
    /// <param name="nodes"></param>
    public static void RemoveNodes(this HtmlNodeCollection nodes)
    {
        if (nodes.SafeAny())
            foreach (var node in nodes)
                node.Remove();
    }

    /// <summary>
    /// Remove attributes that aren't needed
    /// </summary>
    /// <param name="nodes"></param>
    public static void RemoveAttributes(this HtmlNodeCollection nodes, params string[] tags)
    {
        if (nodes.SafeAny())
            foreach (var node in nodes)
                tags.ToList().ForEach(f => node.Attributes.Remove(f));
    }

    /// <summary>
    /// If we're writing out the last chapter include the standard thank you
    /// </summary>
    /// <param name="strings"></param>
    public static void LastChapter(this List<string> strings)
    {
        strings.Add($"{Constants.VOICE_SPLIT} 1500");
        strings.Add($"{Constants.VOICE_SPLIT_NARRATOR}");
        strings.Add("fin-e-tay");
        strings.Add($"{Constants.VOICE_SPLIT} 1500");
        strings.Add($"{Constants.VOICE_SPLIT_NARRATOR}");
        strings.Add("Thanks for listening to this text to speech podfic composed by Burning Aurora");
    }

    #region Voice Splits

    public static void VoiceSplitNarrator(this List<string> strings) =>
        strings.Add($"{Constants.VOICE_SPLIT_NARRATOR}");

    public static void VoiceSplit400(this List<string> strings) =>
        strings.Add($"{VOICE_SPLIT} 400");

    public static void VoiceSplit700(this List<string> strings) =>
        strings.Add($"{VOICE_SPLIT} 700");

    public static void VoiceSplit1000(this List<string> strings) =>
        strings.Add($"{VOICE_SPLIT} 1000");

    public static void VoiceSplit1200(this List<string> strings) =>
        strings.Add($"{VOICE_SPLIT} 1200");

    public static void VoiceSplit1500(this List<string> strings) =>
        strings.Add($"{VOICE_SPLIT} 1500");

    public static void VoiceSplit2000(this List<string> strings) =>
        strings.Add($"{VOICE_SPLIT} 2000");

    public static void VoiceSplit2500(this List<string> strings) =>
        strings.Add($"{VOICE_SPLIT} 2500");

    #endregion

    #endregion
}