using UnityEngine;

/// <summary>
/// Attach this to an empty GameObject and wire your LEFT/RIGHT
/// buttons' OnClick() to these two methods. Fill in what each
/// path should actually do once you know (load a scene, trigger
/// a Timeline, set a story flag, etc.) — these are placeholders
/// so you can test the full chain end-to-end right now.
/// </summary>
public class ChoiceManager : MonoBehaviour
{
    public void ChooseLeft()
    {
        Debug.Log("Player chose LEFT");
        // TODO: what happens on the LEFT path?
    }

    public void ChooseRight()
    {
        Debug.Log("Player chose RIGHT");
        // TODO: what happens on the RIGHT path?
    }
}