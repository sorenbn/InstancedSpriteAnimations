using UnityEngine;

public struct Entity
{
    public int id;

    public Vector2 position;
    public Vector2 scale;
    public float rotation;

    public int current_animation_index;         // index for the animation in the animation bank
    public float current_animation_time;        // normalized time (0-1) of the current animation
    public float current_animation_fps;         // fps of current animation
    public bool current_animation_loop;         // should the current animation loop or stay in the last frame
    public int current_animation_frame_index;   // current frame being displayed 
    public int current_animation_frame_total;   // total frames of the current animtion
}

public class SpriteAnimations_Mesh : MonoBehaviour
{
    public const int MAX_ENTITIES = 100;

    public Mesh mesh_quad;
    public Material sprite_material;
    public Texture2D sprite_sheet;
    public EntityAnimationDefinition[] animations;

    // Graphics.RenderMeshInstanced caps at 1023 per call.
    private Matrix4x4[] matrix_buffer = new Matrix4x4[1023];
    // custom per-instance tiling and offset data, to scroll through texture animation
    private Vector4[] uv_frame_buffer = new Vector4[1023];
    // custom per-instance size and pivot info (xy = ppu-based size, zw: sprite pivot)
    private Vector4[] quad_data_buffer = new Vector4[1023];

    private MaterialPropertyBlock material_property_block;
    private int SHADER_PROP_UV_FRAME = Shader.PropertyToID("_UVFrame");
    private int SHADER_PROP_QUAD_DATA = Shader.PropertyToID("_QuadData");
    private int SHADER_PROP_MAIN_TEX = Shader.PropertyToID("_MainTex");

    private Entity[] entities = new Entity[MAX_ENTITIES];

    void Start()
    {
        material_property_block = new MaterialPropertyBlock();

        // bake animation frame data based on their sprites, to be used by the shader (via GPU instance properties)
        for (int i = 0; i < animations.Length; i++)
        {
            var animation = animations[i];
            animation.frame_data = new SpriteAnimaionFrameData[animation.animation_frames.Length];

            for (int j = 0; j < animation.animation_frames.Length; j++)
            {
                animation.frame_data[j] = SpriteAnimaionFrameData.create_from_sprite(animation.animation_frames[j]);
            }
        }

        // setup entities
        for (int i = 0; i < entities.Length; i++)
        {
            ref var entity = ref entities[i];

            entity.id = i;
            entity.position = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
            entity.scale = Vector2.one;
            entity.rotation = 0.0f;

            set_entity_animation(ref entity, (EntityAnimationType)Random.Range(0, 5), Random.value);
        }
    }

    void Update()
    {
        update_animations();
        draw();
    }

    private void update_animations()
    {
        var dt = Time.deltaTime;

        for (int i = 0; i < entities.Length; i++)
        {
            ref var entity = ref entities[i];

            entity.current_animation_time += dt;

            var frame_time = 1.0f / entity.current_animation_fps;
            var current_frame = (int)(entity.current_animation_time / frame_time);

            if (entity.current_animation_loop)
                entity.current_animation_frame_index = current_frame % entity.current_animation_frame_total;
            else
                entity.current_animation_frame_index = Mathf.Min(current_frame, entity.current_animation_frame_total - 1);
        }
    }

    private void draw()
    {
        var draw_count = 0;

        for (int i = 0; i < entities.Length; i++)
        {
            ref var entity = ref entities[i];

            var animation = animations[entity.current_animation_index];
            var frame_data = animation.frame_data[entity.current_animation_frame_index];

            matrix_buffer[draw_count] = Matrix4x4.TRS(
                new Vector3(entity.position.x, entity.position.y, 0.0f),
                Quaternion.Euler(0.0f, 0.0f, entity.rotation),
                entity.scale);

            uv_frame_buffer[draw_count] = new Vector4(frame_data.size_uv.x, frame_data.size_uv.y, frame_data.offset.x, frame_data.offset.y);
            quad_data_buffer[draw_count] = new Vector4(frame_data.size_rect.x, frame_data.size_rect.y, frame_data.pivot.x, frame_data.pivot.y);

            draw_count++;

            if (draw_count == matrix_buffer.Length)
            {
                flush_buffer(mesh_quad, sprite_material, sprite_sheet, draw_count);
                draw_count = 0;
            }
        }

        if (draw_count > 0)
            flush_buffer(mesh_quad, sprite_material, sprite_sheet, draw_count);
    }

    private void flush_buffer(Mesh mesh, Material material, Texture texture, int count)
    {
        material_property_block.SetVectorArray(SHADER_PROP_UV_FRAME, uv_frame_buffer);
        material_property_block.SetVectorArray(SHADER_PROP_QUAD_DATA, quad_data_buffer);
        material_property_block.SetTexture(SHADER_PROP_MAIN_TEX, texture);

        var render_params = new RenderParams(material) { matProps = material_property_block };
        Graphics.RenderMeshInstanced(render_params, mesh, 0, matrix_buffer, count, 0);
    }

    private void set_entity_animation(ref Entity entity, EntityAnimationType animation_type, float start_time = 0.0f)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].animation_type == animation_type)
            {
                var animation = animations[i];

                entity.current_animation_index = i;
                entity.current_animation_fps = animation.animation_fps;
                entity.current_animation_loop = animation.loop;
                entity.current_animation_frame_total = animation.animation_frames.Length;
                entity.current_animation_time = Mathf.Clamp01(start_time);

                break;
            }
        }
    }
}
