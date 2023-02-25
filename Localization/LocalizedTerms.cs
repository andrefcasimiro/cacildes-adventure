namespace AF
{
    public static class LocalizedTerms
    {
        public enum LocalizedAction {
            NONE,
            PICKUP_ITEM,
            TALK,
        }

        public static string GetActionText(LocalizedAction action)
        {
            return action switch
            {
                LocalizedAction.PICKUP_ITEM => PickupItem(),
                LocalizedAction.TALK => Talk(),
                _ => "",
            };
        }

        public static string PickupItem()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Pegar item",
                _ => "Pickup item",
            };
        }

        public static string Talk()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Conversar",
                _ => "Talk",
            };
        }

        public static string Found()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cacildes encontrou",
                _ => "Found",
            };
        }

        public static string Read()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Ler",
                _ => "Read",
            };
        }
            
        public static string LearnedRecipe()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Receita aprendida: ",
                _ => "Learned recipe: ",
            };
        }

        public static string Cook()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cozinhar",
                _ => "Cook",
            };
        }

        public static string UseAlchemyTable()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Usar mesa de alquimia",
                _ => "Use Alchemy Table",
            };
        }
    }
}
