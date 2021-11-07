using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class PheromonesMap
    {
        static readonly float threshold = 10f;
        static readonly float decayFactor = 1.05f;
        public List<Marker> markers = new List<Marker>();

        public float ComputeZone(Vector3 pos, int radius=2){
            float intensity = 0f;

            foreach (var marker in markers)
            {
                if (marker == null) continue;
                if(marker.x > pos.x + radius || marker.x < pos.x - radius) continue;
                if(marker.y > pos.y + radius || marker.y < pos.y - radius) continue;
                if(marker.z > pos.z + radius || marker.z < pos.z - radius) continue;

                if((marker.Position - pos).magnitude <= radius){
                    intensity += marker.intensity;
                }
            }

            return intensity;
        }
        public void DecayMarkers(){
            List<Marker> expiredElements = new List<Marker>();
            foreach (Marker marker in markers){
                if (marker.intensity > threshold){
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

        public void Mark(Vector3 position){
            markers.Add(new Marker(position));
        }
    }
}
