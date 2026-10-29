using FSO.Content.Framework;
using FSO.Files.Formats.IFF;
using FSO.Files.Formats.IFF.Chunks;

namespace FSO.Content.TS1
{
    /// <summary>
    /// Star level requirements for the Superstar fame track, read from fame.iff.
    /// Each table has one entry per star level (0-10).
    /// </summary>
    public class TS1FameProvider
    {
        public const int NumLevels = 11;
        public const int MaxLevel = 10;
        public const int NumSkills = 6;

        public IffFile FameResource;

        public int[] FameScoreNeeded = new int[NumLevels];
        public int[] FriendStarsNeeded = new int[NumLevels];
        //[skill][level], skills in fame.iff order: cooking, mechanical, charisma, body, logic, creativity
        public int[][] SkillNeeded = new int[NumSkills][];

        /// <summary>
        /// False when fame.iff isn't installed (no Superstar). Fame calls then fail.
        /// </summary>
        public bool Loaded;

        public TS1FameProvider(TS1Provider provider) : this((IffFile)provider.Get("fame.iff"))
        {
        }

        public TS1FameProvider(IffFile fameResource)
        {
            for (int i = 0; i < NumSkills; i++) SkillNeeded[i] = new int[NumLevels];

            FameResource = fameResource;
            var friends = FameResource?.Get<STR>(4);
            var score = FameResource?.Get<STR>(5);
            var skills = FameResource?.Get<STR>(6);
            if (friends == null || score == null || skills == null) return;

            for (int level = 0; level < NumLevels; level++)
            {
                FriendStarsNeeded[level] = ParseInt(friends.GetString(level));
                FameScoreNeeded[level] = ParseInt(score.GetString(level));
                var split = (skills.GetString(level) ?? "").Split(',');
                for (int i = 0; i < NumSkills; i++)
                {
                    SkillNeeded[i][level] = (i < split.Length) ? ParseInt(split[i]) : 0;
                }
            }
            Loaded = true;
        }

        private static int ParseInt(string str)
        {
            int result;
            int.TryParse(str?.Trim(), out result);
            return result;
        }
    }
}
