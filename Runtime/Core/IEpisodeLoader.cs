using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Glitch9.Game.Dialogue
{
    public interface IEpisodeLoader
    {
        UniTask<List<EpisodeData>> LoadEntriesAsync(string characterId);
        UniTask<Episode> LoadEpisodeAsync(string characterId, EpisodeId episodeId);
    }
}