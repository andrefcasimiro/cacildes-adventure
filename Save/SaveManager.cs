using UnityEngine;
using Input = UnityEngine.Input;
using CI.QuickSave;
using System.Linq;
using UnityEngine.SceneManagement;

namespace AF
{
    public class SaveManager : MonoBehaviour
    {

        [Header("Databases")]
        public PickupDatabase pickupDatabase;

        public void SaveGameData()
        {
            SavePickups();
            SaveSceneSettings();
        }

        public void LoadLastSavedGame()
        {
            LoadPickups();
            LoadSceneSettings();
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
            pickups.Read<PickupDatabase.PickupData[]>("pickups", (pickups) =>
            {
                pickupDatabase.pickups = pickups.ToList();
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
