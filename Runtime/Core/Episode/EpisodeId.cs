namespace Glitch9.Game.Dialogue
{
    public class EpisodeId
    {
        public const string DEFAULT_SEPARATOR = "_";
        public static EpisodeId Question => new() { IsQuestionnaire = true };

        public int Id { get; private set; }
        public int ChapterId { get; private set; }
        public bool IsMainEpisode => Id < 1;
        public bool IsSideEpisode => ChapterId < 1;
        public bool HasNarration => !IsSideEpisode;
        public bool IsQuestionnaire { get; private set; } = false;

        public EpisodeId() { }

        public EpisodeId(int chapterId, int sideChapterId = 0)
        {
            ChapterId = chapterId;
            Id = sideChapterId;
        }

        public EpisodeId(string idAsString)
        {
            EpisodeId id = FromString(idAsString);
            ChapterId = id.ChapterId;
            Id = id.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            EpisodeId other = obj as EpisodeId;
            if (other == null)
                return false;

            return ChapterId == other.ChapterId && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return ChapterId.GetHashCode() ^ Id.GetHashCode();
        }

        public static bool operator ==(EpisodeId a, EpisodeId b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return a.ChapterId == b.ChapterId && a.Id == b.Id;
        }

        public static bool operator !=(EpisodeId a, EpisodeId b)
        {
            return !(a == b);
        }

        public string ToString(char separator)
        {
            return $"{ChapterId}{separator}{Id}";
        }
        public override string ToString()
        {
            return ToString(DEFAULT_SEPARATOR[0]);
        }

        public static EpisodeId FromString(string str)
        {
            string[] split = str.Split('_');
            if (split.Length != 2)
            {
                GNLog.Error("Invalid chapter identifier string: " + str);
                return null;
            }

            int chapterID = int.Parse(split[0]);
            int sideChapterID = int.Parse(split[1]);

            return new EpisodeId(chapterID, sideChapterID);
        }


        // acts as a regular string
        public static implicit operator string(EpisodeId chapterIdentifier) => chapterIdentifier.ToString();

        public static implicit operator EpisodeId(string str) => FromString(str);
    }
}
