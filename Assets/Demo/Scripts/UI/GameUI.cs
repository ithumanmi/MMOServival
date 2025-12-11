using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;

public class GameUI : Singleton<GameUI>
{

    public State state { get; private set; }
    public event Action<State, State> stateChanged;
    protected override void Awake()
    {
        base.Awake();

        state = State.Normal;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Time.timeScale = 1f;
    }

    void SetState(State newState)
    {
        if (state == newState)
        {
            return;
        }
        State oldState = state;
        if (oldState == State.Paused || oldState == State.GameOver)
        {
            Time.timeScale = 1f;
        }

        switch (newState)
        {
            case State.Normal:
                break;
            case State.Paused:
            case State.GameOver:
                Time.timeScale = 0f;
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
        state = newState;
        if (stateChanged != null)
        {
            stateChanged(oldState, state);
        }
    }
    public void GameOver()
    {
        SetState(State.GameOver);
    }
    public void Pause()
    {
        Debug.Log("Pause");
        SetState(State.Paused);
    }
    public void Unpause()
    {
        SetState(State.Normal);
    }
}
