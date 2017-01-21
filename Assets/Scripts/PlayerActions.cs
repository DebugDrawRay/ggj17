using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class PlayerActions : PlayerActionSet
{
    public PlayerAction Left;
    public PlayerAction Right;

    public PlayerOneAxisAction Steer;

    public PlayerActions()
    {
        Left = CreatePlayerAction("Steer Left");
        Right = CreatePlayerAction("Steer Right");

        Steer = CreateOneAxisPlayerAction(Left, Right);
    }

    public static PlayerActions BindAll()
    {
        PlayerActions actions = new PlayerActions();

        actions.Left.AddDefaultBinding(Key.A);
        actions.Right.AddDefaultBinding(Key.D);
        actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        return actions;
    }
}
