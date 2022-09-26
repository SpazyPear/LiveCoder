using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command 
{
    
}

public class MoveCommand : Command
{
    public int x;
    public int y;
    public MoveCommand(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class AttackCommand : Command
{
    public Unit toAttack;
    
    public AttackCommand(Unit toAttack)
    {
        this.toAttack = toAttack;
    }
}

public static class CommandManager
{
    public static void ExecuteCommand()
    {
        
    }
}
