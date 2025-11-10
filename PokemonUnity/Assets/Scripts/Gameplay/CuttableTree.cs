using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("Esta árvore parece que pode ser cortada.");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Cut"));

        if (pokemonWithCut != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} deve usar Cut?",
                choices: new List<string>() { "Sim", "Não" }, 
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                // Yes
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} usou Cut.");
                gameObject.SetActive(false);
            }
        }
    }
}
