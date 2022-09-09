using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    public abstract class BaseState : ScriptableObject
    {
        protected GameObject gameObject;

        public BaseState CreateInstance(GameObject gameObject)
        {
            BaseState instance = Instantiate(this);
            instance.gameObject = gameObject;
            instance.Init();

            return instance;
        }

        public abstract AIState GetStateType();
        public abstract void Init();
        public abstract void Start();
        public abstract AIState Tick();
        public abstract void Stop();
    }
}