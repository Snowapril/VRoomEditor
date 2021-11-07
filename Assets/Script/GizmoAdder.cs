using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGLTF;

namespace RTG
{
    public class GizmoAdder : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            ObjectTransformGizmo moveGizmo = RTGizmosEngine.Get.CreateObjectMoveGizmo();
            GameObject targetObject = GameObject.Find("Sphere");
            moveGizmo.SetTargetObject(targetObject);
        }

        // Update is called once per frame
        void Update()
        {
            ObjectTransformGizmo moveGizmo = RTGizmosEngine.Get.CreateObjectMoveGizmo();
            GameObject targetObject = GameObject.Find("GLTFScene");
            if (targetObject)
            { 
               moveGizmo.SetTargetObject(targetObject);
            }
        }
    }
}