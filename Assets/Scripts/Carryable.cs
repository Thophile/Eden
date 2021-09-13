using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarryableType{
    Food,
    Material
}
public class Carryable : MonoBehaviour
{
    public string prefabName;
    public int value;
    public CarryableType type;

}
