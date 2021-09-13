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

    public Marker(MarkerType type){
        switch (type){
            case MarkerType.Wander:
                x = 1;
                break;
            case MarkerType.Resource:
                y=1;
                break;
            case MarkerType.Repel:
                z=1;
                break;
        }
    }
    public Vector3 Values{
        get{
            return new Vector3(x,y,z);
        }
        set{
            x= value.x;
            y= value.y;
            z= value.z;
        }
    }
    public float x;
    public float y;
    public float z;

}

[System.Serializable]
public class PheromonesMap
{
    static int mapSize = 500;
    static int offset = 250;
    static float threshold = 0.1f;
    static float decayFactor = 0.9f;
    public Marker[,,] map = new Marker[mapSize,mapSize,mapSize];
    public List<Marker> markers = new List<Marker>();

    public Vector3Int SnapPoint(Vector3 pos){
        var point = Vector3Int.FloorToInt(pos) + new Vector3Int(offset,offset,offset);
        return point;
    }

    public Vector3 ComputeZone(Vector3 pos, int radius=2){
        Vector3Int point = SnapPoint(pos);
        Vector3 values = new Vector3(0,0,0);
        int valueCount = 0;
        for(int i=0; i <= radius*2; i++){
            var iX = point.x + i-radius; // point-2 -> point+2
            if(iX <= 0 && iX >=-500){
                for(int j=0; j <= radius*2; j++){
                    var iY = point.y + j-radius; // point-2 -> point+2
                    if(iY <= 0 && iY >=-500){
                        for(int k=0; k <= radius*2; k++){
                            var iZ = point.z + k-radius; // point-2 -> point+2
                            if(iZ <= 0 && iZ >=-500){
                                if(map[iX,iY,iZ] != null){
                                    values += map[offset + iX, offset + iY, offset + iZ].Values;
                                    valueCount ++;
                                }
                            }
                        }
                    }
                }
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
        lock(markers){
            foreach (var item in expiredElements)
            {
                markers.Remove(item); 
            }
        }
     }

    public void Mark(Vector3 position, MarkerType type){
        var marker = new Marker(type);
        var point = SnapPoint(position);
        if(map[point.x, point.y, point.z] == null){
            map[point.x, point.y, point.z] = marker;
            markers.Add(marker);
        }else{
            map[point.x, point.y, point.z].Values += marker.Values;
        }
    }
}
