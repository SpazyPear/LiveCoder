using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CodeEditorIntellisense : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField]
    private TextMeshProUGUI inputField;
    [SerializeField]
    private TMP_InputField _inputField;

    public RectTransform rect;


    private void Update()
    {
        //rect.position = (GetLocalCaretPosition());
        print(GetLocalCaretPosition());
        rect.localPosition = GetLocalCaretPosition();
    }


    public Vector2 GetLocalCaretPosition()
    {
        Rect r = new Rect();
        r.yMin = inputField.textInfo.characterInfo[_inputField.caretPosition].topLeft.y;
        r.xMin = inputField.textInfo.characterInfo[_inputField.caretPosition].topLeft.x;
        r.yMax = inputField.textInfo.characterInfo[_inputField.caretPosition].bottomRight.y;
        r.xMax = inputField.textInfo.characterInfo[_inputField.caretPosition].bottomRight.x;

        return new Vector2(r.center.x, r.center.y);
    }
}
