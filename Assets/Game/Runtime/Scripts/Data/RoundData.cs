using UnityEngine;

namespace Game
{
    public class RoundData
    {
        public int Id { get; set; }
        public int GameMode { get; set; }
        public string Category { get; set; }
        public string Question { get; set; }
        public string[] Hints  { get; set; }
        public string[] Choices  { get; set; }
        public int AnswerIdx { get; set; }
        public Sprite AnswerSprite  { get; set; }
    }
}
