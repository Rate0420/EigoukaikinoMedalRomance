using System;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    public bool isPaused;

    public event Action OnPausedChange;

    public void ChangePause(bool isPause)
    { 
        isPaused = isPause;
        OnPausedChange.Invoke();
    }
}
