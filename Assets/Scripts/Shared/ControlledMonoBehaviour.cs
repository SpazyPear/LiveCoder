using UnityEngine;

public class ControlledMonoBehavour : MonoBehaviour
{

    /// When Player Code Execution begins
    public virtual void OnStart() { }

    // On the next step of Player Code execution
    public virtual void OnStep() { }

    public virtual void OnPostStep() { }

}