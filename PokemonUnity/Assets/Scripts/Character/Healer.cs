using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("Você parece cansado! Gostaria de descansar?", 
            choices: new List<string>() { "Sim, por favor", "Não, obrigado" },
            onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Yes
            yield return Fader.i.FadeIn(0.5f);

            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.5f);

            yield return DialogManager.Instance.ShowDialogText($"Seus pokemons devem estar completamente curados!");
        }
        else if(selectedChoice == 1)
        {
            // No
            yield return DialogManager.Instance.ShowDialogText($"Okay! Volte se mudar de ideia.");
        }

        
    }
}
