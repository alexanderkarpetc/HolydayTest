using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public const string canvasPath = "canvas/";
    public const string menusPath = canvasPath + "menus/";
    public const float bananaRushCooldown = 10;
    public const float bananaRushSpawnDuration = 3;

    [Serializable]
    public class UIData {
        public MenuName menu;
        public GameObject prefab;
        [HideInInspector] public bool hasInitialized;
    }

    public UIData[] menuPrefabs;

    private List<UIData> allUI = new List<UIData>();

    private bool gameInitialised = false;

    private GameObject bananaPrefab;
    private Vector4 bananaSpawnEdges = Vector4.zero;
    private const int spawnPosModifier = 200;
    private int bananaProgress;
    private Vector2 previousScreenSize;
    private Button bananaRushButton;
    private Image bananaRushCooldownImage;
    private List<Transform> _rushBananas = new List<Transform>();

    public void Start() {
        foreach (UIData menuPrefab in menuPrefabs) {
            allUI.Add(menuPrefab);
        }

        RecalculateBananaSpawnPlaces();

        previousScreenSize = new Vector2(Screen.width, Screen.height);

        StartCoroutine(GameEntities.SocketConnection.ConnectToSocket());
    }

    private void RecalculateBananaSpawnPlaces()
    {
        var minPosition = Camera.main.ScreenToWorldPoint(new Vector3(spawnPosModifier, spawnPosModifier));
        var maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - spawnPosModifier, Screen.height - spawnPosModifier));

        bananaSpawnEdges = new Vector4(minPosition.x, minPosition.y, maxPosition.x, maxPosition.y);
    }

    public void Update() {
        if (gameInitialised) { 

            GameEntities.BananaController.UpdateSpawnTime(); 
            if(bananaProgress >= 10) {
                GameEntities.GoldController.SendProgress(bananaProgress);
                bananaProgress = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //Raycast to see if colliders are hit.
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);

                if (hit.collider) //Something was hit
                {
                    GameObject hitGameObject = hit.collider.gameObject; //banana game object
                    GameEntities.GoldController.AddGold();
                    GameEntities.Achievements.BananaClicked();
                    bananaProgress++;
                    Destroy(hitGameObject);
                }
            }

            UpdateRushBananas();
        }

        var currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize != previousScreenSize) {
            RecalculateBananaSpawnPlaces();
            previousScreenSize = currentScreenSize;
        }
    }

    private void OnDestroy()
    {
        if (bananaProgress > 0)
        {
            GameEntities.GoldController.SendProgress(bananaProgress);
        }
    }

    public void OnUserDataSet() {

        GameObject.Find(canvasPath + "infoButton").GetComponent<Button>().onClick.AddListener(() => {
            GameEntities.InfoPopup.OpenPopup();
        });
        GameObject.Find(canvasPath + "playerAvatar").GetComponent<Button>().onClick.AddListener(() => {
            GameEntities.AchievementsPopup.OpenPopup();
        });
        GameObject.Find(canvasPath + "counter/quantity").GetComponent<TextMeshProUGUI>().text = GameEntities.GoldController.CurrentGold.ToString();

        GameObject.Find(menusPath + "upgrades/upgrade (0)/upgradeButton").GetComponent<Button>().onClick.AddListener(() => {
            GameEntities.Upgrades.PerformUpgrade(Upgrade.BananaGold);
        });
        GameObject.Find(menusPath + "upgrades/upgrade (0)/upgradeButton").GetComponent<Button>().interactable = GameEntities.GoldController.CurrentGold >= GameEntities.Upgrades.GetUpgradeCost(Upgrade.BananaGold);
        GameObject.Find(menusPath + "upgrades/upgrade (0)/upgradeButton").SetActive(!GameEntities.Upgrades.CheckIfUpgradeMaxed(Upgrade.BananaGold));
        GameObject.Find(menusPath + "upgrades/upgrade (0)/upgradeButton/upgradeCost/quantity").GetComponent<TextMeshProUGUI>().text = GameEntities.Upgrades.GetUpgradeCost(Upgrade.BananaGold).ToString();
        GameObject.Find(menusPath + "upgrades/upgrade (0)/upgradeResult/quantity").GetComponent<TextMeshProUGUI>().text = "+" + GameEntities.Upgrades.GetUpgradeEffect(Upgrade.BananaGold);

        GameObject.Find(menusPath + "upgrades/upgrade (1)/upgradeButton").GetComponent<Button>().onClick.AddListener(() => {
            GameEntities.Upgrades.PerformUpgrade(Upgrade.BananaSpawnTime);
        });
        GameObject.Find(menusPath + "upgrades/upgrade (1)/upgradeButton").GetComponent<Button>().interactable = GameEntities.GoldController.CurrentGold >= GameEntities.Upgrades.GetUpgradeCost(Upgrade.BananaSpawnTime);
        GameObject.Find(menusPath + "upgrades/upgrade (1)/upgradeButton").SetActive(!GameEntities.Upgrades.CheckIfUpgradeMaxed(Upgrade.BananaSpawnTime));
        GameObject.Find(menusPath + "upgrades/upgrade (1)/upgradeButton/upgradeCost/quantity").GetComponent<TextMeshProUGUI>().text = GameEntities.Upgrades.GetUpgradeCost(Upgrade.BananaSpawnTime).ToString();
        GameObject.Find(menusPath + "upgrades/upgrade (1)/upgradeResult/quantity").GetComponent<TextMeshProUGUI>().text = "-" + GameEntities.Upgrades.GetUpgradeEffect(Upgrade.BananaSpawnTime);
        bananaRushButton = GameObject.Find(canvasPath + "bananaRush/button").GetComponent<Button>();
        bananaRushCooldownImage = GameObject.Find(canvasPath + "bananaRush/button/cooldown").GetComponent<Image>();
        bananaRushButton.onClick.AddListener(() => {
            StartCoroutine(StartBananaRush());
            StartCoroutine(StartBananaRushCooldown());
        });
        
        bananaPrefab = transform.Find("banana").gameObject;

        GameEntities.BananaController.SetSpawnTime();

        gameInitialised = true;
    }

    public void SpawnBanana() {
        GameObject newBanana = GameObject.Instantiate(bananaPrefab, new Vector2(UnityEngine.Random.Range(bananaSpawnEdges.x, bananaSpawnEdges.z), UnityEngine.Random.Range(bananaSpawnEdges.y, bananaSpawnEdges.w)), Quaternion.identity, null);
        newBanana.SetActive(true);
    }

    public IEnumerator InitializeMenu(MenuName MenuName, Action<GameObject> Initialize, Action Open, Action OnInitializeComplete = null) {

        UIData menu = allUI.SingleOrDefault(x => x.menu == MenuName);

        if (menu != null) {

            if (!menu.hasInitialized) {
                var newMenu = Instantiate(menu.prefab, GameObject.Find(menusPath).transform);
                Initialize?.Invoke(newMenu);

                yield return null;

                menu.hasInitialized = true;

                OnInitializeComplete?.Invoke();
            }

            Open?.Invoke();
        }
    }

    public bool CheckMenuInitialized(MenuName MenuName) {
        
        UIData menu = allUI.SingleOrDefault(x => x.menu == MenuName);

        if (menu != null) {
            return menu.hasInitialized;
        }

        Debug.LogError("This Menu Does Not Exist");
        
        return false;
    }
        
    private void UpdateRushBananas()
    {
        // move down and rotate bananas
        foreach (var banana in _rushBananas)
        {
            if(banana == null) continue;
            banana.position += Vector3.down * Time.deltaTime * 5;
            banana.Rotate(Vector3.forward, 5);
        }
    }

    private IEnumerator StartBananaRush() {
        var timeLeft = bananaRushSpawnDuration;
        while (timeLeft > 0) {
            bananaRushCooldownImage.fillAmount = timeLeft / bananaRushCooldown;
            var randomInterval = UnityEngine.Random.Range(0.1f, 0.2f);
            timeLeft -= randomInterval;
            yield return new WaitForSeconds(randomInterval);
            var newBanana = Instantiate(bananaPrefab,
                new Vector2(UnityEngine.Random.Range(bananaSpawnEdges.x, bananaSpawnEdges.z), bananaSpawnEdges.w),
                Quaternion.identity, null);
            newBanana.SetActive(true);
            _rushBananas.Add(newBanana.transform);
        }
    }

    private IEnumerator StartBananaRushCooldown() {
        bananaRushButton.interactable = false;
        bananaRushCooldownImage.fillAmount = 1;
        var timeLeft = bananaRushCooldown;
        while (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            bananaRushCooldownImage.fillAmount = timeLeft / bananaRushCooldown;
            yield return null;
        }
        bananaRushButton.interactable = true;
        _rushBananas.ForEach(x=>
        {
            if (x != null)
                Destroy(x.gameObject);
        });
        _rushBananas.Clear();
    }
}

public enum MenuName {
    InfoPopup = 0,
    AchievementsPopup = 1,
}