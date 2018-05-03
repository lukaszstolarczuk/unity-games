using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public Dictionary<EnumColour, int> Requirements;
    public Dictionary<EnumColour, int> Costs;

    public int PrestigePoints;
    public Dictionary<EnumColour, int> Bonus;
}
