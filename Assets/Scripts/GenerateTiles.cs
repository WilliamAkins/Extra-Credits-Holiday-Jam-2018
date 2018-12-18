using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTiles : MonoBehaviour
{
    public int tileWidth = 4;
    public int tileHeight = 4;

    private float xOffset = 0.0f;
    private float zOffset = 0.0f;

    private const float outerRadius = 6.1f;
    private const float innerRadius = outerRadius * 0.866025404f;

    private GameObject[] tile; //contains all the tiles
    private TileInformation tileInfo;

    private string resourceName = ""; //contains the name of the resource that needs to be loaded
    private string friendlyName = "";

    private int curTile = 0;

    private bool initalOwnedTileMade = false;

    //this is the building which will be spawned at the start of the game
    private void getStartingBuilding()
    {
        resourceName = "Hexagon_Suburb";
        friendlyName = "Suburb";
    }

    private void getStartingOwnedTileType()
    {
        resourceName = "Hexagon_Grassland";
        friendlyName = "Grassland";
    }

    private void getTileType()
    {
        switch (Random.Range(4, 6))
        {
            case 0:
                resourceName = "Hexagon_City";
                friendlyName = "City";
                break;
            case 1:
                resourceName = "Hexagon_Suburb";
                friendlyName = "Suburb";
                break;
            case 2:
                resourceName = "Hexagon_Industrial";
                friendlyName = "Industrial";
                break;
            case 3:
                resourceName = "Hexagon_Grassland";
                friendlyName = "Farm Land";
                break;
            case 4:
                resourceName = "Hexagon_Forest";
                friendlyName = "Forest";
                break;
            case 5:
                resourceName = "Hexagon_Grassland";
                friendlyName = "Grassland";
                break;
        }
    }

    private void createTile(float newX, float newY, float newZ)
    {
        float tileRot = 0.0f;
        switch (Random.Range(0, 5))
        {
            case 0:
                tileRot = 30.0f;
                break;
            case 1:
                tileRot = 90.0f;
                break;
            case 2:
                tileRot = 150.0f;
                break;
            case 3:
                tileRot = 210.0f;
                break;
            case 4:
                tileRot = 270.0f;
                break;
            case 5:
                tileRot = 330.0f;
                break;
        }

        //gets the type of tile that will be spawned
        if (initalOwnedTileMade == true)
        {
            //ensure the last tile will always have the specific building
            if (curTile != tile.Length -1)
                getTileType();
            else
                getStartingBuilding();
        }
        else
        {
            getStartingOwnedTileType();
        }


        //instantiate the first element from the queue and then remove it from the queue
        tile[curTile] = Instantiate(Resources.Load(resourceName), new Vector3(newX, newY, newZ), Quaternion.identity) as GameObject;
        tile[curTile].transform.eulerAngles = new Vector3(0.0f, tileRot, 0.0f);

        //only called on the first tile
        if (initalOwnedTileMade == false)
        {
            tile[curTile].GetComponent<TileInformation>().setPlayerOwns(true);
            tile[curTile].transform.Find("TileHighlight").GetComponent<SpriteRenderer>().color = new Color(178.0f / 255, 14.0f / 255, 14.0f / 255);

            initalOwnedTileMade = true;
        }

        tile[curTile].GetComponent<TileInformation>().setTileID(curTile);
        tile[curTile].GetComponent<TileInformation>().setLandType(friendlyName);

        curTile++;
    }

    private void Awake()
    {
        //expand the array
        tile = new GameObject[tileWidth * tileHeight];

        for (int x = 0; x < tileWidth; x++)
        {
            for (int z = 0; z < tileHeight; z++)
            {
                float tmpX = (x + z * 0.5f - z / 2) * (innerRadius * 2.0f);
                float tmpZ = z * (outerRadius * 1.5f);

                createTile(tmpX, 7.0f, tmpZ);
            }
        }

        //hard coded, will need to change this
        for (int i = 0; i < tile.Length; i++)
        {
            tile[i].transform.position = new Vector3(tile[i].transform.position.x - 29.54f, tile[i].transform.position.y, tile[i].transform.position.z - 25.62f);
        }
    }

    // Start is called before the first frame update
    private void Start() {}

    //return all the tiles
    public GameObject[] getTiles()
    {
        return tile;
    }

    //return 1 tile
    public GameObject getSingleTile(int index)
    {
        return tile[index];
    }
}
