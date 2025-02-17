﻿using System;
using System.Collections.Generic;

public class Achievements {

    private const int bananaGoldIncreasePerMilestone = 1;

    private static readonly List<int> bananaClicks = new List<int>()
    {
        10,
        20,
        30,
        40,
        50,
        60,
        70,
        80,
        90,
        100,
        150,
        200,
        250,
        300,
        350,
        400,
        450,
        500,
        600,
        700,
        800,
        900,
        1000,
        1500,
        2000,
        2500,
        3000,
        3500,
        4000,
        4500,
        5000,
        6000,
        7000,
        8000,
        9000,
        10000,
    };

    private static readonly List<int> bananaUpgrades = new List<int>()
    {
        1,
        3,
        5,
        8,
        11,
        14,
        17,
        20,
        23,
        26,
        29,
        32,
        36,
        40,
        44,
        48,
        52,
        56,
        60,
        64,
        68,
        72,
    };

    public static readonly Dictionary<Achievement, List<int>> achievementMilestones = new Dictionary<Achievement, List<int>>() {
        { Achievement.BananasClicked,       bananaClicks },
        { Achievement.BananaUpgrades,       bananaUpgrades },
    };

    public Dictionary<Achievement, int> achievementProgress = new Dictionary<Achievement, int>()
    {
        {Achievement.BananasClicked, 0},
        {Achievement.BananaUpgrades, 0},
    };

    public int GetBonusGold()
    {
        var bonusGold = 0;
        var index = 0;
        while(achievementProgress[Achievement.BananasClicked] >= bananaClicks[index] && index < bananaClicks.Count)
        {
            bonusGold += bananaGoldIncreasePerMilestone;
            index++;
        }
        index = 0;
        while(achievementProgress[Achievement.BananaUpgrades] >= bananaUpgrades[index] && index < bananaUpgrades.Count)
        {
            bonusGold += bananaGoldIncreasePerMilestone;
            index++;
        }
        
        return bonusGold;
    }

    public void BananaClicked()
    {
        achievementProgress[Achievement.BananasClicked]++;
    }
    public void UpgradeMade()
    {
        achievementProgress[Achievement.BananaUpgrades]++;
    }
}

public enum Achievement { 
    BananasClicked,
    BananaUpgrades
}

[Serializable]
public class AchievementList {
    public List<AchievementServer> list = new List<AchievementServer>();
}

[Serializable]
public class AchievementServer {
    public Achievement code;
    public int number;
}