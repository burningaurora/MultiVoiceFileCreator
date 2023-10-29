public static class Constants
{
    public const string BOOK_NLP = "BookNLP_";
    public const string VOICE_SPLIT = "VoiceSplit:";
    public const string VOICE_SPLIT_NARRATOR = "VoiceSplit: Narrator";

    public enum Method
    {
        MultiVoice,
        Edge,
        SingleVoice,
        BookNLP
    }

    public static Dictionary<string, string> ReplacementCharacters =
        new()
        {
            { "h2", "p" },
            { "<p>Chapter Summary</p>", "" },
            { "<p>Chapter Notes</p>", "" },
            { "<hr>", "<p>~~~</p>" },
            { "<hr/>", "<p>~~~</p>" },
            { "***", "~~~" },
            { "&", "and" },
            { "🐾", "; Paw prints emoji." },
            { "🐉", "; Dragon emoji." },
            { "🦦", "; Otter emoji." },
            { "🌑", "; Moon emoji." },
            { "🌼", "; Flower emoji." },
            { " > ", " more than " },
            { " < ", "less than" },
            { ",", "," }
        };

    public static Dictionary<string, string> FormattingReplacements =
        new()
        {
            { "!--", "! " },
            { "---", "-" },
            { "--", "... " },
            { " ", " " },
            { "- \"", ".\"" },
            { "-\"", ".\"" },
            { "....", "..." },
            { "... ?", "?" },
            { "-?", "?" },
            { ".-<", ".<" },
            { "-<", ".<" },
            { "-!", "!" },
            { ">-", ">" },
            { "*", "" },
            { "\"-", "\"" },
            { " - ", "; " },
            { "- ", "... " },
            { "  ", " " },
            { " .", "." },
            { "> ", ">" },
            { " <", "<" },
            { " ?", "?" },
            { " , ", ", " },
            { " !", "!" },
            { " ... ", "... " },
            { " ; ", "; " },
            { "?;", "?" },
            { "!;", "!" },
            { " ,", "," },
            { " . ", ". " },
            { "( ", "(" },
            { " )", ")" },
            { " .<", ".<" },
            { " .\"<", ".\"<" },
            { " !\"<", "!\"<" },
            { ">\" ", ">\"" },
            { ">.\"", ">\"" },
            { ", \"", ". \"" },
            { ", '", ". '" },
            { "\".\"", ".\"" },
            { "-'", "'" },
            { "\".'", "'" },
            { "'.\"", "'\"." },
            { ";;", ";" },
            { " ' ", " " },
            { "...?", "?" },
            { ",;", ";" },
            { "...", "... " }
        };

    public static readonly Dictionary<string, string> CasedCommonReplacements =
        new()
        {
            { "<! chapter content->", "" },
            { "<! /chapter content->", "" },
            { "<! chapter content... >", "" },
            { "<! /chapter content... >", "" }
        };
}