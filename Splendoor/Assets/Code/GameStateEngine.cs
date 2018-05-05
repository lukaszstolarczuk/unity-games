using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateEngine
{

    #region singleton region
    private GameStateEngine instance;

    private GameStateEngine()
    {
        legalTrasitions = new Dictionary<EnumGameState, List<EnumGameState>>();

        legalTrasitions.Add(EnumGameState.NOT_INITALIZED, new List<EnumGameState>() { EnumGameState.GAME_SETUP });
        legalTrasitions.Add(EnumGameState.GAME_SETUP, new List<EnumGameState>() { EnumGameState.BEGINNING_OF_TURN });
        legalTrasitions.Add(EnumGameState.BEGINNING_OF_TURN, new List<EnumGameState>() { EnumGameState.MAKING_TURN_ACTION, EnumGameState.GAME_END });
        legalTrasitions.Add(EnumGameState.MAKING_TURN_ACTION, new List<EnumGameState>() { EnumGameState.TURN_ACTION_DONE });
        legalTrasitions.Add(EnumGameState.TURN_ACTION_DONE, new List<EnumGameState>() { EnumGameState.ACCEPT_NOBLES });
        legalTrasitions.Add(EnumGameState.ACCEPT_NOBLES, new List<EnumGameState>() { EnumGameState.END_OF_TURN });
        legalTrasitions.Add(EnumGameState.END_OF_TURN, new List<EnumGameState>() { EnumGameState.BEGINNING_OF_TURN, EnumGameState.GAME_END });
        legalTrasitions.Add(EnumGameState.GAME_END, new List<EnumGameState>());
    }

    public GameStateEngine Instance
    {
        get
        {
            if (instance == null)
                instance = new GameStateEngine();
            return instance;
        }
    }
    #endregion

    #region private variables

    private bool initalized = false;
    private bool endgame = false;
    private EnumGameState gamestate = EnumGameState.NOT_INITALIZED;
    private Dictionary<EnumGameState, List<EnumGameState>> legalTrasitions;
    
    // indicated whos turn it is
    private int activePlayerIterator = 0;
    
    // indicates who can play action card (this + activePlayerIterator % number of players)
    private int responsivePlayerIterator = 0;

    // indicates how long players fulfills winning condition
    private List<int> winningTurns;

    #endregion

    public EnumGameState CurrentGameState { get { return gamestate; } }

    public List<Player> AttendingPlayers;

    public Player ActivePlayer { get { return AttendingPlayers[activePlayerIterator]; } }

    public Player ResponsivePlayer
    {
        get {
            return AttendingPlayers[(activePlayerIterator + responsivePlayerIterator) % AttendingPlayers.Count];
        }
    }
    
    public void Initalize(List<Player> players)
    {
        AttendingPlayers = new List<Player>(players);
        winningTurns = new List<int>();
        for (int i = 0; i < AttendingPlayers.Count; i++)
            winningTurns.Add(0);
        initalized = true;
        GameStateTransition(EnumGameState.GAME_SETUP);
    }

    public void ReportEvent(EnumGameEvent e)
    {
        if (!initalized)
            throw new InvalidOperationException("ReportEvent(): Engine not initalized");

        switch (e)
        {
            case EnumGameEvent.ACTION_CARD_PLAYED:
                responsivePlayerIterator = 0;
                break;
            case EnumGameEvent.ACTION_CARD_SKIPPED:
                responsivePlayerIterator++;
                if(responsivePlayerIterator == AttendingPlayers.Count)
                {
                    if (CurrentGameState == EnumGameState.BEGINNING_OF_TURN)
                        GameStateTransition(EnumGameState.MAKING_TURN_ACTION);
                    else //(CurrentGameState == EnumGameState.TURN_ACTION_DONE)
                        GameStateTransition(EnumGameState.ACCEPT_NOBLES);
                }
                break;
            case EnumGameEvent.TOKENS_AQUIRED:
                GameStateTransition(EnumGameState.TURN_ACTION_DONE);
                break;
            case EnumGameEvent.REVEALED_CARD_BOUGHT:
                GameStateTransition(EnumGameState.TURN_ACTION_DONE);
                break;
            case EnumGameEvent.RESERVED_CARD_GOUGHT:
                GameStateTransition(EnumGameState.TURN_ACTION_DONE);
                break;
            case EnumGameEvent.REVEALED_CARD_RESERVED:
                GameStateTransition(EnumGameState.TURN_ACTION_DONE);
                break;
            case EnumGameEvent.TOP_DECK_CARD_RESERVED:
                GameStateTransition(EnumGameState.TURN_ACTION_DONE);
                break;
            case EnumGameEvent.NOBLE_ACCEPTED:
                GameStateTransition(EnumGameState.END_OF_TURN);
                break;
            default:
                throw new NotImplementedException(string.Format("ReportEvent() not implemented for event {0}", e));
        }
    }

    private bool VerifyTrasitionLegality(EnumGameState gotoState)
    {
        return legalTrasitions[CurrentGameState].Contains(gotoState);
    }

    private void GameStateTransition(EnumGameState gotoState)
    {
        if (!initalized)
            throw new InvalidOperationException("ReportEvent(): Engine not initalized");

        if (VerifyTrasitionLegality(gotoState) == false)
            throw new NotImplementedException(string.Format("GameStateTransition(): Invalid trasition from {0} to {1} game state", gamestate, gotoState));

        switch (gotoState)
        {
            case EnumGameState.GAME_SETUP:
                gamestate = gotoState;
                GameBoard.Instance.SetupGame(AttendingPlayers.Count);
                GameStateTransition(EnumGameState.BEGINNING_OF_TURN); // consider sending event after setting up the game...
                break;
            case EnumGameState.BEGINNING_OF_TURN:
                gamestate = gotoState;
                if (ActivePlayer.FulfillsWinningConditions() 
                    && winningTurns[activePlayerIterator] == Constants.TURNS_REQUIRED_TO_WIN)
                    GameStateTransition(EnumGameState.GAME_END);
                break;
            case EnumGameState.MAKING_TURN_ACTION:
                gamestate = gotoState;
                break;
            case EnumGameState.TURN_ACTION_DONE:
                gamestate = gotoState;
                break;
            case EnumGameState.ACCEPT_NOBLES:
                gamestate = gotoState;
                break;
            case EnumGameState.END_OF_TURN:
                gamestate = gotoState;

                if (ActivePlayer.FulfillsWinningConditions() && Constants.TURNS_REQUIRED_TO_WIN == 0)
                    endgame = true;
                else if (ActivePlayer.FulfillsWinningConditions())
                    winningTurns[activePlayerIterator]++;
                else
                    winningTurns[activePlayerIterator] = 0;

                if (endgame && activePlayerIterator == AttendingPlayers.Count - 1)
                    GameStateTransition(EnumGameState.GAME_END);

                activePlayerIterator = (activePlayerIterator + 1) % AttendingPlayers.Count;
                GameStateTransition(EnumGameState.BEGINNING_OF_TURN);
                break;
            default:
                throw new NotImplementedException(string.Format("GameStateTransition(): Not implemented trasition to {0}", gotoState));
        }
    }
}
