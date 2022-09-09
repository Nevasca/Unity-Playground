using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CursorLock : MonoBehaviour
{
    [SerializeField] private bool lockCursor = true;

    private void Awake()
    {
        if (!lockCursor)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#if UNITY_EDITOR
        ForceInsideEditorGameWindow();
#endif
    }

    public static void ToggleCursor(bool show)
    {
#if UNITY_EDITOR
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
#else
        Cursor.lockState = show ? CursorLockMode.Confined : CursorLockMode.Locked;
#endif
        Cursor.visible = show;
    }

#if UNITY_EDITOR
    public static void ForceInsideEditorGameWindow()
    {
        var game = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        Vector2 gameWindowCenter = game.rootVisualElement.contentRect.center;

        Event leftClickDown = new Event();
        leftClickDown.button = 0;
        leftClickDown.clickCount = 1;
        leftClickDown.type = EventType.MouseDown;
        leftClickDown.mousePosition = gameWindowCenter;

        game.SendEvent(leftClickDown);
    }
#endif
}