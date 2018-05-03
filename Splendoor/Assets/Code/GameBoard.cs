using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBoard
{
    private static GameBoard instance;

    public static GameBoard Instance
    {
        get
        {
            if (instance == null)
                instance = new GameBoard();
            return instance;
        }
    }

    private GameBoard()
    {
        throw new NotImplementedException("GameBoard() not implemented constructor");
    }

    public List<Deck<DevelopmentCard>> DevCardsDecks;
    public Deck<ActionCard> ActionCards;
    public List<DevelopmentCard> RevealedDevCards;
    public Deck<NobleTile> AllNobleTiles;
    public List<NobleTile> AvailableNobleTiles;
    public Dictionary<EnumColour, int> AvailableTokens;

    public void SetupGame(int numberOfPlayers)
    {
        throw new NotImplementedException("SetupGame() not implemented");
    }
    
    public void RevealDevelopmentCards()
    {
        Dictionary<int, int> revealedCards = new Dictionary<int, int>();
        for(int i=0; i<Constants.NUMBER_OF_CARD_TIERS; i++)
            revealedCards.Add(i, 0);

        if (RevealedDevCards == null)
            RevealedDevCards = new List<DevelopmentCard>();

        foreach(DevelopmentCard card in RevealedDevCards)
        {
            if (!revealedCards.ContainsKey(card.Tier))
                throw new NotImplementedException("RevealDevelopmentCards(): not implemented case of unknown card tier");
            revealedCards[card.Tier]++;
        }

        for(int i=0; i<=2; i++)
        {
            while(revealedCards[i] < Constants.NUMBER_OF_REVEALED_CARDS && DevCardsDecks[i].Cards.Count > 0)
            {
                RevealedDevCards.Add(DevCardsDecks[i].Draw());
            }
        }
    }

    public bool CanGetTokens(Dictionary<EnumColour, int> requestedTokens)
    {
        foreach (EnumColour col in requestedTokens.Keys)
            if (requestedTokens[col] > AvailableTokens[col])
                return false;

        foreach (EnumColour col in requestedTokens.Keys)
            if (requestedTokens[col] == 2 && AvailableTokens[col] < Constants.NUMBER_OF_TOKENS_ENABLING_DOUBLE_TOKEN_PICK)
                return false;

        int total = 0;
        foreach (EnumColour col in requestedTokens.Keys)
            total += requestedTokens[col];

        if (total > 3)
            return false;

        return true;
    }
}
