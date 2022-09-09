using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private List<BaseState> _states;
        [SerializeField] private AIState initialState;

        private Dictionary<AIState, BaseState> _availableStates;

        public AIState CurrentState { get; private set; }
        public event Action<BaseState> OnStateChanged;

        private void Awake()
        {
            //Dictionary<AIState, BaseState> aux = _availableStates;
            _availableStates = new Dictionary<AIState, BaseState>();
            foreach (var state in _states)
                _availableStates.Add(state.GetStateType(), state.CreateInstance(gameObject));

            CurrentState = initialState;

            _availableStates[CurrentState].Start();
        }

        private void Update()
        {
            var nextState = _availableStates[CurrentState].Tick();

            if (nextState != CurrentState)
                SwitchToNewState(nextState);
        }

        public bool SwitchToNewState(AIState nextState)
        {
            bool couldSwitch = true;

            if (nextState == AIState.Default)
                nextState = initialState;

            if(!_availableStates.ContainsKey(nextState))
            {
                Debug.LogWarning($"{gameObject.name} has no {nextState} state. Switching to default {initialState}");
                couldSwitch = false;
                nextState = initialState;                
            }

            _availableStates[CurrentState].Stop();
            CurrentState = nextState;
            _availableStates[CurrentState].Start();

            OnStateChanged?.Invoke(_availableStates[nextState]);
            return couldSwitch;
        }

        public BaseState GetState(AIState state)
        {
            if (_availableStates.ContainsKey(state))
                return _availableStates[state];

            return null;
        }
    }
}