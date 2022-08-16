using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraAlternativeMatrixInjector : MonoBehaviour
{
    private const string PROJECTION_MATRIX_PROPERTY = "_AltProjectionMatrix";
    private Camera cam;
    [SerializeField] private float FOVOffset = 45;

    private int projectionMatrixPropertyId;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        projectionMatrixPropertyId = Shader.PropertyToID(PROJECTION_MATRIX_PROPERTY);
    }

    private void Update()
    {
        float previousFov = cam.fieldOfView;
        cam.fieldOfView =cam.fieldOfView + FOVOffset;
        Matrix4x4 projectionMatrix = cam.projectionMatrix;
        projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix,false);
        projectionMatrix.m11 *= -1;
        Shader.SetGlobalMatrix(projectionMatrixPropertyId, projectionMatrix);
        cam.fieldOfView = previousFov;
    }
}
