#version 330

#pragma name PlanetSurfaceShader
#pragma stage Vertex

in vec3 Vertex;
in vec3 Texcoord;

uniform float MaxDeform;
uniform mat4 MVP;
uniform mat4 LightRotation;
uniform samplerCube BumpMap;

smooth out vec3 Texcoord0;

void main()
{
	Texcoord0 = Texcoord;
	gl_Position = MVP * vec4(Vertex + (Texcoord * texture(BumoMap, Texcoord).x * (MaxDeform - MaxDeform * 0.5)), 1.0);
}