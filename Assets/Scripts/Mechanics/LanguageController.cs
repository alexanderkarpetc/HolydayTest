using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController
{
    private int currentLanguage;
    public int CurrentLanguage {
        get { return currentLanguage; }
        private set
        {
            currentLanguage = value;
        } 
    }

    public void SetLanguage(int language)
    {
        CurrentLanguage = language;
    }

    public void ChangeLanguage(int language)
    {
        SetLanguage(language);
        GameEntities.SocketConnection.ChangeLanguage(currentLanguage);
    }
}
