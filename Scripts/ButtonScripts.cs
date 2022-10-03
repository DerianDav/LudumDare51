using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonScripts : MonoBehaviour
{
    public GameController game;
    public TextMeshProUGUI startButton;
    public TextMeshProUGUI helpText;
    public Canvas helpScreenCanvas;

    private string controlsText =
        "Click on Yellow Troop to select \n"+
        "When Selected:\n" +
        "WASD to move\n" +
        "Bottom right = Troop stats\n" +
        "Left/Mid = Global reserve\n" +
        "Green Arrows increases troop stats using up your reserve\n" +
        "\nUsage per click:\n" +
        "Troops - 100    max - 999\n" +
        "Water - 50    max - 100\n" +
        "Food - 50    max - 100\n" +
        "Ammo - 100"
        ;

    public void troopButton()
    {
        game.ResupplyUnit(null, Enums.Resource.TROOPS);
    }
    public void waterButton()
    {
        game.ResupplyUnit(null, Enums.Resource.WATER);
    }
    public void foodButton()
    {
        game.ResupplyUnit(null, Enums.Resource.FOOD);
    }
    public void ammoButton()
    {
        game.ResupplyUnit(null, Enums.Resource.AMMO);
    }

    public void HelpScreenNextButton()
    {
        if (startButton.text == "Next")
        {
            startButton.SetText("Start");
            helpText.SetText(controlsText);
        }
        else
        {
            helpScreenCanvas.gameObject.SetActive(false);
            game.startGame();
        }
    }
}
