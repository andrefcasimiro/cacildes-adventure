using System.Collections.Generic;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF
{

    public class PlayerAppearance : MonoBehaviour
    {
        // COLORS
        public readonly string HAIR_COLOR_KEY = "HAIR_COLOR";
        public readonly string BODY_COLOR_KEY = "BODY_COLOR";
        public readonly string EYE_COLOR_KEY = "EYE_COLOR";
        public readonly string TATTOO_COLOR_KEY = "TATTOO_COLOR";

        // TYPES
        public readonly string BODY_TYPE_KEY = "BODY_TYPE";
        public readonly string HAIR_TYPE_KEY = "HAIR_TYPE";
        public readonly string FACE_TYPE_KEY = "FACE_TYPE";
        public readonly string EYEBROW_TYPE_KEY = "EYEBROW_TYPE";
        public readonly string BEARD_TYPE_KEY = "BEARD_TYPE";

        public readonly string PORTRAIT_KEY = "PORTRAIT";
        [Header("Available Portraits")]
        public Sprite[] portraits;
        public Sprite defaultPortrait;


        [Header("Naked and Armor Materials")]
        public Material[] playerMaterials;

        private readonly string defaultPlayerName = "Cacildes";
        private readonly string defaultHairColor = "#4F412D";
        private readonly string defaultBodyColor = "#FFCCAE";
        private readonly string defaultEyeColor = "#000000";
        private readonly string defaultTattooColor = "#874FA5";

        public UIDocumentCharacterCustomization uIDocumentCharacterCustomization;

        [Header("Body Containers")]
        public GameObject hairContainer;
        public GameObject eyebrowContainer;
        public GameObject beardContainer;
        public GameObject headContainer;
        public GameObject torsoContainer;
        public GameObject upperRightArmContainer;
        public GameObject upperLeftArmContainer;
        public GameObject lowerRightArmContainer;
        public GameObject lowerLeftArmContainer;
        public GameObject rightHandContainer;
        public GameObject leftHandContainer;

        public GameObject hipContainer;
        public GameObject leftLegContainer;
        public GameObject rightLegContainer;


        List<GameObject> bodyContainers = new();

        public GameSettings gameSettings;


        private void Awake()
        {
            bodyContainers = new() {
                torsoContainer, upperLeftArmContainer, upperRightArmContainer, lowerLeftArmContainer, lowerRightArmContainer,
                leftHandContainer, rightHandContainer, hipContainer, leftLegContainer, rightLegContainer };

            UpdateAppearance();
        }

        public void ActivatePlayerAppearanceManager()
        {
            if (uIDocumentCharacterCustomization.isActiveAndEnabled)
            {
                uIDocumentCharacterCustomization.gameObject.SetActive(false);
                return;
            }

            uIDocumentCharacterCustomization.gameObject.SetActive(true);
        }

        public Sprite GetPlayerPortrait()
        {
            if (!PlayerPrefs.HasKey(PORTRAIT_KEY))
            {
                return defaultPortrait;
            }

            return portraits[PlayerPrefs.GetInt(PORTRAIT_KEY)];
        }
        public int GetPlayerPortraitIndex()
        {
            if (!PlayerPrefs.HasKey(PORTRAIT_KEY))
            {
                return 0;
            }

            return PlayerPrefs.GetInt(PORTRAIT_KEY);
        }

        public void SetPlayerPortrait(int value)
        {
            PlayerPrefs.SetInt(PORTRAIT_KEY, value);
            PlayerPrefs.Save();
        }

        public string GetPlayerName()
        {
            return gameSettings.GetPlayerName();
        }

        public void UpdatePlayerName(string playerName)
        {
            gameSettings.SetPlayerName(playerName);
            UpdateAppearance();
        }


        public string GetHairColor()
        {
            if (!PlayerPrefs.HasKey(HAIR_COLOR_KEY))
            {
                return defaultHairColor;
            }

            return PlayerPrefs.GetString(HAIR_COLOR_KEY);
        }

        public void UpdateHairColor(string color)
        {
            PlayerPrefs.SetString(HAIR_COLOR_KEY, color);
            PlayerPrefs.Save();
            UpdateAppearance();
        }

        public string GetBodyColor()
        {
            if (!PlayerPrefs.HasKey(BODY_COLOR_KEY))
            {
                return defaultBodyColor;
            }

            return PlayerPrefs.GetString(BODY_COLOR_KEY);
        }

        public void UpdateBodyColor(string color)
        {
            PlayerPrefs.SetString(BODY_COLOR_KEY, color);
            PlayerPrefs.Save();
            UpdateAppearance();
        }

        public string GetEyeColor()
        {
            if (!PlayerPrefs.HasKey(EYE_COLOR_KEY))
            {
                return defaultEyeColor;
            }

            return PlayerPrefs.GetString(EYE_COLOR_KEY);
        }

        public void UpdateEyeColor(string color)
        {
            PlayerPrefs.SetString(EYE_COLOR_KEY, color);
            PlayerPrefs.Save();
            UpdateAppearance();
        }

        public string GetTattooColor()
        {
            if (!PlayerPrefs.HasKey(TATTOO_COLOR_KEY))
            {
                return defaultTattooColor;
            }

            return PlayerPrefs.GetString(TATTOO_COLOR_KEY);
        }

        public void UpdateTattooColor(string color)
        {
            PlayerPrefs.SetString(TATTOO_COLOR_KEY, color);
            PlayerPrefs.Save();
            UpdateAppearance();
        }

        public void SetBodyType(int newValue)
        {
            PlayerPrefs.SetInt(BODY_TYPE_KEY, Mathf.Clamp(newValue, 0, 1));
            PlayerPrefs.Save();

            EventManager.EmitEvent(EventMessages.ON_BODY_TYPE_CHANGED);

            UpdateAppearance();
        }

        public int GetBodyType()
        {
            if (!PlayerPrefs.HasKey(BODY_TYPE_KEY))
            {
                return 0;
            }

            return PlayerPrefs.GetInt(BODY_TYPE_KEY);
        }

        public void SetFaceType(int newValue)
        {
            PlayerPrefs.SetInt(FACE_TYPE_KEY, newValue);
            PlayerPrefs.Save();

            UpdateAppearance();
        }

        public int GetFaceType()
        {
            if (!PlayerPrefs.HasKey(FACE_TYPE_KEY))
            {
                return 0;
            }

            return PlayerPrefs.GetInt(FACE_TYPE_KEY);
        }
        public void SetHairType(int newValue)
        {
            PlayerPrefs.SetInt(HAIR_TYPE_KEY, newValue);
            PlayerPrefs.Save();

            UpdateAppearance();
        }

        public int GetHairType()
        {
            if (!PlayerPrefs.HasKey(HAIR_TYPE_KEY))
            {
                return 0;
            }

            return PlayerPrefs.GetInt(HAIR_TYPE_KEY);
        }

        public void SetEyebrowType(int newValue)
        {
            PlayerPrefs.SetInt(EYEBROW_TYPE_KEY, newValue);
            PlayerPrefs.Save();

            UpdateAppearance();
        }

        public int GetEyebrowType()
        {
            if (!PlayerPrefs.HasKey(EYEBROW_TYPE_KEY))
            {
                return 0;
            }

            return PlayerPrefs.GetInt(EYEBROW_TYPE_KEY);
        }

        public void SetBeardType(int newValue)
        {
            PlayerPrefs.SetInt(BEARD_TYPE_KEY, newValue);
            PlayerPrefs.Save();

            UpdateAppearance();
        }

        public int GetBeardType()
        {
            if (!PlayerPrefs.HasKey(BEARD_TYPE_KEY))
            {
                return 0;
            }

            return PlayerPrefs.GetInt(BEARD_TYPE_KEY);
        }


        void UpdateAppearance()
        {
            LoadColors();
            LoadBodyType();
            LoadFaceType();
            LoadHairType();
            LoadEyebrowType();
            LoadBeardType();
        }

        void LoadBodyType()
        {
            int type = GetBodyType();

            foreach (GameObject bodyGameObject in bodyContainers)
            {
                for (int i = 0; i < bodyGameObject.transform.childCount; i++)
                {
                    bodyGameObject.transform.GetChild(i).gameObject.SetActive(type == i);
                }
            }
        }

        void LoadFaceType()
        {
            int type = GetFaceType();
            for (int i = 0; i < headContainer.transform.childCount; i++)
            {
                headContainer.transform.GetChild(i).gameObject.SetActive(type == i);
            }
        }

        void LoadHairType()
        {
            int type = GetHairType();
            for (int i = 0; i < hairContainer.transform.childCount; i++)
            {
                bool value = type == i;
                GameObject targetGameObject = hairContainer.transform.GetChild(i).gameObject;
                targetGameObject.SetActive(value);
            }
        }

        void LoadEyebrowType()
        {
            int type = GetEyebrowType();
            for (int i = 0; i < eyebrowContainer.transform.childCount; i++)
            {
                eyebrowContainer.transform.GetChild(i).gameObject.SetActive(type == i);
            }
        }

        void LoadBeardType()
        {
            int type = GetBeardType();
            for (int i = 0; i < beardContainer.transform.childCount; i++)
            {
                beardContainer.transform.GetChild(i).gameObject.SetActive(type == i);
            }
        }

        void LoadColors()
        {
            foreach (var playerMaterial in playerMaterials)
            {
                ColorUtility.TryParseHtmlString(GetHairColor(), out var hairColor);
                playerMaterial.SetColor("_Color_Hair", hairColor);

                ColorUtility.TryParseHtmlString(GetBodyColor(), out var bodyColor);
                playerMaterial.SetColor("_Color_Skin", bodyColor);
                playerMaterial.SetColor("_Color_Stubble", bodyColor);

                ColorUtility.TryParseHtmlString(GetEyeColor(), out var eyeColor);
                playerMaterial.SetColor("_Color_Eyes", eyeColor);

                ColorUtility.TryParseHtmlString(GetTattooColor(), out var tattooColor);
                playerMaterial.SetColor("_Color_BodyArt", tattooColor);
            }
        }

        public void ResetDefaults()
        {
            UpdatePlayerName(defaultPlayerName);
            SetBodyType(0);
            SetHairType(0);
            SetFaceType(0);
            SetBeardType(0);
            SetEyebrowType(0);
            UpdateBodyColor(defaultBodyColor);
            UpdateHairColor(defaultHairColor);
            UpdateEyeColor(defaultEyeColor);
            UpdateTattooColor(defaultTattooColor);
            SetPlayerPortrait(0);
        }
    }
}
