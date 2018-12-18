using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ManageMainInfo : MonoBehaviour
{
    [Header("Set the starting money")]
    public int money = 100000;

    private bool gameHasEnded = false;

    private bool gamePaused = false;

    private int totalDemand = 0;
    private int totalProduction = 0;
    private int totalPopulation = 0;

    private long totalEnergyDeficit = 0;
    private long deficitIncrement = 0;

    //the total excess energy the player has gotten
    private long totalExcessEnergy = 0;

    private float hourTime, minuteTime, secondsTime;

    private int actualDay = 1;
    private int day = 1; //Not actual day but range from 1 - 7 for days of the week;

    //says whether there is grassland which the player does not own
    private bool isThereGrassland = false;

    private GenerateTiles generateTiles;
    private MessageHandler messageHandler;

    private GameObject[] tiles;
    private TileInformation[] tileInfo;

    private GameTime gameTime;

    private TextMeshProUGUI txtTotalDemand;
    private TextMeshProUGUI txtTotalProduction;
    private TextMeshProUGUI txtMoney;
    private TextMeshProUGUI txtTime;
    private TextMeshProUGUI txtNews;

    private GameObject endScreen;
    private Button btnMenu;

    private bool tileArrayHasChanged = false;

    //START: Public functions
    public bool getGamePaused()
    {
        return gamePaused;
    }

    public void setGamePaused(bool newGamePaused)
    {
        gamePaused = newGamePaused;
    }

    public bool getIsThereGrassland()
    {
        return isThereGrassland;
    }

    public int getTotalDemand()
    {
        return totalDemand;
    }

    public int getTotalProduction()
    {
        return totalProduction;
    }

    public int getMoney()
    {
        return money;
    }

    public void decrementMoney(int newMoney)
    {
        money -= newMoney;
    }

    public void incrementMoney(int newMoney)
    {
        money += newMoney;
    }

    public bool getTileArrayHasChanged()
    {
        return tileArrayHasChanged;
    }

    public void setTileArrayHasChanged(bool newTileArrayHasChanged)
    {
        tileArrayHasChanged = newTileArrayHasChanged;
    }

    public string getFileNames(string friendlyName)
    {
        switch (friendlyName)
        {
            case "Solar Farm":
                return "Hexagon_SolarFarm";
            case "Wind Farm":
                return "Hexagon_WindFarm";
            case "Coal Plant":
                return "Hexagon_CoalPowerPlant";
            case "Nuclear Plant":
                return "Hexagon_Nuclear";
            case "City":
                return "Hexagon_City";
            case "Suburb":
                return "Hexagon_Suburb";
            case "Industrial":
                return "hexagon_Industrial";
            case "Farm Land":
                return "Hexagon_Farm";
            case "Forest":
                return "Hexagon_Forest";
            case "Grassland":
                return "Hexagon_Grassland";
            default:
                return "Hexagon_Grassland";
        }
    }
    //END: Public functions

    private string formatMins(int currentTime)
    {
        string newTime;
        int mins;
        currentTime %= 3600;
        mins = currentTime / 60;

        string minuteUpdate;

        if (mins < 10)
            minuteUpdate = "0" + mins.ToString();
        else
            minuteUpdate = mins.ToString();
        newTime = minuteUpdate;

        return newTime;
    }

    //allows the time to be formatted correctly
    private string formatHours(int currentTime)
    {
        string newTime;
        int hours;
        hours = currentTime / 3600;
        currentTime %= 3600;

        string hourUpdate;
        hourUpdate = hours.ToString();

        newTime = hourUpdate + ":";

        return newTime;
    }

    private string getDayName(int dayNum)
    {
        switch (dayNum) {
            case 1:
                return "Monday";
            case 2:
                return "Tuesday";
            case 3:
                return "Wednesday";
            case 4:
                return "Thursday";
            case 5:
                return "Friday";
            case 6:
                return "Saturday";
            case 7:
                return "Sunday";
            default:
                return "";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameTime = GameObject.Find("EventManager").GetComponent<GameTime>();
        generateTiles = GameObject.Find("EventManager").GetComponent<GenerateTiles>();
        messageHandler = GameObject.Find("EventManager").GetComponent<MessageHandler>();

        tiles = generateTiles.getTiles();
        tileInfo = new TileInformation[tiles.Length];
        for (int i = 0; i < tileInfo.Length; i++)
            tileInfo[i] = tiles[i].transform.GetComponent<TileInformation>();

        //get references to all the text boxes we want to update
        GameObject topPanel = GameObject.Find("MainInformation/TopPanel");
        txtTotalDemand = topPanel.transform.Find("totalDemand").GetComponent<TextMeshProUGUI>();
        txtTotalProduction = topPanel.transform.Find("totalProduction").GetComponent<TextMeshProUGUI>();
        txtMoney = topPanel.transform.Find("money").GetComponent<TextMeshProUGUI>();
        txtTime = topPanel.transform.Find("time").GetComponent<TextMeshProUGUI>();
        txtNews = topPanel.transform.Find("NewsSlider/txtNews").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePaused)
        {
            //reset the values each frame
            totalDemand = 0;
            totalProduction = 0;
            totalPopulation = 0;

            //update the tileinfo array
            for (int i = 0; i < tileInfo.Length; i++)
            {
                if (tiles[i] != null)
                    tileInfo[i] = tiles[i].transform.GetComponent<TileInformation>();
            }

            //add up the data from all tiles for demand and production
            isThereGrassland = false;
            int ownedTiles = 0;
            for (int i = 0; i < tileInfo.Length; i++)
            {
                totalDemand += tileInfo[i].getEnergyDemand();
                totalProduction += tileInfo[i].getEnergyProduction();
                totalPopulation += tileInfo[i].getBasePopulation();

                //modify the durability for each tile that the player owns
                if (tileInfo[i].getPlayerOwns() == true)
                {
                    ownedTiles++;
                    tileInfo[i].changeDurability(-0.01f);
                }

                if (tileInfo[i].getPlayerOwns() == false)
                    if (tileInfo[i].getLandType() == "Grassland")
                        isThereGrassland = true;
            }

            //stores the total deficit for a day
            totalEnergyDeficit += (totalDemand - totalProduction);
            deficitIncrement++;

            //people pay the player every frame
            money += Mathf.RoundToInt(totalPopulation * 0.001f);

            //keep track of the total excess energy the player has gotten for the end game screen
            if (totalProduction > totalDemand)
                totalExcessEnergy += totalProduction - totalDemand;

            if (gameTime.getDay() > actualDay)
            {
                actualDay = gameTime.getDay();

                //give the player their daily money
                money += Mathf.RoundToInt(totalPopulation * 0.02f);

                //fine the player if they are not meeting the energy demand
                int fine = 0;
                if (totalEnergyDeficit > 0)
                    fine = (int)(totalEnergyDeficit / deficitIncrement);

                if (fine > 0)
                {
                    money -= fine;

                    messageHandler.createMessage("You have been fined £" + fine.ToString("n0") + " for not meeting the enrgy demand.", "bad");
                }

                if (money < 0)
                    money = 0;

                totalEnergyDeficit = 0;
                deficitIncrement = 0;


                //charge the player the upkeep cost of each of their tiles
                for (int i = 0; i < tiles.Length; i++)
                {
                    if (tileInfo[i].getPlayerOwns() == true)
                    {
                        if (money >= tileInfo[i].getUpkeepCost())
                        {
                            money -= tileInfo[i].getUpkeepCost();

                            if (money < 0)
                                money = 0;
                        }
                        else
                        {
                            messageHandler.createMessage("You can't afford to pay the upkeep costs for your " + tileInfo[i].getLandType() + ", Its durability gets lowered.", "bad");
                            tileInfo[i].changeDurability(-50.0f);
                        }
                    }
                }

                //if day has reached the end of the week reset the variable
                if (day < 7)
                    day++;
                else
                    day = 1;

                //win condition, the player has survived 14 days
                if (actualDay >= 14)
                {
                    if (!gameHasEnded)
                    {
                        gamePaused = true;
                        gameHasEnded = true;
                        endScreen = Instantiate(Resources.Load("WinGameConditions")) as GameObject;
                        Debug.Log("The player has survived 14 days. Game will now end.");
                        btnMenu = endScreen.transform.Find("BackButton").gameObject.GetComponent<Button>();
                        btnMenu.onClick.AddListener(Menu);
                    }
                }
            }

            //update the output text
            txtTotalDemand.text = totalDemand.ToString("n0") + "J";

            if (totalProduction >= totalDemand)
            {
                txtTotalProduction.color = new Color(0.1490196f, 0.8509804f, 0.1490196f); //Green
            }
            else
            {
                txtTotalProduction.color = new Color(0.8509804f, 0.1490196f, 0.1490196f); //Red
            }

            txtTotalProduction.text = totalProduction.ToString("n0") + "J";

            txtMoney.text = "£" + money.ToString("n0");
            txtTime.text = formatHours(gameTime.getTime()) + formatMins(gameTime.getTime()) + " " + getDayName(day);

            //loose condition, should the player run out of tiles
            if (ownedTiles == 0)
            {
                if (!gameHasEnded)
                {
                    gamePaused = true;
                    gameHasEnded = true;
                    endScreen = Instantiate(Resources.Load("LoseGameConditions")) as GameObject;
                    Debug.Log("The player has lost all tiles. Game will now end.");
                    btnMenu = endScreen.transform.Find("BackButton").gameObject.GetComponent<Button>();
                    btnMenu.onClick.AddListener(Menu);

                }
            }
        }
    }

    void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
