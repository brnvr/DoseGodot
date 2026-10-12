shader_type canvas_item;

// Video codecs rarely preserve a perfectly black background. The threshold
// removes those near-black values, while softness keeps the keyed edge smooth.
uniform float opacity : hint_range(0.0, 1.0) = 1.0;
uniform bool merge_black = false;
uniform float black_threshold : hint_range(0.0, 1.0) = 0.05;
uniform float black_softness : hint_range(0.001, 1.0) = 0.2;

void fragment()
{
    vec4 video_color = texture(TEXTURE, UV);
    float brightness = max(
        max(video_color.r, video_color.g),
        video_color.b
    );
    float keyed_alpha = 1.0;
    if (merge_black)
    {
        keyed_alpha = smoothstep(
            black_threshold,
            black_threshold + black_softness,
            brightness
        );
    }

    COLOR = vec4(
        video_color.rgb,
        video_color.a * keyed_alpha * opacity
    );
}
