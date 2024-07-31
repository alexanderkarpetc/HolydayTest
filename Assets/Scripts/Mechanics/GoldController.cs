using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldController
{
    private const int startingBananaGold = 1;

    private int currentGold;
    public int CurrentGold {
        get { return currentGold; }
        private set
        {
            currentGold = value;
            GameObject.Find(GameController.canvasPath + "counter/quantity").GetComponent<TextMeshProUGUI>().text = currentGold.ToString();
            GameObject.Find(GameController.menusPath + "upgrades/upgrade (0)/upgradeButton").GetComponent<Button>().interactable =
                currentGold >= GameEntities.Upgrades.GetUpgradeCost(Upgrade.BananaGold);
            GameObject.Find(GameController.menusPath + "upgrades/upgrade (1)/upgradeButton").GetComponent<Button>().interactable =
                currentGold >= GameEntities.Upgrades.GetUpgradeCost(Upgrade.BananaSpawnTime);
        } 
    }

    public void SetGold(int Gold) {
        CurrentGold = Gold;
    }

    public void AddGold() {
        var goldGainedFromUpdates = startingBananaGold + (int)GameEntities.Upgrades.GetUpgradeEffect(Upgrade.BananaGold);
        var goldGainedFromMilestones = GameEntities.Achievements.GetBonusGold();
        CurrentGold += goldGainedFromUpdates;
        CurrentGold += goldGainedFromMilestones;
        GameEntities.SocketConnection.GetGold();
    }

    public void RemoveGold(int Gold) {
        if (Gold > 0) { 
            CurrentGold -= Gold;
        }
    }

    public void SendProgress(int bananaProgress)
    {
        GameEntities.SocketConnection.AddAchievementProgress((int)Achievement.BananasClicked, bananaProgress);
    }
}
