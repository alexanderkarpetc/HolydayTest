using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsPopup : BasePopup {

    private ToggleGroup languageToggleGroup;
    private TextMeshProUGUI totalBananasClicked;
    private RectTransform totalBananasClickedProgress;
    private TextMeshProUGUI totalBananaUpgraded;
    private RectTransform totalBananasUpgradedProgress;
    private TextMeshProUGUI bananaIncomePerClicks;
    private TextMeshProUGUI bananaIncomePerUpgrades;
    private float initialMaskWidth;

    protected override void AdditionalInitialize(GameObject popupGo) {
        InitProgressElems(popupGo);
        InitLanguage(popupGo);
    }

    protected override MenuName GetMenuName() {
        return MenuName.AchievementsPopup;
    }

    protected override void SetTexts() {
        popup.transform.Find("bg/totalBananas/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("totalBananaClicks");
        popup.transform.Find("bg/totalUpgrades/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("totalBananaUpgrades");
        popup.transform.Find("bg/languageChange/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("currentLanguage");
    }

    private void InitProgressElems(GameObject popupGo) {
        totalBananasClicked = popupGo.transform.Find("bg/totalBananas/progress/progressText").GetComponent<TextMeshProUGUI>();
        bananaIncomePerClicks = popupGo.transform.Find("bg/totalBananas/bananasIncome/incomeText").GetComponent<TextMeshProUGUI>();
        totalBananasClickedProgress = popupGo.transform.Find("bg/totalBananas/progress/Mask").GetComponent<RectTransform>();
        initialMaskWidth = totalBananasClickedProgress.sizeDelta.x;

        totalBananaUpgraded = popupGo.transform.Find("bg/totalUpgrades/progress/progressText").GetComponent<TextMeshProUGUI>();
        totalBananasUpgradedProgress = popupGo.transform.Find("bg/totalUpgrades/progress/Mask").GetComponent<RectTransform>();
        bananaIncomePerUpgrades = popupGo.transform.Find("bg/totalUpgrades/bananasIncome/incomeText").GetComponent<TextMeshProUGUI>();
    }

    private void InitLanguage(GameObject popupGo) {
        languageToggleGroup = popupGo.transform.Find("bg/languageChange/Options").GetComponent<ToggleGroup>();
        SelectToggle(GameEntities.LanguageController.CurrentLanguage);
        foreach (var toggle in languageToggleGroup.GetComponentsInChildren<Toggle>()) {
            toggle.onValueChanged.AddListener((isSelected) => {
                if (isSelected) {
                    GameEntities.LanguageController.ChangeLanguage((int)(Languages)System.Enum.Parse(typeof(Languages), toggle.name));
                    SetTexts();
                }
            });
        }
    }

    public void SelectToggle(int language) {
        foreach (var toggle in languageToggleGroup.GetComponentsInChildren<Toggle>()) {
            if (toggle.name == ((Languages)language).ToString()) {
                toggle.isOn = true;
                break;
            }
        }
    }

    private void SetProgress() {
        var clickMilestoneBonus = 0;
        var bananaClicks = GameEntities.Achievements.achievementProgress[Achievement.BananasClicked];
        var nextMilestone = Achievements.achievementMilestones[Achievement.BananasClicked][0];

        foreach (var milestone in Achievements.achievementMilestones[Achievement.BananasClicked]) {
            clickMilestoneBonus++;

            if (bananaClicks < milestone) {
                nextMilestone = milestone;
                break;
            }
        }
        totalBananasClicked.text = $"{bananaClicks}/{nextMilestone}";
        bananaIncomePerClicks.text = $"+{clickMilestoneBonus}";
        totalBananasClickedProgress.sizeDelta = new Vector2(initialMaskWidth * (bananaClicks / (float)nextMilestone), totalBananasClickedProgress.sizeDelta.y);

        var upgradeMilestoneBonus = 0;
        var bananaUpgrades = GameEntities.Achievements.achievementProgress[Achievement.BananaUpgrades];
        var nextUpgradeMilestone = Achievements.achievementMilestones[Achievement.BananaUpgrades][0];

        foreach (var milestone in Achievements.achievementMilestones[Achievement.BananaUpgrades]) {
            upgradeMilestoneBonus++;

            if (bananaUpgrades < milestone) {
                nextUpgradeMilestone = milestone;
                break;
            }
        }
        totalBananaUpgraded.text = $"{bananaUpgrades}/{nextUpgradeMilestone}";
        bananaIncomePerUpgrades.text = $"+{upgradeMilestoneBonus}";
        totalBananasUpgradedProgress.sizeDelta = new Vector2(initialMaskWidth * (bananaUpgrades / (float)nextUpgradeMilestone), totalBananasUpgradedProgress.sizeDelta.y);
    }

    public override void OpenPopup() {
        base.OpenPopup();
        SetProgress();
    }
}