using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using Random = UnityEngine.Random;

public class SquadController
{
    public int troopCount { get; set; }
    public int food { get; set; }
    public int water { get; set; }
    public int ammo { get; set; }
    public int locationX { get; set; }
    public int locationY { get; set; }
    public bool isFighting { get; set; }
    public Direction curMovement { get; set; }
    public Faction faction { get; }

    public SquadController (int x, int y, Faction faction)
    {
        locationX = x;
        locationY = y;
        this.faction = faction;
        food = 100;
        water = 100;
        ammo = 100;
        troopCount = 1000;
        isFighting = false;
        curMovement = Direction.STATIONARY;
    }
    
    public bool IsDead()
    {
        if (troopCount <= 0)
            return true;
        if (isFighting && ammo <= 0)
            return true;
        else
            return false;
    }

    public void TenSecondsStep()
    {
        food = Math.Max(food - 10, 0);
        water = Math.Max(water - 10,0);
        if (isFighting)
        {
            troopCount = Math.Max(troopCount - 100,0);
            ammo = Math.Max(ammo - 20,0);
        }
        if (food == 0)
            troopCount = troopCount - 75;
        else if (food < 50)
            troopCount = troopCount - 25;
        if (water == 0)
            troopCount = troopCount - 100;

        //AI get +15 food and water if not fighting (75% chance)
        if(faction == Faction.Green && !isFighting)
        {
            int rand = Random.Range(1, 100);
            if (rand > 25)
            {
                food = Math.Min(food + 15, 100);
                water = Math.Min(water + 15, 100);
            }
        }
    }

    public void MoveSquad(Direction direction)
    {
        if (isFighting)
            return;
        switch (direction)
        {
            case Direction.LEFT:
                locationX = locationX - 1;
                break;
            case Direction.RIGHT:
                locationX = locationX + 1;
                break;
            case Direction.UP:
                locationY = locationY + 1;
                break;
            case Direction.DOWN:
                locationY = locationY - 1;
                break;

        }
    }

}
