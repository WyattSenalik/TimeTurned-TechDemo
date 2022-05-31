using UnityEngine;
// Original Authors - Wyatt Senalik

/// <summary>
/// Very similar to <see cref="SingletonMonoBehaviourPersistant{T}"/> but
/// will dynamically create itself if there is not one in the scene already.
/// </summary>
[RequireComponent(typeof(DontDestroyOnLoad))]
public class DynamicSingletonMonoBehaviourPersistant<T> : 
    DynamicSingletonMonoBehaviour<T> where T : 
    DynamicSingletonMonoBehaviourPersistant<T>
{ }
