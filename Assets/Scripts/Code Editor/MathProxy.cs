using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Python3DMath
{

    public class VectorMath
    {

    }

    public class MathHelpers
    {

    }

    public class MathProxy
    {

        public int round(float val)
        {
            return Mathf.RoundToInt(val);
        }

    }
    public class VectorMathProxy
    {
        public vector2 add(vector2 a, vector2 b)
        {
            return vector2.fromVec2(a.from() + b.from());
        }

        public vector2 subtract(vector2 a, vector2 b)
        {
            return vector2.fromVec2(a.from() - b.from());
        }

        public vector2 dot(vector2 a, vector2 b)
        {
            return vector2.fromVec2(a.from() * b.from());
        }

        public vector2 scale(vector2 a, float scale)
        {
            return vector2.fromVec2(a.from() * scale);
        }


        public float dist(vector2 a, vector2 b)
        {
            return Vector2.Distance(a.from(), b.from());
        }

    }

    public class vector2
    {
        public float x;
        public float y;

        public vector2(float _x, float _y)
        {
            this.x = _x;
            this.y = _y;
        }

        public static vector2 fromVec2(Vector2 v)
        {
            return new vector2(v.x, v.y);
        }

        public Vector2 from()
        {
            return new Vector2(x, y);
        }

    }

    public class Vector2Proxy
    {
        vector2 target;

        [MoonSharp.Interpreter.MoonSharpHidden]
        public Vector2Proxy(vector2 p)
        {
            this.target = p;
        }

        public float x => target.x;
        public float y => target.y;

    }

}
