using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PheromonesMap
{
    static float threshold = 0.2f;
    static float decayFactor = 1.1f;
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
                marker.Decay(decayFactor);
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
