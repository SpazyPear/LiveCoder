using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorMath
{

}

public class VectorMathProxy
{
    public Vector2 add (Vector2Int a, Vector2Int b)
    {
        return a + b;
    }

    public Vector2 subtract(Vector2Int a, Vector2Int b)
    {
        return a - b;
    }

    public Vector2 dot(Vector2Int a, Vector2Int b)
    {
        return a * b;
    }

    public Vector2 scale(Vector2Int a, float scale)
    {
        return new Vector2(a.x, a.y) * scale;
    }


    public float dist(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a,b);
    }

}
