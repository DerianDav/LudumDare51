using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Enums;

public class GameController : MonoBehaviour
{
    public Tilemap tileMap;
    public Tilemap squadMap;
    public TileBase mountainTile;
    public TileBase grassTile;
    public TileBase gSoldierTile;
    public TileBase ySoldierTile;
    public TileBase ySoldierUpTile;
    public TileBase ySoldierLeftTile;
    public TileBase ySoldierRightTile;
    public TileBase ySoldierDownTile;
    public TileBase ySoldierSelectedTile;
    public TileBase ySoldierUpSelectedTile;
    public TileBase ySoldierLeftSelectedTile;
    public TileBase ySoldierRightSelectedTile;
    public TileBase ySoldierDownSelectedTile;


    public TextMeshProUGUI ySoldierCounterText;
    public TextMeshProUGUI gSoldierCounterText;

    public TextMeshProUGUI troopText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI squadTroopText;
    public TextMeshProUGUI squadWaterText;
    public TextMeshProUGUI squadFoodText;
    public TextMeshProUGUI squadAmmoText;
    public TextMeshProUGUI endGameText;

    private SquadController selectedSquad = null;

    int gameTime = 0;
    int reserveTroops { get; set; }
    int food { get; set; }
    int water { get; set; }
    int ammo { get; set; }
    int cash { get; set; }

    private MapTerrain[,] map = new MapTerrain[20, 10];

    private ArrayList squadList = new ArrayList();
    private int greenSquads = 0;
    private int yellowSquads = 0;

    bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        cash = 10000;
        ammo = 1000;
        reserveTroops = 3000;
        food = 1000;
        water = 1000;

        RandomizeMap();
        PrintMap();
        
        SetTileMap();
        while(greenSquads < 5)
            SpawnUnits(Faction.Green);
        SpawnUnits(Faction.Yellow);

