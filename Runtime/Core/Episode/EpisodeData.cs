
namespace Glitch9.Game.Dialogue
{
    /// <summary>
    /// Reflection으로 불러오는 형태이기 때문에 공교롭게도 Partial로 사용할 수 없다.
    /// </summary>
    public class EpisodeData
    {
        public EpisodeId Id { get; set; }
        public int BGMId { get; set; }

        public string Title { get; set; }
        public int Available { get; set; }
        public int AutoStart { get; set; }
        public int IsVoiced { get; set; }
        public bool IsNarration => Id.HasNarration;

        public UnlockCondition UnlockType { get; set; }
        public string UnlockArgument { get; set; }

        public EpisodeData() { }

        /// <summary>
        /// 스프레드시트의 칼럼 순서대로 생성자를 작성해야 한다.
        /// </summary>
        public EpisodeData(EpisodeId id, UnlockCondition unlockType, string unlockArgument, int autoStart, int available, int bgmId, int isVoiced, string title)
        {
            Id = id;
            BGMId = bgmId;
            Title = title;
            Available = available;
            AutoStart = autoStart;
            IsVoiced = isVoiced;
            UnlockType = unlockType;
            UnlockArgument = unlockArgument;
        }
    }
}