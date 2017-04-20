using UnityEngine;

public class LineTest : MonoBehaviour
{
    public Mesh mesh;
    public Material mat;
    public void OnPostRender()
    {
        // set first shader pass of the material
        mat.SetPass(0);
        // draw mesh at the origin
        Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
    }
}