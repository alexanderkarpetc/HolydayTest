using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup {

    #region Fields
    private Button infoCloseButton;
    private Button infoBackgroundClose;
    private GameObject infoPopUp;
    private RectTransform infoPopUpRect;

    public bool infoMenuIsOpen;

    private float animationDuration = 0.3f;
    #endregion

    #region Initialize
    public void Initialize(GameObject popupGo) {
        infoPopUp = popupGo;
        infoPopUpRect = popupGo.transform.Find("bg").GetComponent<RectTransform>();

        infoCloseButton = popupGo.transform.Find("bg/closeButton").GetComponent<Button>();
        infoCloseButton.onClick.AddListener(ClosePopup);

        infoBackgroundClose = popupGo.transform.Find("background").GetComponent<Button>();
        infoBackgroundClose.onClick.AddListener(ClosePopup);
    }
    #endregion

    public void OpenPopup() {
        GameEntities.GameController.StartCoroutine(GameEntities.GameController.InitializeMenu(MenuName.InfoPopup, Initialize, OpenPopupActions));
        SetTexts();
    }
    public void SetTexts() {
        infoPopUp.transform.Find("bg/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("howTheGameWorks");
        infoPopUp.transform.Find("bg/content/generalInfo").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("generalInfo");
        infoPopUp.transform.Find("bg/content/upgradeInfo").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("upgradeInfo");
        infoPopUp.transform.Find("bg/content/progressInfo").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("progressInfo");
    }

    private void OpenPopupActions() {

        if (!infoMenuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(true, infoPopUpRect, Vector3.zero, Vector3.one));
            infoPopUp.transform.SetAsLastSibling();
            infoMenuIsOpen = true;
        }
    }

    public void ClosePopup() {

        if (infoMenuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(false, infoPopUpRect, Vector3.one, Vector3.zero));
            infoMenuIsOpen = false;
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