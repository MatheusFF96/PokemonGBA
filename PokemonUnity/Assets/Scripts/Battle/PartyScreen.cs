using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : SelectionUI<TextSlot>
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;
    PokemonParty party;

    public Pokemon SelectedMember => pokemons[selectedItem];

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.Grid, 2);

        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }
    
    public void SetPartyData()
    {
        pokemons = party.Pokemons;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(pokemons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        var textSlots = memberSlots.Select(m => m.GetComponent<TextSlot>());
        SetItems(textSlots.Take(pokemons.Count).ToList());

        messageText.text = "Escolha um pokemon!";
    }    

    public void ShowIfTmIsUsable(TMItem tmItem)
    {
        for(int i = 0; i < pokemons.Count; i++)
        {
            string message = tmItem.CanBeTaught(pokemons[i]) ? "APRENDE!" : "NÃO APRENDE!";
            memberSlots[i].SetMessage(message);
        }
    }

    public void ClearMemberSlotMessage()
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            memberSlots[i].SetMessage("");
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
