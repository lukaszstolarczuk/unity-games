using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckTestBehaviour : MonoBehaviour
{
    Deck<int> test = new Deck<int>();

	// Use this for initialization
	void Start ()
    {
        string msg = string.Empty;

        for (int i = 0; i < 15; i++)
            test.PutOnTop(i + 1);

        Debug.Log(DeckContent());
        test.Shuffle();
        Debug.Log(DeckContent());
        test.Shuffle();
        Debug.Log(DeckContent());

        while(test.Cards.Count > 10)
        {
            Debug.Log("Drew: " + test.Draw());
            Debug.Log(DeckContent());
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    string DeckContent()
    {
        string msg = "Deck Content: ";
        foreach (int c in test.Cards)
            msg += c + ", ";
        return msg;
    }
}
