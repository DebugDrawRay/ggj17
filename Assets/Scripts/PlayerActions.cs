using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
public class PlayerActions : PlayerActionSet
{
    public PlayerAction Left;
    public PlayerAction Right;

    public PlayerOneAxisAction Steer;

    public PlayerAction Quit;
    public PlayerActions()
    {
        Left = CreatePlayerAction("Steer Left");
        Right = CreatePlayerAction("Steer Right");

        Steer = CreateOneAxisPlayerAction(Left, Right);

        Quit = CreatePlayerAction("Quit");
    }

    public static PlayerActions BindAll()
    {
        PlayerActions actions = new PlayerActions();

        actions.Left.AddDefaultBinding(Key.A);
        actions.Right.AddDefaultBinding(Key.D);
        actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        actions.Quit.AddDefaultBinding(InputControlType.Menu);
        actions.Quit.AddDefaultBinding(InputControlType.Options);
        actions.Quit.AddDefaultBinding(InputControlType.Start);
        actions.Quit.AddDefaultBinding(InputControlType.View);
        actions.Quit.AddDefaultBinding(InputControlType.Back);
        actions.Quit.AddDefaultBinding(InputControlType.Home);

        return actions;
    }
}
