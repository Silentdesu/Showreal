using System;
using UnityEngine;

namespace TechnoDemo.Code.Scripts.Shaders
{
    [DisallowMultipleComponent]
    public sealed class CameraTarget : MonoBehaviour
    {
        [SerializeField] private float _speed;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.Translate(-_transform.right * (_speed * Time.deltaTime));
        }
    }
}