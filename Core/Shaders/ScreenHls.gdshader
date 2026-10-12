shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

// Independent channel multipliers provide decomposed RGB hue control.
uniform vec3 hue_rgb = vec3(1.0);
uniform float lightness : hint_range(-1.0, 1.0) = 0.0;
uniform float saturation : hint_range(0.0, 2.0) = 1.0;

void fragment()
{
    vec4 screen_color = texture(SCREEN_TEXTURE, SCREEN_UV);
    vec3 color = screen_color.rgb * hue_rgb;
    float luminance = dot(color, vec3(0.299, 0.587, 0.114));

    color = mix(vec3(luminance), color, saturation);
    color = clamp(color + vec3(lightness), vec3(0.0), vec3(1.0));

    COLOR = vec4(color, screen_color.a);
}
