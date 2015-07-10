#version 330

#pragma name PlanetSurfaceShader
#pragma stage Fragment

in vec3 Texcoord0;

uniform vec3 RelLightDir;

uniform samplerCube ColourTexture;
uniform samplerCube NormalTexture;

out vec3 Colour;

void main()
{
	vec3 normal = texture(NormalTexture, Texcoord0).rgb;
	
	vec3 tangent;
	if(normal.x > normal.y && normal.x > normal.z)
		tangent = sign(normal.x) * normalize(cross(normal, cross(vec3(0.0, 0.0, 1.0), normal)));
	else if(normal.y > normal.z && normal.y > normal.x)
		tangent = normalize(cross(normal, cross(vec3(1.0, 0.0, 0.0), normal)));
	else
		tangent = sign(normal.z) * normalize(cross(normal, cross(vec3(-1.0, 0.0, 0.0), normal)));
		
	vec3 bitangent = cross(tangent, normal);
	
	mat3 TBN = mat3(tangent, bitangent, Texcoord0);
	
	Colour = texture(colour_texture, texcoord0).rgb * max(dot(light_dir, TBN * normal), 0.1);
}