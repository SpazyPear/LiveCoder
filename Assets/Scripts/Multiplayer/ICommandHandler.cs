using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandHandler {
    void HandleCommand(Command command);
}
