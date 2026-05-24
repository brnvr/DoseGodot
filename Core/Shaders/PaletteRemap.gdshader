shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
uniform sampler2D palette_texture;

uniform int palette_size = 16;

void fragment()
{
    vec3 color = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;

    float best_distance = 999999.0;
    vec3 best_color = vec3(0.0);

    for (int i = 0; i < palette_size; i++)
    {
        vec2 uv = vec2(
            (float(i) + 0.5) / float(palette_size),
            0.5
        );

        vec3 palette_color =
            texture(palette_texture, uv).rgb;

        float d = distance(color, palette_color);

        if (d < best_distance)
        {
            best_distance = d;
            best_color = palette_color;
        }
    }

    COLOR = vec4(best_color, 1.0);
}