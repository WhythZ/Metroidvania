using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }
    //存储这个状态机当下展示的动作状态是什么；{ get; private set; }表示这个变量对外部是只可读，不可更改的

    public void Initialize(EnemyState _startState)
    {
        //设定这个状态机的初始状态，并进入该状态
        this.currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState _newState)
    {
        //退出上一个状态，并设置当前状态为输入的状态，然后进入该状态
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
