using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public abstract class BasePopup {

    #region Fields
    protected Button closeButton;
    protected Button backgroundCloseButton;
    protected GameObject popup;
    protected RectTransform popupRect;
    protected bool menuIsOpen;

    protected float animationDuration = 0.3f;
    #endregion

    #region Initialize
    public virtual void Initialize(GameObject popupGo) {
        popup = popupGo;
        popupRect = popupGo.transform.Find("bg").GetComponent<RectTransform>();

        closeButton = popupGo.transform.Find("bg/closeButton").GetComponent<Button>();
        closeButton.onClick.AddListener(ClosePopup);

        backgroundCloseButton = popupGo.transform.Find("background").GetComponent<Button>();
        backgroundCloseButton.onClick.AddListener(ClosePopup);

        AdditionalInitialize(popupGo);
    }

    protected abstract void AdditionalInitialize(GameObject popupGo);
    #endregion

    public virtual void OpenPopup() {
        GameEntities.GameController.StartCoroutine(GameEntities.GameController.InitializeMenu(GetMenuName(), Initialize, OpenPopupActions));
        SetTexts();
    }

    protected abstract MenuName GetMenuName();
    protected abstract void SetTexts();

    private void OpenPopupActions() {
        if (!menuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(true, popupRect, Vector3.zero, Vector3.one));
            popup.transform.SetAsLastSibling();
            menuIsOpen = true;
        }
    }

    public void ClosePopup() {
        if (menuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(false, popupRect, Vector3.one, Vector3.zero));
            menuIsOpen = false;
        }
    }

    private IEnumerator TogglePopupWithZoomAnimation(bool open, RectTransform popupRect, Vector3 startScale, Vector3 finalScale) {
        popupRect.localScale = startScale;

        if (open) {
            popupRect.DOKill(true);
            popupRect.parent.gameObject.SetActive(true);
        }

        yield return new WaitForEndOfFrame();

        popupRect.DOScale(finalScale, animationDuration).OnComplete(() => { if (!open) popupRect.parent.gameObject.SetActive(false); });
    }
}