using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorMath
{

}

public class MathHelpers
{

}

public class MathProxy { 

    public int round (float val)
    {
        return Mathf.RoundToInt(val);
    }

}
public class VectorMathProxy
{
    public Vector2Float add (Vector2Float a, Vector2Float b)
    {
        return Vector2Float.fromVec2(a.from() + b.from());
    }

    public Vector2Float subtract(Vector2Float a, Vector2Float b)
    {
        return Vector2Float.fromVec2(a.from() - b.from());
    }

    public Vector2Float dot(Vector2Float a, Vector2Float b)
    {
        return Vector2Float.fromVec2(a.from() * b.from());
    }

    public Vector2Float scale(Vector2Float a, float scale)
    {
        return Vector2Float.fromVec2(a.from() * scale);
    }


    public float dist(Vector2Float a, Vector2Float b)
    {
        return Vector2.Distance(a.from(),b.from());
    }

}

public class Vector2Float
{
    public float x;
    public float y;

    public Vector2Float(float _x, float _y)
    {
        this.x = _x;
        this.y = _y;
    }

   public  static Vector2Float fromVec2 (Vector2 v)
    {
        return new Vector2Float(v.x, v.y);
    }

    public Vector2 from ()
    {
        return new Vector2(x, y);
    }

}

public class Vector2Proxy
{
    Vector2Float target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public Vector2Proxy(Vector2Float p)
    {
        this.target = p;
    }

    public float x => target.x;
    public float y => target.y;

}