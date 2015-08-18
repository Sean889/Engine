#version 430

#pragma stage Vertex
#pragma name PlanetSurfaceShader


layout(location = 0) in vec3 Vertex;
layout(location = 1) in vec3 Texcoord;

layout(location = 0) uniform mat4 MVP;
layout(location = 4) uniform mat4 LightRot;
layout(location = 9) uniform float max_height;
layout(location = 11) uniform samplerCube heightmap;
layout(location = 20) uniform vec3 lightdir;

#define vert Vertex
#define texcoord Texcoord

#define mvp MVP
#define rot LightRot

out vec3 vs_lightdir;
smooth out vec3 texcoord0;

void main()
{
	vs_lightdir = normalize((rot * vec4(lightdir, 1.0)).xyz);
	gl_Position = mvp * vec4(vert + (texcoord * texture(heightmap, texcoord).x * (max_height - max_height * 0.5)), 1.0);
	texcoord0 = texcoord;
}