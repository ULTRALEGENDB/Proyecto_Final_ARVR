using UnityEngine;

// Heredamos de OVRCursor para que el EventSystem lo reconozca
public class GazePointer : OVRCursor
{
    public override void SetCursorRay(Transform ray) { }
    public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
    {
        transform.position = dest; // Mueve la esfera al punto donde toca el menú
    }
}