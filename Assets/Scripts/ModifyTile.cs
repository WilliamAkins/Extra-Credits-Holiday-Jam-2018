using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyTile : MonoBehaviour
{
    private TileInformation tileInfo;
    private CheckTiles checkTiles;
    private ManageMainInfo manageMainInfo;

    private string tileType = "";

    private int basePopulation = 0;

    private int baseSpeedModifier = 1;
    private int speedModifier = 1;

    private bool grassTransitionOccuring = false;
    private bool forestTransitionOccuring = false;
    private bool farmTransitionOccuring = false;
    private bool suburbTransitionOccuring = false;

    private void changeGrasslandTile()
    {
        if (grassTransitionOccuring == false && basePopulation >= 1000)
        {
            grassTransitionOccuring = true;
            checkTiles.changeTile("Farm Land", true, gameObject);
        }
    }

    private void changeforestTile()
    {
        if (forestTransitionOccuring == false && basePopulation >= 1000)
        {
            //first check whether all the grassland is gone
            if (manageMainInfo.getIsThereGrassland() == false)
            {
                forestTransitionOccuring = true;
                checkTiles.changeTile("Farm Land", true, gameObject);
            }

        }
    }

    private void changeFarmTile()
    {
        if (farmTransitionOccuring == false && basePopulation >= 5000)
        {
            farmTransitionOccuring = true;

            int outcome = Random.Range(0, 100);
            string newTile = "";
            if (outcome < 70) //70% to be a suburb
                newTile = "Suburb";
            else
                newTile = "Industrial";

            checkTiles.changeTile(newTile, true, gameObject);
        }
    }

    private void changeSuburbTile()
    {
        if (suburbTransitionOccuring == false && basePopulation >= 15000)
        {
            suburbTransitionOccuring = true;
            checkTiles.changeTile("City", true, gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tileInfo = transform.gameObject.GetComponent<TileInformation>();
        checkTiles = GameObject.Find("EventManager").GetComponent<CheckTiles>();
        manageMainInfo = GameObject.Find("EventManager").GetComponent<ManageMainInfo>();

        tileType = tileInfo.getLandType();
        basePopulation = tileInfo.getBasePopulation();

        //randomly pick how much slower the population will increase from 1x - 5x
        baseSpeedModifier = Random.Range(1, 6);
        speedModifier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (manageMainInfo.getGamePaused() == false)
        {
            switch (tileType)
            {
                case "City":
                    if (speedModifier == baseSpeedModifier)
                    {
                        if (manageMainInfo.getTotalProduction() >= manageMainInfo.getTotalDemand())
                        {
                            basePopulation++;
                        }
                        speedModifier = 0;
                    }
                    speedModifier++;
                    break;
                case "Suburb":
                    if (!tileInfo.getPlayerOwns())
                    {
                        if (speedModifier == baseSpeedModifier)
                        {
                            if (manageMainInfo.getTotalProduction() >= manageMainInfo.getTotalDemand())
                            {
                                basePopulation++;
                                changeSuburbTile();
                            }
                            speedModifier = 0;
                        }
                        speedModifier++;
                    }
                    break;
                case "Industrial":
                    if (speedModifier == baseSpeedModifier)
                    {
                        if (manageMainInfo.getTotalProduction() >= manageMainInfo.getTotalDemand())
                        {
                            basePopulation++;
                        }
                        speedModifier = 0;
                    }
                    speedModifier++;
                    break;
                case "Farm Land":
                    if (!tileInfo.getPlayerOwns())
                    {
                        if (speedModifier == baseSpeedModifier)
                        {
                            if (manageMainInfo.getTotalProduction() >= manageMainInfo.getTotalDemand())
                            {
                                basePopulation++;
                                changeFarmTile();
                            }

                            speedModifier = 0;
                        }
                        speedModifier++;
                    }
                    break;
                case "Grassland":
                    if (!tileInfo.getPlayerOwns())
                    {
                        if (speedModifier == baseSpeedModifier)
                        {
                            if (manageMainInfo.getTotalProduction() >= manageMainInfo.getTotalDemand())
                            {
                                basePopulation++;
                                changeGrasslandTile();
                            }
                            speedModifier = 0;
                        }
                        speedModifier++;
                    }
                    break;
                case "Forest":
                    if (!tileInfo.getPlayerOwns())
                    {
                        if (speedModifier == baseSpeedModifier)
                        {
                            if (manageMainInfo.getIsThereGrassland() == false)
                            {
                                if (manageMainInfo.getTotalProduction() >= manageMainInfo.getTotalDemand())
                                {
                                    basePopulation++;
                                    changeforestTile();
                                }
                                speedModifier = 0;
                            }
                        }
                        speedModifier++;
                    }
                    break;
            }
            tileInfo.setBasePopulation(basePopulation);
        }
    }
}
