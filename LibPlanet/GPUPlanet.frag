#version 430

#pragma stage Fragment

highp float rand(vec2 co)
{
    highp float a = 12.9898;
    highp float b = 78.233;
    highp float c = 43758.5453;
    highp float dt= dot(co.xy ,vec2(a,b));
    highp float sn= mod(dt,3.14);
    return fract(sin(sn) * c);
}

highp float rand(vec3 co)
{
	return vec3(rand(co.xy), rand(co.yz), rand(co.zx));
}

in vec3 texcoord0;

layout (location = 10) uniform samplerCube colour_texture;
layout (location = 12) uniform samplerCube normal_texture;
layout (location = 15) uniform vec3 light_dir;

out vec3 colour;

void main()
{
	vec3 normal = texture(normal_texture, texcoord0).rgb;
	
	vec3 tangent;
	if(normal.x > normal.y && normal.x > normal.z)
		tangent = sign(normal.x) * normalize(cross(normal, cross(vec3(0.0, 0.0, 1.0), normal)));
	else if(normal.y > normal.z && normal.y > normal.x)
		tangent = normalize(cross(normal, cross(vec3(1.0, 0.0, 0.0), normal)));
	else
		tangent = sign(normal.z) * normalize(cross(normal, cross(vec3(-1.0, 0.0, 0.0), normal)));
		
	vec3 bitangent = cross(tangent, normal);
	
	mat3 TBN = mat3(tangent, bitangent, texcoord0);
	
	colour = (texture(colour_texture, texcoord0).rgb + rand(texcoord0 * 10000) * 0.0625) * max(dot(light_dir, TBN * normal), 0.1);
};