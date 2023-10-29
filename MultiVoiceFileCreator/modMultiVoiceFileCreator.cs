using static Constants;

namespace MultiVoiceFileCreator
{
    public class modMultiVoiceFileCreator
    {
        #region Properties

        private static CreationParms _creationParms;

        #endregion

        #region Handler

        private static void Main(string[] args)
        {
            ParseArgs(args.FirstOrDefault());

            // Run the App
            var MultiVoiceFileCreator = new MultiVoiceFileCreator(_creationParms);
            MultiVoiceFileCreator.Execute();
        }

        #endregion

        #region Private Methods

        private static void ParseArgs(string txtFile)
        {
            var args = File.ReadAllLines(Path.Combine(@"C:\sandbox\MultiVoiceFileCreator\MultiVoiceFileCreator\Fics", $"{txtFile}.txt"));
            _creationParms = new CreationParms(
                args.Where(f => f.StartsWithString("/url=")).First().Replace("/url=", string.Empty),
                args.Where(f => f.StartsWithString("/baseDirectory=")).FirstOrDefault()?.Replace("/baseDirectory=", string.Empty)?.Replace("\"", ""),
                args.Where(f => f.StartsWithString("/includeChapterNumber=")).FirstOrDefault()?.Replace("/includeChapterNumber=", string.Empty),
                args.Where(f => f.StartsWithString("/title=")).FirstOrDefault()?.Replace("/title=", string.Empty),
                args.Where(f => f.StartsWithString("/author=")).FirstOrDefault()?.Replace("/author=", string.Empty),
                args.Where(f => f.StartsWithString("/chapter=")).FirstOrDefault()?.Replace("/chapter=", string.Empty),
                args.Where(f => f.StartsWithString("/lastChapter=")).FirstOrDefault()?.Replace("/lastChapter=", string.Empty),
                args.Where(f => f.StartsWithString("/method=")).FirstOrDefault()?.Replace("/method=", string.Empty) ?? nameof(Method.MultiVoice));
        }

        #endregion
    }
}