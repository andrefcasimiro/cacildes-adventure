using UnityEngine;
using Input = UnityEngine.Input;
using CI.QuickSave;
using System.Linq;
using UnityEngine.SceneManagement;
using AYellowpaper.SerializedCollections;

namespace AF
{
    public class SaveManager : MonoBehaviour
    {

        [Header("Databases")]
        public PickupDatabase pickupDatabase;

        [Header("Components")]
        public FadeManager fadeManager;

        public bool HasSavedGame()
        {
            return QuickSaveReader.Create("Scene").Exists("sceneIndex");
        }

        public void SaveGameData()
        {
            SavePickups();
            SaveSceneSettings();
        }

        public void LoadLastSavedGame()
        {
            fadeManager.FadeIn(1f, () =>
            {
                LoadPickups();
                LoadSceneSettings();
            });
        }

        void SavePickups()
        {
            var pickups = QuickSaveWriter.Create("Pickups");
            pickups.Write("pickups", pickupDatabase.pickups.ToArray());
            pickups.TryCommit();
        }
        void LoadPickups()
        {
            pickupDatabase.Clear();

            var pickups = QuickSaveReader.Create("Pickups");
            pickups.Read<SerializedDictionary<string, string>>("pickups", (pickups) =>
            {
                pickupDatabase.pickups = pickups;
            });
        }

        void SaveSceneSettings()
        {
            var data = QuickSaveWriter.Create("Scene");
            data.Write("sceneIndex", SceneManager.GetActiveScene().buildIndex);
            data.TryCommit();
        }

        void LoadSceneSettings()
        {
            var data = QuickSaveReader.Create("Scene");
            data.Read<int>("sceneIndex", (sceneIndex) =>
            {
                SceneManager.LoadScene(sceneIndex);
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGameData();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadLastSavedGame();
            }
        }
    }
}
