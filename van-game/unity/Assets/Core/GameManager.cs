using System;
using UnityEngine;
using Van.Survival;
using Van.Detective;
using Van.NPC;

namespace Van.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Ссылки на системы")]
        public PlayerSurvival playerSurvival;
        public JournalSystem journal;
        public RangerDialogue ranger;
        public PoacherAI poacher;

        [Header("День/ночь")]
        public float dayLengthSeconds = 600f; // 10 минут реального времени
        private float dayTimer;

        public GameState State { get; private set; } = new GameState();

        public event Action<GameAct> OnActChanged;
        public event Action<int> OnDayChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SaveSystem.TryLoad(out var loaded);
            if (loaded != null) State = loaded;
            ChangeAct(State.currentAct);
        }

        private void Update()
        {
            dayTimer += Time.deltaTime;
            if (dayTimer >= dayLengthSeconds)
            {
                dayTimer = 0f;
                State.dayCount++;
                OnDayChanged?.Invoke(State.dayCount);
                CheckActProgression();
            }
        }

        public void ChangeAct(GameAct next)
        {
            State.currentAct = next;
            OnActChanged?.Invoke(next);
            Debug.Log($"[ACT] {next}");
        }

        private void CheckActProgression()
        {
            switch (State.currentAct)
            {
                case GameAct.Act1_Survival:
                    // акт 2 начинается, когда найдена вышка
                    if (State.HasClue("found_tower"))
                        ChangeAct(GameAct.Act2_Ranger);
                    break;

                case GameAct.Act2_Ranger:
                    // акт 3 — если доверие егеря получено ИЛИ прошло 4 дня
                    if (State.rangerTrustsPlayer || State.dayCount >= 4)
                    {
                        State.vanReturned = true;
                        ChangeAct(GameAct.Act3_VanReturns);
                    }
                    break;
            }
        }

        public void TriggerEnding(string endingId)
        {
            State.endingId = endingId;
            ChangeAct(GameAct.Final);
            SaveSystem.Save(State);
            Debug.Log($"[ENDING] {endingId}");
        }

        public void SaveGame() => SaveSystem.Save(State);
    }
}
