using TMPro;
using UnityEngine;

public class InfoPopup : BasePopup {

    protected override void AdditionalInitialize(GameObject popupGo) {
        // Any additional initialization for InfoPopup
    }

    protected override MenuName GetMenuName() {
        return MenuName.InfoPopup;
    }

    protected override void SetTexts() {
        popup.transform.Find("bg/title").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("howTheGameWorks");
        popup.transform.Find("bg/content/generalInfo").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("generalInfo");
        popup.transform.Find("bg/content/upgradeInfo").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("upgradeInfo");
        popup.transform.Find("bg/content/progressInfo").GetComponent<TextMeshProUGUI>().text = Translator.GetTranslation("progressInfo");
    }
}