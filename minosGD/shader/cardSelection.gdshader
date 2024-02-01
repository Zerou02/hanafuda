shader_type canvas_item;
uniform sampler2D blendTex;

void vertex() {
}

void fragment() {
	vec4 blendVal = texture(blendTex,UV);
	vec4 texPx = texture(TEXTURE,UV - TIME * 0.05);
	float width = TEXTURE_PIXEL_SIZE.x;
	float magnitude = length(blendVal.rgb);
	
	COLOR = texPx*magnitude;
}
