using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck<Card>
{
    public List<Card> Cards = new List<Card>();
    

    public Card Draw()
    {
        if (Cards.Count == 0)
            return default(Card);

        Card top = Cards[0];
        Cards.Remove(top);
        return top;
    }

    public void PutOnTop(Card card)
    {
        Cards.Insert(0, card);
    }

    public void Shuffle()
    {
        List<Card> tmp = new List<Card>();
        while (Cards.Count > 0)
            tmp.Add(Draw());

        while(tmp.Count > 0)
        {
            int rand = Random.Range(0, tmp.Count);
            Card c = tmp[rand];
            PutOnTop(c);
            tmp.RemoveAt(rand);
        }
    }
}
