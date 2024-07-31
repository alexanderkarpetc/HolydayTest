using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class AchievementsPopup {

    #region Fields
    private Button achievementsCloseButton;
    private Button achievementsBackgroundClose;
    private GameObject achievementsPopUp;
    private RectTransform achievementsPopUpRect;
    private ToggleGroup languageToggleGroup;
    private TextMeshProUGUI totalBananasClicked;
    private RectTransform totalBananasClickedProgress;
    private TextMeshProUGUI totalBananaUpgraded;
    private RectTransform totalBananasUpgradedProgress;
    private TextMeshProUGUI bananaIncomePerClicks;
    private TextMeshProUGUI bananaIncomePerUpgrades;
    private float initialMaskWidth;
    public bool achievementsMenuIsOpen;

    private float animationDuration = 0.3f;
    #endregion

    #region Initialize
    public void Initialize(GameObject popupGo) {
        achievementsPopUp = popupGo;
        achievementsPopUpRect = popupGo.transform.Find("bg").GetComponent<RectTransform>();

        achievementsCloseButton = popupGo.transform.Find("bg/closeButton").GetComponent<Button>();
        achievementsCloseButton.onClick.AddListener(ClosePopup);

        achievementsBackgroundClose = popupGo.transform.Find("background").GetComponent<Button>();
        achievementsBackgroundClose.onClick.AddListener(ClosePopup);
        InitProgressElems(popupGo);
        InitLanguage(popupGo);
    }

    private void InitProgressElems(GameObject popupGo)
    {
        totalBananasClicked = popupGo.transform.Find("bg/totalBananas/progress/progressText").GetComponent<TextMeshProUGUI>();
        bananaIncomePerClicks = popupGo.transform.Find("bg/totalBananas/bananasIncome/incomeText").GetComponent<TextMeshProUGUI>();
        totalBananasClickedProgress = popupGo.transform.Find("bg/totalBananas/progress/Mask").GetComponent<RectTransform>();
        initialMaskWidth = totalBananasClickedProgress.sizeDelta.x;
        
        totalBananaUpgraded = popupGo.transform.Find("bg/totalUpgrades/progress/progressText").GetComponent<TextMeshProUGUI>();
        totalBananasUpgradedProgress = popupGo.transform.Find("bg/totalUpgrades/progress/Mask").GetComponent<RectTransform>();
        bananaIncomePerUpgrades = popupGo.transform.Find("bg/totalUpgrades/bananasIncome/incomeText").GetComponent<TextMeshProUGUI>();
    }

    private void InitLanguage(GameObject popupGo)
    {
        languageToggleGroup = popupGo.transform.Find("bg/languageChange/Options").GetComponent<ToggleGroup>();
        SelectToggle(GameEntities.LanguageController.CurrentLanguage);
        foreach (var toggle in languageToggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.onValueChanged.AddListener((isSelected) =>
            {
                if(isSelected)
                    GameEntities.LanguageController.ChangeLanguage((int)(Languages)System.Enum.Parse(typeof(Languages), toggle.name));
                SetTexts();
            });
        }
    }

    #endregion
    public void SelectToggle(int language)
    {
        foreach (var toggle in languageToggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.name == ((Languages)language).ToString())
            {
                toggle.isOn = true;
                break;
            }
        }
    }

    public void OpenPopup() {
        GameEntities.GameController.StartCoroutine(GameEntities.GameController.InitializeMenu(MenuName.AchievementsPopup, Initialize, OpenPopupActions));
        SetProgress();
        SetTexts();
    }

    private void SetProgress()
    {
        var clickMilestoneBonus = 0;
        var bananaClicks = GameEntities.Achievements.achievementProgress[Achievement.BananasClicked];

        var nextMilestone = Achievements.achievementMilestones[Achievement.BananasClicked][0];

        foreach (var milestone in Achievements.achievementMilestones[Achievement.BananasClicked])
        {
            clickMilestoneBonus++;

            if (bananaClicks < milestone)
            {
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

        foreach (var milestone in Achievements.achievementMilestones[Achievement.BananaUpgrades])
        {
            upgradeMilestoneBonus++;

            if (bananaUpgrades < milestone)
            {
                nextUpgradeMilestone = milestone;
                break;
            }
        }
        totalBananaUpgraded.text = $"{bananaUpgrades}/{nextUpgradeMilestone}";
        bananaIncomePerUpgrades.text = $"+{upgradeMilestoneBonus}";
        totalBananasUpgradedProgress.sizeDelta = new Vector2(initialMaskWidth * (bananaUpgrades / (float)nextUpgradeMilestone), totalBananasUpgradedProgress.sizeDelta.y);
    }

    public void SetTexts() {
        achievementsPopUp.transform.Find("bg/totalBananas/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("totalBananaClicks");
        achievementsPopUp.transform.Find("bg/totalUpgrades/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("totalBananaUpgrades");
        achievementsPopUp.transform.Find("bg/languageChange/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("currentLanguage");
    }
    
    private void OpenPopupActions() {

        if (!achievementsMenuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(true, achievementsPopUpRect, Vector3.zero, Vector3.one));
            achievementsPopUp.transform.SetAsLastSibling();
            achievementsMenuIsOpen = true;
        }
        
    }

    public void ClosePopup() {

        if (achievementsMenuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(false, achievementsPopUpRect, Vector3.one, Vector3.zero));
            achievementsMenuIsOpen = false;
        }
    }

    private IEnumerator TogglePopupWithZoomAnimation(bool Open, RectTransform PopupRect, Vector3 StartScale, Vector3 FinalScale) {

        PopupRect.localScale = StartScale;

        if (Open) {
            PopupRect.DOKill(true);
            PopupRect.parent.gameObject.SetActive(true);
        }

        yield return new WaitForEndOfFrame();

        PopupRect.DOScale(FinalScale, animationDuration).OnComplete(() => { if (!Open) PopupRect.parent.gameObject.SetActive(false); });
    }
}