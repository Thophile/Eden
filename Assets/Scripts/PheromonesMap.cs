using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerType{
    Wander,
    Resource,
    Repel
}

[System.Serializable]
public class Marker{

    public Marker(Vector3 position, MarkerType type){
        this.Position = position;
        switch (type){
            case MarkerType.Wander:
                wander = 1;
                break;
            case MarkerType.Resource:
                resource=1;
                break;
            case MarkerType.Repel:
                repel=1;
                break;
        }
    }
    public Vector3 Values{
        get{
            return new Vector3(wander,resource,repel);
        }
        set{
            wander = value.x;
            resource = value.y;
            repel = value.z;
        }
    }
    public float wander;
    public float resource;
    public float repel;

    public Vector3 Position{
        get{
            return new Vector3(x,y,z);
        }
        set{
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
    public float x;
    public float y;
    public float z;

}

[System.Serializable]
public class PheromonesMap
{
    static float threshold = 0.1f;
    static float decayFactor = 0.9f;
    public List<Marker> markers = new List<Marker>();

    public Vector3 ComputeZone(Vector3 pos, int radius=2){
        Vector3 values = new Vector3(0,0,0);
        int valueCount = 0;

        foreach (var marker in markers)
        {
            if (marker == null) continue;
            if(marker.x > pos.x + radius) continue;
            if(marker.y > pos.y + radius) continue;
            if(marker.z > pos.z + radius) continue;

            if((marker.Position - pos).magnitude <= radius){
                values += marker.Values;
                valueCount++;
            }
        }

        return valueCount == 0 ? values: values/valueCount;
    }
    public void decayMarkers(){
        List<Marker> expiredElements = new List<Marker>();
        foreach (Marker marker in markers){
            if (marker.Values.magnitude > threshold){
                marker.Values *= decayFactor;
            }else{
                expiredElements.Add(marker);
            }
        }
        RemoveMarkers(expiredElements);
    }
     public void RemoveMarkers(List<Marker> expiredElements){
        foreach (var item in expiredElements)
        {
            markers.Remove(item); 
        }
     }

    public void Mark(Vector3 position, MarkerType type){
        markers.Add(new Marker(position, type));
    }
}
