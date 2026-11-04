using System.IO;
using UnityEngine;

namespace Van.Core
{
    public static class SaveSystem
    {
        private static string PathFile =>
            Path.Combine(Application.persistentDataPath, "van_save.json");

        public static void Save(GameState state)
        {
            var json = JsonUtility.ToJson(state, true);
            File.WriteAllText(PathFile, json);
            Debug.Log($"[SAVE] {PathFile}");
        }

        public static bool TryLoad(out GameState state)
        {
            state = null;
            if (!File.Exists(PathFile)) return false;
            try
            {
                var json = File.ReadAllText(PathFile);
                state = JsonUtility.FromJson<GameState>(json);
                return state != null;
            }
            catch { return false; }
        }

        public static void Delete()
        {
            if (File.Exists(PathFile)) File.Delete(PathFile);
        }
    }
}
