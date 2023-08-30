using System;
using Cinemachine.Editor;
using TechnoDemo.Player;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.UIElements;

namespace TechnoDemo.Editor.Inspector
{
    [CustomEditor(typeof(PlayerSettingsSO))]
    public sealed class PrettyInspectorDrawerEditor : UnityEditor.Editor
    {
        private VisualElement m_visualElement;
        
        private SerializedProperty m_sprintSpeed;
        private SerializedProperty m_regularSpeed;
        private SerializedProperty m_rotationSmoothTime;
        private SerializedProperty m_gravity;
        private SerializedProperty m_groundOffset;
        private SerializedProperty m_groundSphereCastRadius;
        private SerializedProperty m_groundLayer;
        private SerializedProperty m_jumpHeight;
        private SerializedProperty m_fallTimeout;

        private void OnEnable()
        {
            m_sprintSpeed = serializedObject.FindProperty(nameof(m_sprintSpeed));
            m_regularSpeed = serializedObject.FindProperty(nameof(m_regularSpeed));
            m_rotationSmoothTime = serializedObject.FindProperty(nameof(m_rotationSmoothTime));
            m_gravity = serializedObject.FindProperty(nameof(m_gravity));
            m_groundOffset = serializedObject.FindProperty(nameof(m_groundOffset));
            m_groundSphereCastRadius = serializedObject.FindProperty(nameof(m_groundSphereCastRadius));
            m_groundLayer = serializedObject.FindProperty(nameof(m_groundLayer));
            m_jumpHeight = serializedObject.FindProperty(nameof(m_jumpHeight));
            m_fallTimeout = serializedObject.FindProperty(nameof(m_fallTimeout));
        }

        public override VisualElement CreateInspectorGUI()
        {
            m_visualElement = new VisualElement();
            
            return base.CreateInspectorGUI();
;
        }
    }
}