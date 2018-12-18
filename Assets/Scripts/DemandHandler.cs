using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemandHandler : MonoBehaviour
{
    private TileInformation tileInfo;
    private GameTime gameTime;

    private int hours = 0;

    private string tileType = "";

    float interpolationSpeed = 0;

    private int changedPopulation = 0;

    private float baseMultiplier = 0.0f;
    private float currentMultiplier = 0.0f;

    private int calculateHours(int currentTime)
    {
        return currentTime / 3600;
    }

    private void modifyCityDemand()
    {
        if (hours >= 8 && hours <= 12)
        {
            currentMultiplier += interpolationSpeed;
            changedPopulation += 2;
        }
        else if (hours >= 14 && hours <= 18)
        {
            currentMultiplier -= interpolationSpeed;
            changedPopulation -= 2;
        }

        tileInfo.setEnergyMultiplier(currentMultiplier);
        tileInfo.setChangedPopulation(changedPopulation);

        //reset the energy multiplier and population
        if (hours >= 18)
        {
            tileInfo.setEnergyMultiplier(baseMultiplier);
            tileInfo.setChangedPopulation(0);
        }
    }

    private void modifySuburbDemand()
    {
        if (hours >= 8 && hours <= 12)
        {
            currentMultiplier -= interpolationSpeed;
            changedPopulation += 2;
        }
        else if (hours >= 14 && hours <= 18)
        {
            currentMultiplier += interpolationSpeed;
            changedPopulation -= 2;
        }

        tileInfo.setEnergyMultiplier(currentMultiplier);
        tileInfo.setChangedPopulation(changedPopulation);

        //reset the energy multiplier and population
        if (hours >= 18)
        {
            tileInfo.setEnergyMultiplier(baseMultiplier);
            tileInfo.setChangedPopulation(0);
        }
    }

    private void modifyIndustrialDemand()
    {
        if (hours % 2 == 0)
        {
            currentMultiplier += interpolationSpeed;
            changedPopulation += 1;
        }
        else if (hours % 2 == 1)
        {
            currentMultiplier -= interpolationSpeed;
            changedPopulation -= 1;
        }

        tileInfo.setEnergyMultiplier(currentMultiplier);
        tileInfo.setChangedPopulation(changedPopulation);

        //reset the energy multiplier and population
        //if (hours == 4 || hours == 10 || hours == 16 || hours == 22)
        //{
        //    tileInfo.setEnergyMultiplier(baseMultiplier);
        //    tileInfo.setPopulation(population);
        //}
    }

    private void modifyFarmDemand()
    {
        if (hours >= 3 && hours <= 5)
        {
            currentMultiplier -= interpolationSpeed;
        }
        else if (hours >= 6 && hours <= 8)
        {
            currentMultiplier += interpolationSpeed;
        }

        tileInfo.setEnergyMultiplier(currentMultiplier);

        //reset the energy multiplier and population
        if (hours >= 8)
        {
            tileInfo.setEnergyMultiplier(baseMultiplier);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tileInfo = transform.gameObject.GetComponent<TileInformation>();
        gameTime = GameObject.Find("EventManager").GetComponent<GameTime>();

        tileType = tileInfo.getLandType();

        //setup the starting values
        baseMultiplier = tileInfo.getEnergyMultiplier();
        currentMultiplier = baseMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        hours = calculateHours(gameTime.getTime());

        changedPopulation = tileInfo.getChangedPopulation();

        switch (tileType)
        {
            case "City":
                interpolationSpeed = (1 / (((12 * 3600) - (8 * 3600)) / gameTime.getInGameSecsPerRealSecs()));
                modifyCityDemand();
                break;
            case "Suburb":
                interpolationSpeed = (1 / (((12 * 3600) - (8 * 3600)) / gameTime.getInGameSecsPerRealSecs()));
                modifySuburbDemand();
                break;
            case "Industrial":
                interpolationSpeed = (1 / (((3 * 3600) - (2 * 3600)) / gameTime.getInGameSecsPerRealSecs()));
                modifyIndustrialDemand();
                break;
            case "Farm Land":
                interpolationSpeed = (1 / (((5 * 3600) - (3 * 3600)) / gameTime.getInGameSecsPerRealSecs()));
                modifyFarmDemand();
                break;
            case "Forest":
                break;
            case "Grassland":
                break;
        }
        tileInfo.calculateEnergyDemand();
    }
}