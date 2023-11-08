using System.Collections.Generic;
using UnityEngine;
namespace AF
{

    public class TutorialsManager : MonoBehaviour
    {

        [System.Serializable]
        public class TutorialEntry
        {
            public string title;

            public string text;
        }

        [System.Serializable]
        public class TutorialCategory
        {
            public string category;
            public List<TutorialEntry> tutorials;
        }

        public GameObject portuguese;
        public GameObject english;

        public List<TutorialCategory> englishTutorialCategories;
        public List<TutorialCategory> portugueseTutorialCategories;

        private bool hasLoaded = false;
        
        private void LoadTutorialsForLanguage(Transform languageTransform, List<TutorialCategory> tutorialCategoriesList)
        {
            foreach (Transform tutorialCategories in languageTransform)
            {
                TutorialCategory t = new TutorialCategory();
                t.category = tutorialCategories.gameObject.name;
                t.tutorials = new List<TutorialEntry>();

                foreach (Transform tutorial in tutorialCategories.transform)
                {
                    t.tutorials.Add(new TutorialEntry()
                    {
                        title = tutorial.gameObject.name,
                        text =  tutorial.GetComponent<AF.Comment>().text,
                    });
                }

                tutorialCategoriesList.Add(t);
            }
        }

        private void LoadTutorials()
        {
            LoadTutorialsForLanguage(portuguese.transform, portugueseTutorialCategories);
            LoadTutorialsForLanguage(english.transform, englishTutorialCategories);
        }

        public List<TutorialCategory> GetTutorials()
        {
            // Load on demand
            if (!hasLoaded)
            {
                LoadTutorials();
                hasLoaded = true;
            }

            if (GamePreferences.instance.IsEnglish())
            {
                return englishTutorialCategories;
            }

            return portugueseTutorialCategories;
        }
        
    }
}
