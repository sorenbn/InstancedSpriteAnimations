using UnityEngine;

public enum EntityAnimationType
{
    SAMURAI_IDLE,
    SAMURAI_RUN,
    SAMURAI_ATTACK,
    SAMURAI_PARRY,
    SAMURAI_DIE,
}

[System.Serializable]
public class EntityAnimationDefinition
{
    public string name;
    public EntityAnimationType animation_type;
    public int animation_fps;
    public bool loop = true;
    public Sprite[] animation_frames;

    // will be 'baked' during game initialization
    [HideInInspector]
    public SpriteAnimaionFrameData[] frame_data;
}

public struct SpriteAnimaionFrameData
{
    public Vector2 size_uv;
    public Vector2 offset;
    public Vector2 size_rect;
    public Vector2 pivot;

    // method for 'baking' frame data for the shader to use (instanced rendering)
    public static SpriteAnimaionFrameData create_from_sprite(Sprite sprite)
    {
        var texture = sprite.texture;
        var texel_size = texture.texelSize;
        var texture_rect = sprite.textureRect;
        var sprite_rect = sprite.rect;
        var ppu = sprite.pixelsPerUnit;
        var pivot = sprite.pivot;
        var pivot_offset = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);

        return new SpriteAnimaionFrameData
        {
            size_uv = texture_rect.size * texture.texelSize,                    // scale/size of the rectangle
            offset = texture_rect.position * texture.texelSize,                 // where on the texture should it sample from
            size_rect = sprite.bounds.size,                                     // size for the quad
            pivot = new Vector2(0.5f - pivot_offset.x, 0.5f - pivot_offset.y),  // quad-offset based on the sprite's pivot
        };
    }
}
