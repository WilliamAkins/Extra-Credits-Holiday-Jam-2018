using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CheckTiles : MonoBehaviour
{
    private ManageMainInfo manageMainInfo;

    private GameObject[] tiles; //holds all tiles

    private GameObject touchHistory;
    private GameObject stats;
    private GameObject ownedTileInfo;

    private Button btnRepair;

    //store the buy button instances
    private Button btnBuy;
    private Button btnBuySolar;
    private Button btnBuyWind;
    private Button btnBuyCoal;
    private Button btnBuyNuclear;

    //stores the currently clicked on tile
    private GameObject selectedTile;

    private void removeListeners()
    {
        if (btnBuy != null)
        {
            btnBuy.onClick.RemoveAllListeners();
            btnBuy = null;
        }

        if (btnRepair != null)
        {
            btnRepair.onClick.RemoveAllListeners();
            btnRepair = null;
        }


        if (btnBuySolar != null)
        {
            btnBuySolar.onClick.RemoveAllListeners();
            btnBuySolar = null;

            btnBuyWind.onClick.RemoveAllListeners();
            btnBuyWind = null;

            btnBuyCoal.onClick.RemoveAllListeners();
            btnBuyCoal = null;

            btnBuyNuclear.onClick.RemoveAllListeners();
            btnBuyNuclear = null;
        }
    }

    private void destroyStatsMenu()
    {
        //remove the stats popup if it exists
        if (stats != null)
        {
            Destroy(stats);
            stats = null;
        }
        removeListeners();
    }

    private void destroyOwnedMenu()
    {
        //remove the stats popup if it exists
        if (ownedTileInfo != null)
        {
            Destroy(ownedTileInfo);
            ownedTileInfo = null;
        }
        removeListeners();
    }

    // Start is called before the first frame update
    private void Start()
    {
        tiles = GameObject.Find("EventManager").GetComponent<GenerateTiles>().getTiles();
        manageMainInfo = GameObject.Find("EventManager").GetComponent<ManageMainInfo>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Tile" && !EventSystem.current.IsPointerOverGameObject())
            {
                GameObject highlight = hit.transform.Find("TileHighlight").gameObject;

                highlight.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("highlight_full");

                selectedTile = hit.transform.gameObject;

                if (hit.transform.GetComponent<TileInformation>().getPlayerOwns() == false)
                {
                    //remove the owned tile info popup if it exists
                    if (ownedTileInfo != null)
                    {
                        Destroy(ownedTileInfo);
                        ownedTileInfo = null;
                    }

                    //load the prefab with the information about this tile
                    if (stats == null)
                        stats = Instantiate(Resources.Load("LandStats")) as GameObject;

                    TileInformation tileInfo = hit.transform.GetComponent<TileInformation>();

                    //if the btnBuy instance still exists then remove it and its listener
                    if (btnBuy != null)
                    {
                        btnBuy.onClick.RemoveAllListeners();
                        btnBuy = null;
                    }

                    if (btnBuySolar != null)
                    {
                        btnBuySolar.onClick.RemoveAllListeners();
                        btnBuySolar = null;

                        btnBuyWind.onClick.RemoveAllListeners();
                        btnBuyWind = null;

                        btnBuyCoal.onClick.RemoveAllListeners();
                        btnBuyCoal = null;

                        btnBuyNuclear.onClick.RemoveAllListeners();
                        btnBuyNuclear = null;
                    }

                    btnBuy = stats.transform.Find("Panel/btnBuyLand").gameObject.GetComponent<Button>();
                    int landCost = tileInfo.getTileCost();

                    //check whether the player has enough money to buy the land and if they do allow the purchase
                    if (GameObject.Find("EventManager").GetComponent<ManageMainInfo>().getMoney() >= landCost)
                    {
                        btnBuy.interactable = true;
                        btnBuy.onClick.AddListener(delegate { btnBuyPressed("Grassland", landCost); });
                    }
                    else
                    {
                        btnBuy.interactable = false;
                    }
                    btnBuy.transform.Find("buyLand").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + landCost.ToString("n0");
                }
                else
                {
                    //remove the menu system
                    destroyStatsMenu();

                    //load the prefab with the information about this tile
                    if (ownedTileInfo == null)
                        ownedTileInfo = Instantiate(Resources.Load("OwnedTileInfo")) as GameObject;

                    updateOwnedPanelInfo(selectedTile);
                }

                if (touchHistory != null && touchHistory != highlight)
                    touchHistory.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("highlight");

                touchHistory = highlight;
            }
        }

        //if the stats panel is up, update the information in it each frame
        if (stats != null && selectedTile != null)
            updateNotOwnedPanelInfo(selectedTile);

        if (ownedTileInfo != null && selectedTile != null)
            updateOwnedPanelInfoEachFrame(selectedTile);
    }

    private void updateOwnedPanelInfoEachFrame(GameObject tile)
    {
        TileInformation tileInfo = tile.transform.GetComponent<TileInformation>();
        ownedTileInfo.transform.Find("InfoPanel/btnRepair/repair").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + tileInfo.getRepairCost().ToString("n0");
        ownedTileInfo.transform.Find("InfoPanel/durability").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getDurability().ToString("n0") + "%";
    }

    private void updateNotOwnedPanelInfo(GameObject tile)
    {
        TileInformation tileInfo = tile.transform.GetComponent<TileInformation>();
        stats.transform.Find("Panel/topPanel/txtLandType").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getLandType();
        stats.transform.Find("Panel/energyDemand").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getEnergyDemand().ToString("n0") + "J";
        stats.transform.Find("Panel/population").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getPopulation().ToString("n0");
    }

    private void updateOwnedPanelInfo(GameObject tile)
    {
        int playerMoney = GameObject.Find("EventManager").GetComponent<ManageMainInfo>().getMoney();

        TileInformation tileInfo = tile.transform.GetComponent<TileInformation>();
        ownedTileInfo.transform.Find("InfoPanel/HeadingPanel/txtHeading").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getLandType();
        ownedTileInfo.transform.Find("InfoPanel/production").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getEnergyProduction().ToString("n0") + "J";
        ownedTileInfo.transform.Find("InfoPanel/upkeep").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + tileInfo.getUpkeepCost().ToString("n0");
        ownedTileInfo.transform.Find("InfoPanel/durability").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getDurability().ToString("n0") + "%";
        ownedTileInfo.transform.Find("InfoPanel/btnRepair/repair").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + tileInfo.getRepairCost().ToString("n0");

        //if (playerMoney >= nuclearBaseCost)
        //{
        btnRepair = ownedTileInfo.transform.Find("InfoPanel/btnRepair").GetComponent<Button>();
        btnRepair.interactable = true;
        btnRepair.onClick.AddListener(delegate { btnRepairPressed(tileInfo.getRepairCost()); });
        //}

        //fuelSlider = fuelObj.transform.Find("FuelSlider").GetComponent<Slider>();

        ownedTileInfo.transform.Find("Panel/solarFarm/production").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getBaseProduction("Solar Farm").ToString("n0") + "J";
        ownedTileInfo.transform.Find("Panel/solarFarm/upkeep").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + tileInfo.getBaseUpkeep("Solar Farm").ToString("n0");

        int solarBaseCost = tileInfo.getBaseBuyCost("Solar Farm");
        btnBuySolar = ownedTileInfo.transform.Find("Panel/solarFarm/btnBuyNow").gameObject.GetComponent<Button>();
        btnBuySolar.transform.Find("buyNow").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + solarBaseCost.ToString("n0");

        if (playerMoney >= solarBaseCost)
        {
            btnBuySolar.interactable = true;
            btnBuySolar.onClick.AddListener(delegate { btnBuyPressed("Solar Farm", solarBaseCost); });
        }
        else
        {
            btnBuySolar.interactable = false;
        }

        ownedTileInfo.transform.Find("Panel/windFarm/production").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getBaseProduction("Wind Farm").ToString("n0") + "J";
        ownedTileInfo.transform.Find("Panel/windFarm/upkeep").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + tileInfo.getBaseUpkeep("Wind Farm").ToString("n0");

        int windBaseCost = tileInfo.getBaseBuyCost("Wind Farm");
        btnBuyWind = ownedTileInfo.transform.Find("Panel/windFarm/btnBuyNow").gameObject.GetComponent<Button>();
        btnBuyWind.transform.Find("buyNow").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + windBaseCost.ToString("n0");

        if (playerMoney >= solarBaseCost)
        {
            btnBuyWind.interactable = true;
            btnBuyWind.onClick.AddListener(delegate { btnBuyPressed("Wind Farm", windBaseCost); });
        }
        else
        {
            btnBuyWind.interactable = false;
        }

        ownedTileInfo.transform.Find("Panel/coalPlant/production").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getBaseProduction("Coal Plant").ToString("n0") + "J";
        ownedTileInfo.transform.Find("Panel/coalPlant/upkeep").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + tileInfo.getBaseUpkeep("Coal Plant").ToString("n0");

        int coalBaseCost = tileInfo.getBaseBuyCost("Coal Plant");
        btnBuyCoal = ownedTileInfo.transform.Find("Panel/coalPlant/btnBuyNow").gameObject.GetComponent<Button>();
        btnBuyCoal.transform.Find("buyNow").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + coalBaseCost.ToString("n0");

        if (playerMoney >= coalBaseCost)
        {
            btnBuyCoal.interactable = true;
            btnBuyCoal.onClick.AddListener(delegate { btnBuyPressed("Coal Plant", coalBaseCost); });
        }
        else
        {
            btnBuyCoal.interactable = false;
        }

        ownedTileInfo.transform.Find("Panel/nuclearPlant/production").gameObject.GetComponent<TextMeshProUGUI>().text = tileInfo.getBaseProduction("Nuclear Plant").ToString("n0") + "J";
        ownedTileInfo.transform.Find("Panel/nuclearPlant/upkeep").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + tileInfo.getBaseUpkeep("Nuclear Plant").ToString("n0");

        int nuclearBaseCost = tileInfo.getBaseBuyCost("Nuclear Plant");
        btnBuyNuclear = ownedTileInfo.transform.Find("Panel/nuclearPlant/btnBuyNow").gameObject.GetComponent<Button>();
        btnBuyNuclear.transform.Find("buyNow").gameObject.GetComponent<TextMeshProUGUI>().text = "£" + nuclearBaseCost.ToString("n0");

        if (playerMoney >= nuclearBaseCost)
        {
            btnBuyNuclear.interactable = true;
            btnBuyNuclear.onClick.AddListener(delegate { btnBuyPressed("Nuclear Plant", nuclearBaseCost); });
        }
        else
        {
            btnBuyNuclear.interactable = false;
        }
    }

    private void btnRepairPressed(int repairCost)
    {
        Debug.Log("Repair button was pressed");

        int playerMoney = GameObject.Find("EventManager").GetComponent<ManageMainInfo>().getMoney();

        //lower the player money by the repair cost
        if (playerMoney >= repairCost)
        {
            manageMainInfo.decrementMoney(repairCost);

            selectedTile.GetComponent<TileInformation>().setDurability(100);
        }
        else
        {
            Debug.Log("Oh noes, the player is too poor!");
        }
    }

    private void btnBuyPressed(string tileType, int cost)
    {
        Debug.Log("Tile is being changed.");

        //lower the players money
        manageMainInfo.decrementMoney(cost);

        //change the tile to a new one
        int tileIndex = changeTile(tileType, false);

        //remove the stats popup if it exists
        destroyStatsMenu();

        //load the prefab with the information about this tile
        if (ownedTileInfo == null)
            ownedTileInfo = Instantiate(Resources.Load("OwnedTileInfo")) as GameObject;

        setTileColourOwned(tileIndex);

        GameObject highlight = tiles[tileIndex].transform.Find("TileHighlight").gameObject;
        SpriteRenderer tileSpriteRenderer = highlight.GetComponent<SpriteRenderer>();
        tileSpriteRenderer.sprite = Resources.Load<Sprite>("highlight_full");

        //update the text output with the new data
        updateOwnedPanelInfo(tiles[tileIndex]);
    }

    public void setTileColourOwned(int index)
    {
        GameObject highlight = tiles[index].transform.Find("TileHighlight").gameObject;
        SpriteRenderer tileSpriteRenderer = highlight.GetComponent<SpriteRenderer>();
        tileSpriteRenderer.color = new Color(178.0f / 255, 14.0f / 255, 14.0f / 255); //red
        tileSpriteRenderer.sprite = Resources.Load<Sprite>("highlight");
        touchHistory = highlight;
    }

    public void setTileColourNotOwned(int index)
    {
        GameObject highlight = tiles[index].transform.Find("TileHighlight").gameObject;
        SpriteRenderer tileSpriteRenderer = highlight.GetComponent<SpriteRenderer>();
        tileSpriteRenderer.color = new Color(93.0f / 255, 147.0f / 255, 91.0f / 255); //green
        tileSpriteRenderer.sprite = Resources.Load<Sprite>("highlight");
        touchHistory = highlight;
    }

    public int changeTile(string tileType, bool externalChange, GameObject oldTile = null)
    {
        //if this tile change occured not from the player changing it
        Transform tmpTilePos;
        if (externalChange)
        {
            //store the current transform
            tmpTilePos = oldTile.transform;
        }
        else
        {
            //store the current transform
            tmpTilePos = selectedTile.transform;
        }

        //find the index of the current tile so that the new one can be added to the array
        //int tileIndex = 0;
        //for (int i = 9; i < tiles.Length; i++)
        //{
        //    if (tiles[i].transform == tmpTilePos)
        //        tileIndex = i;
        //}
        int tileIndex = tmpTilePos.gameObject.GetComponent<TileInformation>().getTileID();

        tiles[tileIndex] = Instantiate(Resources.Load(manageMainInfo.getFileNames(tileType)), tmpTilePos.position, Quaternion.identity) as GameObject;
        tiles[tileIndex].transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        tiles[tileIndex].transform.eulerAngles = tmpTilePos.eulerAngles;

        //begin the transition to change the tile
        StartCoroutine(reduceTileSize(tileIndex, externalChange, oldTile));
        StartCoroutine(increaseTileSize(tileIndex));

        //initialise the tile with the default information for a newly bought tile
        TileInformation tileInfo = tiles[tileIndex].transform.GetComponent<TileInformation>();

        if (!externalChange)
        {
            tileInfo.setPlayerOwns(true);
        }
        else
        {
            tileInfo.setPlayerOwns(false);
            if (selectedTile == tiles[tileIndex])
            {
                destroyStatsMenu();
                destroyOwnedMenu();
            }
        }

        tileInfo.setTileID(tileIndex);
        tileInfo.setLandType(tileType);

        return tileIndex;
    }

    private IEnumerator reduceTileSize(int tileIndex, bool externalChange, GameObject oldTile = null)
    {
        float time = 1.0f;
        float currentTime = 0.0f;

        GameObject tmpTile;

        if (externalChange)
            tmpTile = oldTile;
        else
            tmpTile = selectedTile;

        Vector3 originalScale = tmpTile.transform.localScale;
        Vector3 destinationScale = new Vector3(0.0f, 0.0f, 0.0f);

        do
        {
            tmpTile.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);

            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);

        if (selectedTile == tiles[tileIndex])
        {
            destroyStatsMenu();
            destroyOwnedMenu();
        }

        //destory the currently selected tile and replace it with the new one
        Destroy(tmpTile);

        if (!externalChange)
            selectedTile = tiles[tileIndex];
    }

    private IEnumerator increaseTileSize(int tileIndex)
    {
        float time = 1.0f;
        float currentTime = 0.0f;

        Vector3 originalScale = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 destinationScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (selectedTile == tiles[tileIndex])
        {
            destroyStatsMenu();
            destroyOwnedMenu();
        }

        do
        {
            tiles[tileIndex].transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);

            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }
}