namespace AF.Events
{
    public static class EventMessages
    {
        public static readonly string ON_EQUIPMENT_CHANGED = "ON_EQUIPMENT_CHANGED";

        // Events
        public static readonly string ON_MOMENT_START = "ON_MOMENT_START";
        public static readonly string ON_MOMENT_END = "ON_MOMENT_END";
        public static readonly string ON_BOSS_BATTLE_BEGINS = "ON_BOSS_BATTLE_BEGINS";
        public static readonly string ON_BOSS_BATTLE_ENDS = "ON_BOSS_BATTLE_ENDS";

        // Quests
        public static readonly string ON_QUEST_TRACKED = "ON_QUEST_TRACKED";
        public static readonly string ON_QUESTS_PROGRESS_CHANGED = "ON_QUESTS_PROGRESS_CHANGED";

        //Day / Night
        public static readonly string ON_HOUR_CHANGED = "ON_HOUR_CHANGED";

        // Companions
        public static readonly string ON_PARTY_CHANGED = "ON_PARTY_CHANGED";

        // Flags
        public static readonly string ON_FLAGS_CHANGED = "ON_FLAGS_CHANGED";

        // Combat
        public static readonly string ON_CHARACTER_KILLED = "ON_CHARACTER_KILLED";

        // Misc
        public static readonly string ON_LEAVING_BONFIRE = "ON_LEAVING_BONFIRE";

        // Game Settings
        public static readonly string ON_GRAPHICS_QUALITY_CHANGED = "ON_GRAPHICS_QUALITY_CHANGED";

    }
}