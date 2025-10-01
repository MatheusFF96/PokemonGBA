using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "foi envenenado!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} sofreu dano de poison!");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "foi queimado!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} sofreu dano de burn!");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "foi paralizado!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} est� paralizado e n�o pode se mover!");
                        return false;
                    }

                    return true;
                }                
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "foi congelado!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} n�o est� mais congelado!");
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "caiu no sono!",
                OnStart = (Pokemon pokemon) =>
                {
                    // Sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"ficar� dormindo por {pokemon.StatusTime} turnos");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} acordou!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} est� dormindo!");
                    return false;
                }
            }
        },

        // Volatile Status Conditions
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "ficou confuso!",
                OnStart = (Pokemon pokemon) =>
                {
                    // Confused for 1-4 turns
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"ficar� confuso por {pokemon.VolatileStatusTime} turnos");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} n�o est� mais confuso!");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;

                    // 50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                        return true;

                    // Hurt by confusion
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} est� confuso!");
                    pokemon.DecreaseHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} machucou-se devido a confus�o!");
                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
            return 1.5f;

        return 1f;
    }
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, confusion
}
