using System;
using UnityEngine;

namespace TechnoDemo.Code.Scripts.Shaders
{
    [DisallowMultipleComponent]
    public sealed class ProceduralGrass : MonoBehaviour
    {
        [SerializeField] private ComputeShader _shader;

        [SerializeField] private Mesh _terrainMesh;
        [SerializeField] private Mesh _grassMesh;
        [SerializeField] private Material _material;

        [SerializeField] private float _scale = 0.1f;
        [SerializeField] private Vector2 _minMaxBladeHeight = new Vector2(0.5f, 1.5f);

        private GraphicsBuffer _terrainTriangleBuffer;
        private GraphicsBuffer _terrainVertexBuffer;
        private GraphicsBuffer _transformMatrixBuffer;

        private GraphicsBuffer _grassTriangleBuffer;
        private GraphicsBuffer _grassVertexBuffer;
        private GraphicsBuffer _grassUVBuffer;

        private Bounds _bounds;

        private int _kernel;
        private uint _threadGroupSize;
        private int _terrainTriangleCount = 0;
        private static readonly int TerrainPositions = Shader.PropertyToID("_TerrainPositions");
        private static readonly int TerrainTriangles = Shader.PropertyToID("_TerrainTriangles");
        private static readonly int TransformMatrices = Shader.PropertyToID("_TransformMatrices");
        private static readonly int TerrainObjectToWorld = Shader.PropertyToID("_TerrainObjectToWorld");
        private static readonly int TerrainTriangleCount = Shader.PropertyToID("_TerrainTriangleCount");
        private static readonly int MinMaxBladeHeight = Shader.PropertyToID("_MinMaxBladeHeight");
        private static readonly int Scale = Shader.PropertyToID("_Scale");
        private static readonly int Positions = Shader.PropertyToID("_Positions");
        private static readonly int UVs = Shader.PropertyToID("_UVs");

        private void Start()
        {
            _kernel = _shader.FindKernel("TerrainOffsets");

            _terrainMesh = GetComponent<MeshFilter>().sharedMesh;

            var terrainVertices = _terrainMesh.vertices;
            _terrainVertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainVertices.Length,
                sizeof(float) * 3);
            _terrainVertexBuffer.SetData(terrainVertices);
            _shader.SetBuffer(_kernel, TerrainPositions, _terrainVertexBuffer);

            var terrainTriangles = _terrainMesh.triangles;
            _terrainTriangleBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainTriangles.Length, sizeof(int));
            _terrainTriangleBuffer.SetData(terrainTriangles);
            _shader.SetBuffer(_kernel, TerrainTriangles, _terrainTriangleBuffer);

            _terrainTriangleCount = terrainTriangles.Length / 3;

            var grassVertices = _grassMesh.vertices;
            _grassVertexBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassVertices.Length, sizeof(float) * 3);
            _grassVertexBuffer.SetData(grassVertices);

            var grassTriangles = _grassMesh.triangles;
            _grassTriangleBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassTriangles.Length, sizeof(int));
            _grassTriangleBuffer.SetData(grassTriangles);

            var grassUVs = _grassMesh.uv;
            _grassUVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassUVs.Length, sizeof(float) * 2);
            _grassUVBuffer.SetData(grassUVs);

            _transformMatrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _terrainTriangleCount,
                sizeof(float) * 16);
            _shader.SetBuffer(_kernel, TransformMatrices, _transformMatrixBuffer);

            _bounds = _terrainMesh.bounds;
            _bounds.center += transform.position;
            _bounds.Expand(_minMaxBladeHeight.y);

            RunComputeShader();
        }

        private void RunComputeShader()
        {
            _shader.SetMatrix(TerrainObjectToWorld, transform.localToWorldMatrix);
            _shader.SetInt(TerrainTriangleCount, _terrainTriangleCount);
            _shader.SetVector(MinMaxBladeHeight, _minMaxBladeHeight);
            _shader.SetFloat(Scale, _scale);

            _shader.GetKernelThreadGroupSizes(_kernel, out _threadGroupSize, out _, out _);
            var threadGroups = Mathf.CeilToInt(_terrainTriangleCount / _threadGroupSize);
            _shader.Dispatch(_kernel, threadGroups, 1, 1);
        }

        private void Update()
        {
            var rp = new RenderParams(_material);
            rp.worldBounds = _bounds;
            rp.matProps = new MaterialPropertyBlock();
            rp.matProps.SetBuffer(TransformMatrices, _transformMatrixBuffer);
            rp.matProps.SetBuffer(Positions, _grassVertexBuffer);
            rp.matProps.SetBuffer(UVs, _grassUVBuffer);

            Graphics.RenderPrimitivesIndexed(rp, MeshTopology.Triangles, _grassTriangleBuffer,
                _grassTriangleBuffer.count, instanceCount: _terrainTriangleCount);
        }

        private void OnDestroy()
        {
            _terrainTriangleBuffer.Dispose();
            _terrainVertexBuffer.Dispose();
            _transformMatrixBuffer.Dispose();
            
            _grassTriangleBuffer.Dispose();
            _grassVertexBuffer.Dispose();
            _grassUVBuffer.Dispose();
        }
    }
}