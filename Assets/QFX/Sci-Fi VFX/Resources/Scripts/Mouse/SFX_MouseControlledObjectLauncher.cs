using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable once CheckNamespace
namespace QFX.SFX
{
    public class SFX_MouseControlledObjectLauncher : MonoBehaviour
    {
        public SFX_ControlledObject[] ControlledObjects;
        public int MouseButtonCode;
        public bool CallStop = true;

        private void Update()
        {
            if (Input.GetMouseButtonDown(MouseButtonCode))
            {
                foreach (var controlledObject in ControlledObjects)
                {
                    controlledObject.Setup();
                    controlledObject.Run();
                }
            }
            else if (CallStop && Input.GetMouseButtonUp(MouseButtonCode))
            {
                foreach (var controlledObject in ControlledObjects)
                    controlledObject.Stop();
            }
        }
    }
}