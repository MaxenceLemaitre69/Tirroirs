using UnityEngine;

[CreateAssetMenu(fileName = "ObjectStateData", menuName = "Game/Object State Data")]
public class ObjectStateData : ScriptableObject
{
    public GameObject initialPrefab;
    public GameObject afterEventPrefab;

    [Header("Notes du carnet")]
    [TextArea(5, 12)] public string noteState1;  // état initial
    [TextArea(5, 12)] public string noteState2;  // après event
}
