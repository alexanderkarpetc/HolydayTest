using UnityEngine;

public class GameEntities {

    private static GameController gameController;
    public static GameController GameController {
        get {
            if (gameController == null) {
                gameController = GameObject.Find("GameController").GetComponent<GameController>();
            }
            return gameController;
        }
    }

    private static SocketConnection socketConnection;
    public static SocketConnection SocketConnection => socketConnection ??= new SocketConnection();

    private static GoldController goldController;
    public static GoldController GoldController => goldController ??= new GoldController();
    
    private static LanguageController languageController;
    public static LanguageController LanguageController => languageController ??= new LanguageController();

    private static BananaController bananaController;
    public static BananaController BananaController => bananaController ??= new BananaController();

    private static Upgrades upgrades;
    public static Upgrades Upgrades => upgrades ??= new Upgrades();
    
    private static Achievements achievements;
    public static Achievements Achievements => achievements ??= new Achievements();

    private static InfoPopup infoPopup;
    public static InfoPopup InfoPopup => infoPopup ??= new InfoPopup();

    private static AchievementsPopup achievementsPopup;
    public static AchievementsPopup AchievementsPopup => achievementsPopup ??= new AchievementsPopup();
}
