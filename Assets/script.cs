using UnityEngine;

public class script : MonoBehaviour
{
    public float brushSize = 0.1f; // Size of the brush
    public Color brushColor = Color.red; // Color to paint with

    private Camera cam; // Reference to the camera

    void Start()
    {
        cam = Camera.main; // Get the main camera
    }

    void Update()
    {
        // Move brush with the cursor or touch input
        MoveBrushWithCursor();

        // Paint when mouse button is held down (for PC) or touch (for mobile)
        if (Input.GetMouseButton(0))
        {
            PaintObject();
        }
    }

    void MoveBrushWithCursor()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Move the brush to the hit point
            transform.position = hit.point;
        }
    }

    void PaintObject()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Renderer renderer = hit.transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                if (material != null && material.mainTexture is Texture2D texture)
                {
                    // Ensure texture is readable
                    if (!texture.isReadable)
                    {
                        Debug.LogWarning("Texture is not readable. Enable 'Read/Write Enabled' in the texture import settings.");
                        return;
                    }

                    // Convert hit texture coordinate to pixel coordinates
                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= texture.width;
                    pixelUV.y *= texture.height;

                    // Calculate brush size in pixels
                    int brushSizePixels = Mathf.RoundToInt(brushSize);

                    // Calculate starting pixel position for painting
                    int startX = Mathf.RoundToInt(pixelUV.x - brushSizePixels / 2);
                    int startY = Mathf.RoundToInt(pixelUV.y - brushSizePixels / 2);

                    // Clamp to ensure within texture bounds
                    startX = Mathf.Clamp(startX, 0, texture.width - brushSizePixels);
                    startY = Mathf.Clamp(startY, 0, texture.height - brushSizePixels);

                    // Example: Paint a block of pixels around the hit point with brushColor
                    Color[] colors = new Color[brushSizePixels * brushSizePixels];
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = brushColor;
                    }

                    // Paint pixels around the hit point
                    texture.SetPixels(startX, startY, brushSizePixels, brushSizePixels, colors);
                    texture.Apply(); // Apply changes to the texture
                }
                else
                {
                    Debug.LogWarning("Texture2D not found on hit object's material.");
                }
            }
            else
            {
                Debug.LogWarning("Renderer not found on hit object.");
            }
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any object.");
        }
    }




}
