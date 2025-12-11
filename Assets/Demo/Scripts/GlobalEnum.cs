public class GlobalEnum
{
    public enum SkillType
    {
        Boomb,
    }
    public enum SceneIndex
    {
        LoginScene = 0,
        Lobby = 1,
        Chapter1 = 2,
        Chapter2 = 3,
        Chapter3 = 4,
        Unknown = 999,
    }
    public enum BallisticFireMode
    {
        UseLaunchSpeed,
        UseLaunchAngle
    }
    public enum PanelState
    {
        Hide, Show
    }
    public enum EquipmentSlot
    {
        Head,
        Torso,
        Legs,
        Feet,
        Accessory,
        Weapon
    }
    public enum CurrencyType
    {
        Gold,
        Gems,
        // use real money to buy gems
        USD
    }
    public enum ShopItemType
    {
        // soft currency (in-game)
        Gold,

        // hard currency (buy with real money)
        Gems,

        // used in gameplay
        HealthPotion,

        // levels up the character (PowerPotion)
        LevelUpPotion
    }

    public enum LevelState
    {
        Intro,
        Building,
        SpawningEnemies,
        AllEnemiesSpawned,
        Lose,
        Win,
        Out,
    }
    public enum State
    {
        /// <summary>
        /// The game is in its normal state. Here the player can pan the camera, select units and towers
        /// </summary>
        Normal,

        /// <summary>
        /// The game is Paused. Here, the player can restart the level, or quit to the main menu
        /// </summary>
        Paused,

        /// <summary>
        /// The game is over and the level was failed/completed
        /// </summary>
        GameOver,
    }/// <summary>
     /// Ballistic arc calculation priorities/preferences.
     /// </summary>
    public enum BallisticArcHeight
    {
        /// <summary>
        /// High "underarm" arc
        /// </summary>
        UseHigh,

        /// <summary>
        /// Low "overarm" arc
        /// </summary>
        UseLow,

        /// <summary>
        /// Use high arc if valid, fall back to low if possible.
        /// </summary>
        PreferHigh,

        /// <summary>
        /// Use low arc if valid, fall back to high if possible.
        /// </summary>
        PreferLow
    }
}
