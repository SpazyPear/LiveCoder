using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct ErrorSource
{
    public string playerId;
    public string function;
}

public class Error
{
    public string errorMessage;
    public int level; // Level of Error

    public Error(string errorMessage, int level = 0)
    {
        this.errorMessage = errorMessage;
        this.level = level;

    }
}



public class ErrorManager : MonoBehaviour
{

    public UnityEvent<ErrorSource, Error> handleError;

    public static ErrorManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);

        ErrorManager.instance.PushError(new ErrorSource { function = "test", playerId = "testId" }, new Error("test error message"));
    }

    public void TestErrorHandling(ErrorSource source, Error error)
    {
        Debug.LogWarning($"Game Error => <b>{source.playerId}:{source.function}</b> : {error.errorMessage}");
    }


    public void PushError(ErrorSource source, Error error)
    {
        handleError.Invoke(source, error);
    }

}
