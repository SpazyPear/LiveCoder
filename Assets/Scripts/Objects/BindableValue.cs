using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class BindableValue<T>
{

    private T Value;
    public T value { get { return Value; } set { Value = value; updater(value); } } 
    Action<T> updater;

    public BindableValue(Action<T> uiUpdater)
    {
        this.updater = uiUpdater;
    }

}
