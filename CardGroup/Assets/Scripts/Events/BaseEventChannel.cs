using UnityEngine;
using System;

public abstract class BaseEventChannel<T> : ScriptableObject
{
    private Action<T> _listeners;

    public void Raise(T value) => _listeners?.Invoke(value);

    public void Subscribe(Action<T> listener) => _listeners += listener;

    public void Unsubscribe(Action<T> listener) => _listeners -= listener;
}