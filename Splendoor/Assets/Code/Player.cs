using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player
{
    public List<DevelopmentCard> DevelopmentCards;
    public List<NobleTile> NobleTiles;
    public Dictionary<EnumColour, int> Tokens;
    public List<Card> ReservedCards;

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

    public void GetToken(EnumColour colour)
    {
        if (GameBoard.Instance.AvailableTokens.ContainsKey(colour) &&
            GameBoard.Instance.AvailableTokens[colour] > 0)
        {
            GameBoard.Instance.AvailableTokens[colour]--;
            if (!Tokens.ContainsKey(colour))
                Tokens.Add(colour, 0);
            Tokens[colour]++;
        }
    }

    public void GetTokens(Dictionary<EnumColour, int> tokens)
    {
        if (!GameBoard.Instance.CanGetTokens(tokens))
            return;

        foreach (EnumColour col in tokens.Keys)
            for (int i = 0; i < tokens[col]; i++)
                GetToken(col);
    }

    public void ReturnToken(EnumColour colour)
    {
        if(Tokens.ContainsKey(colour) && Tokens[colour] > 0)
        {
            Tokens[colour]--;
            if (!GameBoard.Instance.AvailableTokens.ContainsKey(colour))
                GameBoard.Instance.AvailableTokens.Add(colour, 0);
            GameBoard.Instance.AvailableTokens[colour]++;
        }
    }

    public void ReserveRevealedCard(DevelopmentCard card)
    {
        GameBoard.Instance.RevealedDevCards.Remove(card);
        ReservedCards.Add(card);
        GetToken(EnumColour.GOLD);
    }

    public void ReserveTopDeckCard(Deck<Card> deck)
    {
        if (deck.Cards.Count > 0)
            ReservedCards.Add(deck.Draw());
        GetToken(EnumColour.GOLD);
    }

    public void BuyDevelopmentCard(DevelopmentCard card)
    {
        if (!CanBuyCard(card))
            return;

        Dictionary<EnumColour, int> costs = CostToPay(card);
        foreach (EnumColour col in costs.Keys)
            while (costs[col] > 0)
            {
                if (Tokens[col] > 0)
                {
                    ReturnToken(col);
                    costs[col]--;
                }
                else if (Tokens[EnumColour.GOLD] > 0)
                {
                    ReturnToken(EnumColour.GOLD);
                    costs[col]--;
                }
                else
                    throw new NotImplementedException("BuyDevelopmentCard(): can't pay cost; CanBuyCard() returned invalid result");
            }

        if (ReservedCards.Contains(card))
            ReservedCards.Remove(card);
        else if (GameBoard.Instance.RevealedDevCards.Contains(card))
        {
            GameBoard.Instance.RevealedDevCards.Remove(card);
            GameBoard.Instance.RevealDevelopmentCards();
        }
        else
            throw new NotImplementedException("BuyDevelopmentCard(): unknown card origin");

        DevelopmentCards.Add(card);
    }

    public void AcceptNobleTile(NobleTile tile)
    {
        if(!FulfillsRequirements(tile.Requirements))
            return;

        if (GameBoard.Instance.AvailableNobleTiles.Contains(tile))
            GameBoard.Instance.AvailableNobleTiles.Remove(tile);
        else
            throw new NotImplementedException("AcceptNobleTile(): unknown tile origin");

        NobleTiles.Add(tile);
    }
}
