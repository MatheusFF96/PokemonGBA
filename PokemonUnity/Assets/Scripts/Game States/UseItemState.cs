using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseItemState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    public static UseItemState i { get; private set; }
    Inventory inventory;
    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

        StartCoroutine(UseItem());
    }

    IEnumerator UseItem()
    {
        var item = inventoryUI.SelectedItem;
        var pokemon = partyScreen.SelectedMember;

        if (item is TMItem)
        {
            yield return HandleTmItems();
        }
        else
        {
            // Handle Evolution Items
            if (item is EvolutionItem)
            {
                var evolution = pokemon.CheckForEvolution(item);
                if (evolution != null)
                {
                    yield return EvolutionManager.i.Evolve(pokemon, evolution);
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialogText($"Não teve nenhum efeito.");
                    gc.StateMachine.Pop();
                    yield break;
                }
            }

            var usedItem = inventory.UseItem(item, partyScreen.SelectedMember);
            if (usedItem != null)
            {
                if (usedItem is RecoveryItem)
                    yield return DialogManager.Instance.ShowDialogText($"O player usou {usedItem.Name}.");
            }
            else
            {
                if (inventoryUI.SelectedCategory == (int)ItemCategory.Items)
                    yield return DialogManager.Instance.ShowDialogText($"Não teve nenhum efeito.");
            }
        }

        gc.StateMachine.Pop();
    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventoryUI.SelectedItem as TMItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} já aprendeu {tmItem.Move.Name}.");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} não pode aprender {tmItem.Move.Name}.");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} aprendeu {tmItem.Move.Name}.");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} está tentando aprender {tmItem.Move.Name}.");
            yield return DialogManager.Instance.ShowDialogText($"Mas não pode aprender mais de {PokemonBase.MaxNumOfMoves} habilidades.");

            yield return DialogManager.Instance.ShowDialogText($"Escolha uma habilidade que você quer esquecer.", true, false);

            MoveToForgetState.i.NewMove = tmItem.Move;
            MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
            yield return gc.StateMachine.PushAndWait(MoveToForgetState.i);

            int moveIndex = MoveToForgetState.i.Selection;
            if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
            {
                // Don't learn the new move
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} não aprendeu {tmItem.Move.Name}.");
            }
            else
            {
                // Forget the selected move and learn new move
                var selectedMove = pokemon.Moves[moveIndex].Base;
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} esqueceu {selectedMove.Name} e aprendeu {tmItem.Move.Name}.");

                pokemon.Moves[moveIndex] = new Move(tmItem.Move);
            }
        }
    }
}
