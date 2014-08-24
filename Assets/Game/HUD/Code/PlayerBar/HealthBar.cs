using UnityEngine;
using System.Collections;

public class HealthBar : HUDBase 
{
    public Texture2D BackgroundTexture;
    public Material BackgroundMaterial;

    public Texture2D SegmentTexture;
    public Material SegmentMaterial;

    public Vector3 PositionOffset;   // Position relative to the playerbar gameobject

    public void Redraw(float health)
    {
        Vector3 position = Camera.WorldToViewportPoint(PlayerBar.playerBarTransform.position + PositionOffset);
        GUI.DrawTexture(new Rect(position.x, position.y, BackgroundTexture.width, BackgroundTexture.height), BackgroundTexture);

        for (int i = 0; i < (int)health; i++)
        {
            GUI.DrawTexture(new Rect(position.x + i * SegmentTexture.width, position.y, SegmentTexture.width, SegmentTexture.height), SegmentTexture);
        }
    }
}
