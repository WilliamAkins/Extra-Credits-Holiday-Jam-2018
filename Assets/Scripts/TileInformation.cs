using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInformation : MonoBehaviour
{
    private CheckTiles checkTiles;
    private MessageHandler messageHandler;

    private string landType = "";
    private bool playerOwns = false;

    private int tileID = 0;

    //START: base values for different production tile types
    private const int solarBaseProduction = 25000;
    private const int solarBaseUpKeep = 6000;
    private const int solarBaseBuyCost = 12000;

    private const int windBaseProduction = 30000;
    private const int windBaseUpKeep = 8000;
    private const int windBaseBuyCost = 20000;

    private const int coalBaseProduction = 80000;
    private const int coalBaseUpKeep = 20000;
    private const int coalBaseBuyCost = 50000;

    private const int nuclearBaseProduction = 250000;
    private const int nuclearBaseUpKeep = 30000;
    private const int nuclearBaseBuyCost = 100000;

    private const int grasslandBaseProduction = 0;
    private const int grasslandBaseUpKeep = 4000;
    private const int grasslandBaseBuyCost = 0;
    //END: base values for different production tile types

    //production tile values
    private int energyProduction = 0;
    private int upkeepCost = 0;
    private float durability = 100.0f;
    private int baseRepairCost = 0;
    //private int repairCost = 0;

    //consumption tile values
    private int energyDemand = 0;
    private float energyMultiplier = 1.0f;
    private float perPersonEnergy = 0.0f;
    private int tileCost = 0;

    private int basePopulation = 0; //only changes as the tile expands / shrinks
    private int changedPopulation = 0; //the new population - base, can be negetive
    private int population = 0; //made up of base population + changed population

    public void calculateEnergyDemand()
    {
        population = basePopulation + changedPopulation;
        energyDemand = Mathf.RoundToInt((population * perPersonEnergy) * energyMultiplier);

        //add random fluxuation to the energy by adjusting it value by -0.1% to +0.1%
        energyDemand += Random.Range((energyDemand / 1000) * -1, (energyDemand / 1000));
    }

    private void playerOwnsTile()
    {
        switch (landType)
        {
            case "Solar Farm":
                energyProduction = solarBaseProduction;
                upkeepCost = solarBaseUpKeep;
                baseRepairCost = solarBaseBuyCost;
                break;
            case "Wind Farm":
                energyProduction = windBaseProduction;
                upkeepCost = windBaseUpKeep;
                baseRepairCost = windBaseBuyCost;
                break;
            case "Coal Plant":
                energyProduction = coalBaseProduction;
                upkeepCost = coalBaseUpKeep;
                baseRepairCost = coalBaseBuyCost;
                break;
            case "Nuclear Plant":
                energyProduction = nuclearBaseProduction;
                upkeepCost = nuclearBaseUpKeep;
                baseRepairCost = nuclearBaseBuyCost;
                break;
            case "Grassland":
                energyProduction = 0;
                upkeepCost = grasslandBaseUpKeep;
                baseRepairCost = 5000;
                break;
        }

        energyDemand = 0;
        basePopulation = 0;
        changedPopulation = 0;
        population = 0;
    }

    private void playerDoesntOwnTile()
    {
        switch (landType)
        {
            case "City":
                energyMultiplier = 2.0f;
                perPersonEnergy = 4.0f;
                tileCost = 300000;
                basePopulation = 0;
                break;
            case "Suburb":
                energyMultiplier = 1.4f;
                perPersonEnergy = 1.5f;
                tileCost = 120000;
                basePopulation = Random.Range(15000, 20000);
                break;
            case "Industrial":
                energyMultiplier = 1.6f;
                perPersonEnergy = 3.0f;
                tileCost = 220000;
                basePopulation = 0;
                break;
            case "Farm Land":
                energyMultiplier = 1.0f;
                perPersonEnergy = 1.2f;
                tileCost = 90000;
                basePopulation = 0;
                break;
            case "Forest":
                energyMultiplier = 0.5f;
                perPersonEnergy = 0.5f;
                tileCost = 50000;
                basePopulation = 0;
                break;
            case "Grassland":
                energyMultiplier = 0.5f;
                perPersonEnergy = 0.5f;
                tileCost = 30000;
                basePopulation = Random.Range(0, 50);
                break;
        }
        population = basePopulation + changedPopulation;
        calculateEnergyDemand();
    }

    // Start is called before the first frame update
    void Start()
    {
        //playerDoesntOwnTile();
        checkTiles = GameObject.Find("EventManager").GetComponent<CheckTiles>();
        messageHandler = GameObject.Find("EventManager").GetComponent<MessageHandler>();
    }

    // Update is called once per frame
    void Update() {}

    public int getBasePopulation()
    {
        return basePopulation;
    }

    public void setBasePopulation(int newBasePopulation)
    {
        basePopulation = newBasePopulation;
    }

    public void setChangedPopulation(int newChangedPopulation)
    {
        changedPopulation = newChangedPopulation;
    }

    public int getChangedPopulation()
    {
        return changedPopulation;
    }

    public int getPopulation()
    {
        population = basePopulation + changedPopulation;
        return population;
    }

    public void setPopulation(int newPopulation)
    {
        population = newPopulation;
    }

    public float getPerPersonEnergy()
    {
        return perPersonEnergy;
    }

    public void setPerPersonEnergy(float newPerPersonEnergy)
    {
        perPersonEnergy = newPerPersonEnergy;
    }

    public float getEnergyMultiplier()
    {
        return energyMultiplier;
    }

    public void setEnergyMultiplier(float newEnergyMultiplier)
    {
        energyMultiplier = newEnergyMultiplier;
    }

    public string getLandType()
    {
        return landType;
    }

    public void setLandType(string newLandType)
    {
        landType = newLandType;

        if (playerOwns)
            playerOwnsTile();
        else
            playerDoesntOwnTile();
    }

    public int getEnergyDemand()
    {
        return energyDemand;
    }

    public bool getPlayerOwns()
    {
        return playerOwns;
    }

    public void setPlayerOwns(bool newOwns)
    {
        playerOwns = newOwns;
    }

    public int getEnergyProduction()
    {
        return energyProduction;
    }

    public int getTileCost()
    {
        return tileCost;
    }

    public int getUpkeepCost()
    {
        return upkeepCost;
    }

    private void checkDurability()
    {
        if (durability <= 0.0f)
        {
            durability = 0.0f;
            //destroy the tile as it has no durability left
            checkTiles.changeTile("Grassland", true, gameObject);
            checkTiles.setTileColourNotOwned(tileID);
            playerOwns = false;

            messageHandler.createMessage(getLandType() + " has reached 0% durability, you loose the tile.", "bad");
        }
    }

    public void setDurability(float newDurability)
    {
        durability = newDurability;
        checkDurability();
    }

    public void changeDurability(float newDurability)
    {
        durability += newDurability;
        checkDurability();
    }

    public float getDurability()
    {
        return durability;
    }

    public int getRepairCost()
    {
        return (int)(baseRepairCost * ((100.0f - durability) / 100));
    }

    public void setTileID(int newTileID)
    {
        tileID = newTileID;
    }

    public int getTileID()
    {
        return tileID;
    }

    public int getBaseProduction(string energyType)
    {
        switch (energyType)
        {
            case "Solar Farm":
                return solarBaseProduction;
            case "Wind Farm":
                return windBaseProduction;
            case "Coal Plant":
                return coalBaseProduction;
            case "Nuclear Plant":
                return nuclearBaseProduction;
        }
        return -1;
    }

    public int getBaseUpkeep(string energyType)
    {
        switch (energyType)
        {
            case "Solar Farm":
                return solarBaseUpKeep;
            case "Wind Farm":
                return windBaseUpKeep;
            case "Coal Plant":
                return coalBaseUpKeep;
            case "Nuclear Plant":
                return nuclearBaseUpKeep;
        }
        return -1;
    }

    public int getBaseBuyCost(string energyType)
    {
        switch (energyType)
        {
            case "Solar Farm":
                return solarBaseBuyCost;
            case "Wind Farm":
                return windBaseBuyCost;
            case "Coal Plant":
                return coalBaseBuyCost;
            case "Nuclear Plant":
                return nuclearBaseBuyCost;
        }
        return -1;
    }
}
