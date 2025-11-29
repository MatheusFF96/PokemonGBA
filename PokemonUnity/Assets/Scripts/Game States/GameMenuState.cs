using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : State<GameController>
{
    [SerializeField] MenuController menuController;

    public static GameMenuState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelcted;
        menuController.OnBack += OnBack;
    }

    public override void Execute()
    {
        menuController.HandleUpdate();
    }

    public override void Exit()
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelcted;
        menuController.OnBack -= OnBack;
    }

    void OnMenuItemSelcted(int selection)
    {
        if (selection == 0) // Pokemon
            gc.StateMachine.Push(PartyState.i);
        else if (selection == 1) // Bag
            gc.StateMachine.Push(InventoryState.i);
    }

    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
