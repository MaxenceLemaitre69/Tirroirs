using UnityEngine;

[CreateAssetMenu(fileName = "ObjectStateData", menuName = "Game/Object State Data")]
public class ObjectStateData : ScriptableObject
{
    public GameObject initialPrefab;
    public GameObject afterEventPrefab;
}