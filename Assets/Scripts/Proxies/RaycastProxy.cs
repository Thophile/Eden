using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Proxies
{
    public class RaycastProxy
    {
        LayerMask antLayer;
        float climbDist;

        public bool climbCheck;
        public Vector3 climbNormal;

        public bool surfaceCheck;
        public Vector3 surfaceNormal;

        public bool DryRCheck;
        public Vector3 rightDir;

        public bool DryLCheck;
        public Vector3 leftDir;

        public RaycastProxy(TransformProxy transformProxy, LayerMask antLayer, float climbDist)
        {
            this.antLayer = antLayer;
            this.climbDist = climbDist;
            Refresh(transformProxy);
        }
        public void Refresh(TransformProxy transformProxy)
        {
            RaycastHit hit;
            climbCheck = Physics.Raycast(transformProxy.position - transformProxy.up * 0.02f, transformProxy.forward, out hit, climbDist, ~antLayer);
            climbNormal = hit.normal;

            RaycastHit surfaceHit;
            surfaceCheck = Physics.Raycast(transformProxy.position - transformProxy.up * 0.01f + transformProxy.forward * 0.02f, Quaternion.Euler(30, 0, 0) * -transformProxy.up, out surfaceHit, 0.06f, ~antLayer);
            surfaceNormal = surfaceHit.normal;

            RaycastHit dryHit;
            leftDir = Quaternion.Euler(0, -30, 0) * transformProxy.forward * 0.3f;
            DryLCheck = Physics.Raycast(transformProxy.position + leftDir + Vector3.up, -Vector3.up, out dryHit) && dryHit.collider.gameObject.layer == 4;

            rightDir = Quaternion.Euler(0, 30, 0) * transformProxy.forward * 0.3f;
            DryRCheck = Physics.Raycast(transformProxy.position + rightDir + Vector3.up, -Vector3.up, out dryHit) && dryHit.collider.gameObject.layer == 4;
        }
    }
}