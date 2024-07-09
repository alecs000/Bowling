using System;
using UnityEngine;


    public abstract class BankDefault : MonoBehaviour
    {
        protected ObservableVariable<int> _bankValue;
        public int Value => _bankValue.Value;
        protected virtual void Initialize()
        {
            _bankValue = new ObservableVariable<int>(0);
        }

        public virtual void Add(int value)
        {
            _bankValue.Value += value;
        }

        public virtual bool TryRemove(int value)
        {
            if (_bankValue.Value >= value)
            {
                _bankValue.Value -= value;
                return true;
            }
            return false;
        }

        public void AddObserver(Action<int> @delegate)
        {
            _bankValue.OnChanged += @delegate;
        }

        public void RemoveObserver(Action<int> @delegate)
        {
            _bankValue.OnChanged -= @delegate;
        }
    }