        RefreshMap();

    }


    
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;
        gameTime++;
        troopText.SetText(reserveTroops+"");
        waterText.SetText(water + "");
        foodText.SetText(food + "");
        ammoText.SetText(ammo + "");

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y > 50)
        {
            int xLoc = (int)(Input.mousePosition.x / (Screen.width / map.GetLength(0)));
            int yLoc = (int)(Input.mousePosition.y / (Screen.height / map.GetLength(1)));
            ClickOnSquad(xLoc, yLoc);
        }

        if (selectedSquad != null)
        {
            squadTroopText.SetText(selectedSquad.troopCount + "");
            squadFoodText.SetText(selectedSquad.food + "");
            squadWaterText.SetText(selectedSquad.water + "");
            squadAmmoText.SetText(selectedSquad.ammo + "");
        }
        else
        {
            squadTroopText.SetText("");
            squadFoodText.SetText("");
            squadWaterText.SetText("");
            squadAmmoText.SetText("");
        }

        if (Input.GetKeyDown(KeyCode.W) && selectedSquad != null)
        {
            if (IsValidMovement(selectedSquad, Direction.UP))
                selectedSquad.curMovement = Direction.UP;
        }
        if (Input.GetKeyDown(KeyCode.D) && selectedSquad != null)
        {
            if (IsValidMovement(selectedSquad, Direction.RIGHT)) 
                selectedSquad.curMovement = Direction.RIGHT;
        }
        if (Input.GetKeyDown(KeyCode.S) && selectedSquad != null)
        {
            if (IsValidMovement(selectedSquad, Direction.DOWN))
                selectedSquad.curMovement = Direction.DOWN;
        }
        if (Input.GetKeyDown(KeyCode.A) && selectedSquad != null)
        {
            if (IsValidMovement(selectedSquad, Direction.LEFT))
                selectedSquad.curMovement = Direction.LEFT;
        }

        ySoldierCounterText.SetText(yellowSquads + "");
        gSoldierCounterText.SetText(greenSquads + "");
        RefreshMap();
    }

    public void startGame()
    {
        isActive = true;
        InvokeRepeating("TenSecondRepeat", 1.0f, 10.0f);
    }
    private void TenSecondRepeat()
    {
        foreach (object o in squadList)
        {
            SquadController squad = (SquadController)o;
            if (EnemyInSurrondingTiles(squad))
                squad.isFighting = true;
            else
                squad.isFighting = false;
            if (squad.faction == Faction.Green && !squad.isFighting)
                MoveAI(squad);
            else if (squad.faction == Faction.Yellow)
            {
                squad.MoveSquad(squad.curMovement);
                squad.curMovement = Direction.STATIONARY;
            }
            squad.TenSecondsStep();
            if (squad.IsDead())
            {
                squadList.Remove(squad);
                if (squad.faction == Faction.Green)
                    greenSquads--;
                else
                    yellowSquads--;

                if (CheckEndGame())
                {
                    EndGameScreen();
                    return;
                }
               
            }
            
        }
        RefreshMap();
    }

    private void RandomizeMap()
    {
        for (int xIterator = 0; xIterator < map.GetLength(0); xIterator++)
        {
            for (int yIterator = 0; yIterator < map.GetLength(1); yIterator++)
            {
                int randNumb = Random.Range(1, 10);
               // Vector3Int loc = new Vector3Int(xIterator, yIterator, 0);
                switch (randNumb)
                {
                    case 1:
                        map[xIterator, yIterator] = MapTerrain.Mountain;
                    //    tileMap.SetTile(loc, mountainTile);
                        break;
                    default:
                        map[xIterator, yIterator] = MapTerrain.Field;
                     //   tileMap.SetTile(loc, grassTile);
                        break;
                }
            }
        }

    }

    //prints map in the Debug Log
    private void PrintMap()
    {
        string mapString = "";
        for (int xIterator = 0; xIterator < map.GetLength(0); xIterator++)
        {
            for (int yIterator = 0; yIterator < map.GetLength(1); yIterator++)
            {
                switch (map[xIterator, yIterator])
                {
                    case MapTerrain.Field:
                        mapString += "O";
                        break;
                    case MapTerrain.Mountain:
                        mapString += "X";
                        break;
                }
            }
            mapString += "\n";
        }
        Debug.Log(mapString);
    }

  

    private void SpawnUnits(Faction facColor)
    {
        int y;
        if (facColor == Faction.Green)
        {
            y = 7;
            squadList.Clear();
            greenSquads = 0;
        }
        else
            y = 3;
        bool secondPass = false;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            if (facColor == Faction.Green && greenSquads >= 8)
                return;
            if (facColor == Faction.Yellow && yellowSquads >= 3)
                return;
            if (map[x, y] == MapTerrain.Mountain)
                continue;
            if (secondPass)
            {
                bool foundAtLoc = false;
                foreach (object o in squadList)
                {
                    SquadController squad = (SquadController)o;
                    if (squad.faction != Faction.Yellow)
                        continue;
                    if (x == squad.locationX && y == squad.locationY)
                    {
                        foundAtLoc = true;
                        break;
                    }
                }
                if (!foundAtLoc)
                {
                    squadList.Add(new SquadController(x, y, facColor));
                    yellowSquads++;
                    continue;
                }
            }
            else if (Random.Range(0, 100) < 25)
            {
                squadList.Add(new SquadController(x, y, facColor));
                if (facColor == Faction.Green)
                    greenSquads++;
                else
                    yellowSquads++;
            }

            if (facColor == Faction.Yellow && x + 1 == map.GetLength(0) && yellowSquads != 3 && !secondPass)
            {
                x = 0;
                secondPass = true;
            }
        }
    }

    private void SetTileMap()
    {
        tileMap.ClearAllTiles();
        for (int xIterator = 0; xIterator < map.GetLength(0); xIterator++)
        {
            for (int yIterator = 0; yIterator < map.GetLength(1); yIterator++)
            {
                Vector3Int loc = new Vector3Int(xIterator, yIterator, 0);
                switch (map[xIterator,yIterator])
                {
                    case MapTerrain.Mountain:
                        tileMap.SetTile(loc, mountainTile);
                        break;
                    default:
                        tileMap.SetTile(loc, grassTile);
                        break;
                }
            }
        }
    }

    private void RefreshMap()
    {
        squadMap.ClearAllTiles();
        foreach (object o in squadList)
        {
            SquadController squad = (SquadController)o;
            Vector3Int loc = new Vector3Int(squad.locationX, squad.locationY, 0);
            if (squad.faction == Faction.Green)
                squadMap.SetTile(loc, gSoldierTile);
            else
            {
                if (squad.curMovement == Direction.UP)
                    squadMap.SetTile(loc, ySoldierUpTile);
                else if (squad.curMovement == Direction.DOWN)
                    squadMap.SetTile(loc, ySoldierDownTile);
                else if (squad.curMovement == Direction.RIGHT)
                    squadMap.SetTile(loc, ySoldierRightTile);
                else if (squad.curMovement == Direction.LEFT)
                    squadMap.SetTile(loc, ySoldierLeftTile);
                else
                    squadMap.SetTile(loc, ySoldierTile);
            }
        }

        if (selectedSquad != null && !selectedSquad.IsDead())
        {
            if (selectedSquad.curMovement == Direction.UP)
                squadMap.SetTile(new Vector3Int(selectedSquad.locationX, selectedSquad.locationY), ySoldierUpSelectedTile);
            else if (selectedSquad.curMovement == Direction.DOWN)
                squadMap.SetTile(new Vector3Int(selectedSquad.locationX, selectedSquad.locationY), ySoldierDownSelectedTile);
            else if (selectedSquad.curMovement == Direction.RIGHT)
                squadMap.SetTile(new Vector3Int(selectedSquad.locationX, selectedSquad.locationY), ySoldierRightSelectedTile);
            else if (selectedSquad.curMovement == Direction.LEFT)
                squadMap.SetTile(new Vector3Int(selectedSquad.locationX, selectedSquad.locationY), ySoldierLeftSelectedTile);
            else
                squadMap.SetTile(new Vector3Int(selectedSquad.locationX, selectedSquad.locationY), ySoldierSelectedTile);
        }
    }

    private void ClickOnSquad(int xLoc, int yLoc)
    {
        foreach(object o in squadList)
        {
            SquadController squad = (SquadController)o;
            if (squad.faction != Faction.Yellow)
                continue;
            if (xLoc == squad.locationX && yLoc == squad.locationY)
            {
                selectedSquad = squad;
                RefreshMap();
                return;
            }
        }
        selectedSquad = null;
        RefreshMap();
    }

    private void MoveAI(SquadController aiSquad)
    {
        int rand = Random.Range(1, 5);
        switch (rand)
            {
            case 1:
                if(IsValidMovement(aiSquad, Direction.LEFT)){ aiSquad.MoveSquad(Direction.LEFT); }
                break;
            case 2:
                if (IsValidMovement(aiSquad, Direction.UP)) { aiSquad.MoveSquad(Direction.UP); }
                break;
            case 3:
                if (IsValidMovement(aiSquad, Direction.RIGHT)) { aiSquad.MoveSquad(Direction.RIGHT); }
                break;
            case 4:
                if (IsValidMovement(aiSquad, Direction.DOWN)) { aiSquad.MoveSquad(Direction.DOWN); }
                break;
        }
        RefreshMap();
        
    }

    //checks to see if a squad can move in a given direction without hitting a mountain or the side of the map
    private bool IsValidMovement(SquadController squad, Direction direction)
    {
        int xLoc = squad.locationX;
        int yLoc = squad.locationY;

        if (direction == Direction.LEFT)
            xLoc--;
        else if (direction == Direction.UP)
            yLoc++;
        else if (direction == Direction.RIGHT)
            xLoc++;
        else
            yLoc--;

    //    Debug.Log(direction.ToString() + ": direction of squad ---" + xLoc + ", " + yLoc + "getLenght = " + map.GetLength(0));

        if (xLoc < 0 || xLoc >= map.GetLength(0))
            return false;
        if (yLoc < 0 || yLoc >= map.GetLength(1))
            return false;
        if (map[xLoc, yLoc] == MapTerrain.Mountain)
            return false;
        if (squadMap.GetTile(new Vector3Int(xLoc, yLoc)) != null)
            return false;
        return true;
    }

    //checks to see if there is an enemy in a 3x3 grid around the squad
    private bool EnemyInSurrondingTiles(SquadController squad)
    {
        int xLoc = squad.locationX;
        int yLoc = squad.locationY;
        for (int x = xLoc - 1; x <= xLoc + 1; x++)
        {
            if (x < 0 || x >= map.GetLength(0))
                continue;
            for (int y = yLoc - 1; y <= yLoc + 1; y++)
            {
                if (y < 0 || y >= map.GetLength(1))
                    continue;
                TileBase curTile = squadMap.GetTile(new Vector3Int(x, y));
                if (curTile == null)
                    continue;
                if (curTile == gSoldierTile && squad.faction == Faction.Yellow)
                    return true;
                if (curTile == ySoldierTile && squad.faction == Faction.Green)
                    return true;
            }
        }
        return false;
    }

    /*resupplys unit at given cord
     * Rates:
     *  Ammo - 100
     *  Food - 50;
     *  Water - 50;
     *  Troops - 100;
     */
    private void ResupplyUnit(int xLoc, int yLoc, Resource resup)
    {
        foreach (object o in squadList)
        {
            SquadController squad = (SquadController)o;
            if (squad.locationX != xLoc || squad.locationY != yLoc)
                continue;
            ResupplyUnit(squad, resup);
        }
    }
    public void ResupplyUnit(SquadController squad, Resource resup)
    {
        if (squad == null && selectedSquad == null)
            return;
        if(squad == null)
            squad = selectedSquad;
        switch (resup)
        {
            case Resource.AMMO:
                if (ammo > 0)
                {
                    squad.ammo += 100;
                    ammo -= 100;
                }
                break;
            case Resource.FOOD:
                if (food >= 50)
                {
                    squad.food = System.Math.Min(squad.food + 50, 100);
                    food -= 50;
                }
                break;
            case Resource.WATER:
                if (water > 0)
                {
                    squad.water = System.Math.Min(squad.water + 50, 100);
                    water -= 50;
                }
                break;
            case Resource.TROOPS:
                if (reserveTroops > 100)
                {
                    squad.troopCount = System.Math.Min(squad.troopCount + 100, 999);
                    reserveTroops -= 100;
                }
                break;

        }
    }





    private bool CheckEndGame()
    {
        if (greenSquads == 0 || yellowSquads == 0)
            return true;
        return false;
    }
    private void EndGameScreen()
    {
        if (greenSquads == 0)
            endGameText.SetText("GAME OVER: \n YOU WIN");
        if (yellowSquads == 0)
            endGameText.SetText("GAME OVER: \n YOU LOSE");
    }

    private void TestMovement(SquadController testSquad, int testCase)
    {
        Debug.Log("testcase: " + testCase);
        switch (testCase%4)
        {
            case 0:
                if (IsValidMovement(testSquad, Direction.LEFT)) { testSquad.MoveSquad(Direction.LEFT); }
                break;
            case 1:
                if (IsValidMovement(testSquad, Direction.UP)) { testSquad.MoveSquad(Direction.UP); }
                break;
            case 2:
                if (IsValidMovement(testSquad, Direction.RIGHT)) { testSquad.MoveSquad(Direction.RIGHT); }
                break;
            case 3:
                if (IsValidMovement(testSquad, Direction.DOWN)) { testSquad.MoveSquad(Direction.DOWN); }
                break;
        }
    }

    private void PrintSquadList()
    {

        string output = "";
        for (int i = 0; i < squadList.Count; i++)
        {
            SquadController curSquad = (SquadController)squadList[i];
            output += "Squad #" + i + ": Location (" + curSquad.locationX + " , " + curSquad.locationY + " )\n";
        }
        Debug.Log(output);
    }
}
