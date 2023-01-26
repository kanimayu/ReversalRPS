using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardBase : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] int number;
    [SerializeField] int level;
    [SerializeField] CardType type;
    [SerializeField] Sprite icon;
    [SerializeField] Sprite cardFront;
    [TextArea]
    [SerializeField] string description;

    public string Name { get => name; }
    public int Level { get => level; }
    public CardType Type { get => type;}
    public int Number { get => number; }
    public Sprite Icon { get => icon; }
    public Sprite CardFront { get => cardFront; }
    public string Description { get => description; }
}

public enum CardType
{
    Rock,
    Paper,
    Scissors,
    Joker
}