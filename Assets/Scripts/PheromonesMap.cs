using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PheromonesMarker{


    public readonly float lifeTime = 30f;
    public Vector3 Position{
        get{
            return new Vector3(x,y,z);
        }
        set{
            x= value.x;
            y=value.y;
            z=value.z;
        }
    }
    public float x;
    public float y;
    public float z;
    public readonly float timeStamp;
    public PheromonesMarker(Vector3 position){
        Position = position;
        
        timeStamp = Time.time;
    }

}

[System.Serializable]
public class PheromonesMap
{
    public List<PheromonesMarker> markers = new List<PheromonesMarker>(); 
    public float getPheromonesValue( Vector3 position, float range){
        var closeElements = new List<PheromonesMarker>();
        var expiredElements = new List<PheromonesMarker>();
        var now = Time.time;
        lock (markers)
        {
            foreach (var item in markers)
            {
                // Expired check
                if( now - item.timeStamp > item.lifeTime){
                    expiredElements.Add(item);
                }else if( (position - item.Position).sqrMagnitude <= range*range){
                    // marker is in range
                    closeElements.Add(item);
                }
            }
        }
        RemoveMarkers(expiredElements);

        float str = 0;
        foreach (var item in closeElements)
        {
            var dist = (position - item.Position).sqrMagnitude;
            var decayRatio = (item.lifeTime - (now - item.timeStamp)) / item.lifeTime;
            str+= decayRatio / (dist + 1f);
        }
        return str;
    }
    public void RemoveMarkers(List<PheromonesMarker> expiredElements){
        lock(markers){
            foreach (var item in expiredElements)
            {
                markers.Remove(item); 
            }
        }
    }
    public void Mark(Vector3 position){
        markers.Add(new PheromonesMarker(position));
    }
}
