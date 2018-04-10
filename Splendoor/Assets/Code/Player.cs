using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<DevelopmentCard> DevelopmentCards;
    public List<NobleTile> NobleTiles;
    public Dictionary<EnumColour, int> Tokens;

    public int PrestigePoints
    {
        get
        {
            int sum = 0;
            foreach (DevelopmentCard dc in DevelopmentCards)
                sum += dc.PrestigePoints;
            foreach (NobleTile nt in NobleTiles)
                sum += nt.PrestigePoints;
            return sum;
        }
    }

    public int GetBonusAmount(EnumColour colour)
    {
        int sum = 0;
        foreach (DevelopmentCard dc in DevelopmentCards)
            if (dc.Bonus.ContainsKey(colour))
                sum += dc.Bonus[colour];
        return sum;
    }

    public bool FulfillsRequirements(Dictionary<EnumColour, int> req)
    {
        foreach (EnumColour col in req.Keys)
            if (GetBonusAmount(col) < req[col])
                return false;
        return true;
    }

    public Dictionary<EnumColour, int> CostToPay(DevelopmentCard card)
    {
        Dictionary<EnumColour, int> costToPay = new Dictionary<EnumColour, int>();
        foreach (EnumColour col in card.Costs.Keys)
        {
            costToPay.Add(col, card.Costs[col] - GetBonusAmount(col));
            if (costToPay[col] < 0)
                costToPay[col] = 0;
        }

        return costToPay;
    }

    public bool CanBuyCard(DevelopmentCard card)
    {
        if (!FulfillsRequirements(card.Requirements))
            return false;

        Dictionary<EnumColour, int> costs = CostToPay(card);
        foreach (EnumColour col in costs.Keys)
        {
            costs[col] -= Tokens[col];
            if (costs[col] < 0)
                costs[col] = 0;
        }
        int goldRequired = 0;
        foreach (EnumColour col in costs.Keys)
            goldRequired += costs[col];

        return Tokens.ContainsKey(EnumColour.GOLD) && goldRequired <= Tokens[EnumColour.GOLD];
    }
}
