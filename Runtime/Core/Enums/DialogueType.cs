using Glitch9.Apis.Google.Sheets;
using UnityEngine;

namespace Glitch9.Game.Dialogue
{
    [Sheets(typeof(DialogueType))]
    public enum DialogueType
    {
        [InspectorName("대사")]
        Line,
        [InspectorName("선택지")]
        Event,
        [InspectorName("칵테일 이벤트")]
        Cocktail,
        [InspectorName("BGM 변경")]
        Bgm,
        [InspectorName("SFX 재생")]
        Sfx,
        [InspectorName("나레이션")]
        Narration,
        [InspectorName("아이템 획득")]
        GetItem,
        [InspectorName("챕터 제목")]
        Title,
        [InspectorName("챕터 부제목")]
        Subtitle,
        [InspectorName("캐릭터 등장")]
        Enter,
        [InspectorName("캐릭터 퇴장")]
        Exit,
        [InspectorName("텍스트 입력")]
        InputField,
        [InspectorName("대화 종료")]
        Finish,
        [InspectorName("튜토리얼 모드")]
        AssistStart,
        [InspectorName("튜토리얼 모드 종료")]
        AssistEnd,
        [InspectorName("UI 선택")]
        Click,
        [InspectorName("다음 대화 대기")]
        Wait,
        [InspectorName("설문지/질문지")]
        Questionnaire,
        [InspectorName("컷씬/이벤트이미지")]
        ShowCutScene,
        [InspectorName("컷씬/이벤트이미지 종료")]
        HideCutScene,
        [InspectorName("없음")]
        Empty
    }
}
