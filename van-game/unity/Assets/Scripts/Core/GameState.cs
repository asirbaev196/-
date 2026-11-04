using System;
using System.Collections.Generic;

namespace Van.Core
{
    public enum GameAct { Prolog, Act1_Survival, Act2_Ranger, Act3_VanReturns, Final }

    [Serializable]
    public class Clue
    {
        public string id;          // "lavender_smell"
        public string description; // "Запах лаванды в фургоне"
        public bool isFact;        // false = подозрение
        public float timestamp;    // когда найден

        public Clue(string id, string desc, bool fact = false)
        {
            this.id = id;
            description = desc;
            isFact = fact;
            timestamp = UnityEngine.Time.time;
        }
    }

    [Serializable]
    public class GameState
    {
        public GameAct currentAct = GameAct.Prolog;
        public List<Clue> clues = new List<Clue>();
        public List<string> inventory = new List<string>();
        public float health = 100f;
        public float brokenRibPain = 50f;   // 0..100, влияет на скорость
        public bool rangerTrustsPlayer = false;
        public bool playerLiedToRanger = false;
        public bool vanReturned = false;
        public int dayCount = 1;
        public string endingId = "";

        public bool HasClue(string id) => clues.Exists(c => c.id == id);

        public void AddClue(Clue c)
        {
            if (!HasClue(c.id)) clues.Add(c);
        }
    }
}
