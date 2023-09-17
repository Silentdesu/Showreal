using System;
using UnityEngine;

namespace TechnoDemo.Code.Scripts.Shaders
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class DissolveWorld : MonoBehaviour
    {
        [SerializeField] private Renderer[] _dissolveRenderers;

        private Transform _transform;
        private Material[] _materials;

        private int _materialsAmount;

        private static readonly int PlaneNormal = Shader.PropertyToID("_PlaneNormal");
        private static readonly int PlaneOrigin = Shader.PropertyToID("_PlaneOrigin");

        private void Awake()
        {
            _transform = transform;

            _materialsAmount = _dissolveRenderers.Length;
            _materials = new Material[_materialsAmount];
            for (int i = 0; i < _materialsAmount; ++i)
            {
                _materials[i] = _dissolveRenderers[i].sharedMaterial;
            }
        }

        private void Update()
        {
            for (int i = 0; i < _materialsAmount; ++i)
            {
                _materials[i].SetVector(PlaneOrigin, _transform.position);
                _materials[i].SetVector(PlaneNormal, _transform.up);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _materialsAmount; ++i)
            {
                Destroy(_materials[i]);
            }

            _transform = null;
        }
    }
}