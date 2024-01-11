using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace _Project.Code.Scripts.Utils
{
    [DisallowMultipleComponent]
    public sealed class NameGiverBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform _handler;

        [Button]
        private void GetShaderName()
        {
            var textMeshPro = transform.GetComponentInChildren<TextMeshPro>();
            if (textMeshPro == null) return;
            
            var material = _handler.GetComponentInChildren<MeshRenderer>().sharedMaterial;

            if (material == null)
                throw new Exception("Material is null!");

            var shaderName = material.name.Split('_')[1];
            transform.name = $"{shaderName}_Shader";
            textMeshPro.SetText(shaderName);
        }
    }
}