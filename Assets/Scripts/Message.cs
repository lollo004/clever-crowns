using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : Card
{
    public void ChangeTurnAndPhase(string [] msg)
    {
        //increment ttl
        if (msg[0] == team && msg[1] == stopPhase) //remove stop if a turn has passed
        {
            isStopped = false;
        }
    }
}
