using System;
using UnityEngine;

namespace Zubble
{
    public class Billboard : MonoBehaviour
    {
        private Transform _cam;
        
        private void Awake()
        {
            _cam = Camera.main.transform;
        }

        private void Update()
        {
            transform.forward = -(_cam.transform.position - transform.position);
        }
    }
}
