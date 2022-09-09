using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestAnimatedText : MonoBehaviour
{
    [TextArea] public string testText;

    private void Awake()
    {
        TestText();
    }

    [ContextMenu("Test Text")]
    private void TestText()
    {
        GetComponent<TMP_Animated>().ReadText(testText);
    }
}
