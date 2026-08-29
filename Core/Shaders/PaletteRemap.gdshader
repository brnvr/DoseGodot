shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
uniform sampler2D palette_texture;

uniform int palette_size = 16;
uniform float dither_size : hint_range(1.0, 8.0, 1.0) = 1.0;

// A 4x4 Bayer matrix gives each pixel a stable threshold. Comparing the
// desired blend against this threshold spatially mixes two palette colors
// without ever introducing colors that are not in the palette.
float bayer_threshold(ivec2 pixel)
{
    int x = pixel.x % 4;
    int y = pixel.y % 4;

    if (y == 0)
    {
        if (x == 0) return 0.5 / 16.0;
        if (x == 1) return 8.5 / 16.0;
        if (x == 2) return 2.5 / 16.0;
        return 10.5 / 16.0;
    }
    if (y == 1)
    {
        if (x == 0) return 12.5 / 16.0;
        if (x == 1) return 4.5 / 16.0;
        if (x == 2) return 14.5 / 16.0;
        return 6.5 / 16.0;
    }
    if (y == 2)
    {
        if (x == 0) return 3.5 / 16.0;
        if (x == 1) return 11.5 / 16.0;
        if (x == 2) return 1.5 / 16.0;
        return 9.5 / 16.0;
    }

    if (x == 0) return 15.5 / 16.0;
    if (x == 1) return 7.5 / 16.0;
    if (x == 2) return 13.5 / 16.0;
    return 5.5 / 16.0;
}

void fragment()
{
    vec3 color = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;

    float best_distance = 999999.0;
    float second_distance = 999999.0;
    vec3 best_color = vec3(0.0);
    vec3 second_color = vec3(0.0);

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
            second_distance = best_distance;
            second_color = best_color;
            best_distance = d;
            best_color = palette_color;
        }
        else if (d < second_distance)
        {
            second_distance = d;
            second_color = palette_color;
        }
    }

    vec3 output_color = best_color;

    if (palette_size > 1)
    {
        vec3 palette_span = second_color - best_color;
        float blend = clamp(
            dot(color - best_color, palette_span) /
                max(dot(palette_span, palette_span), 0.000001),
            0.0,
            1.0
        );

        ivec2 dither_pixel = ivec2(floor(
            SCREEN_UV / SCREEN_PIXEL_SIZE / max(dither_size, 1.0)
        ));

        if (blend > bayer_threshold(dither_pixel))
        {
            output_color = second_color;
        }
    }

    COLOR = vec4(output_color, 1.0);
}
