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
