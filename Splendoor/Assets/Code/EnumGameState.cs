using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumGameState
{
    NOT_INITALIZED,
    GAME_SETUP,

    BEGINNING_OF_TURN,
    MAKING_TURN_ACTION,
    TURN_ACTION_DONE,
    ACCEPT_NOBLES,
    END_OF_TURN,

    GAME_END
}
