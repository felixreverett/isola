#shader vertex
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 acolor;
layout (location = 3) in float aTexIndex;

out vec2 texCoord;
flat out int v_TexIndex;

void main()
{
	texCoord = aTexCoord;
	v_TexIndex = int(aTexIndex);
	gl_Position = vec4(aPosition.xyz, 1.0);
}

#shader fragment
#version 330 core
out vec4 outputColor;

in vec2 texCoord;
flat in int v_TexIndex;

uniform sampler2D u_Texture[6];
uniform vec4 u_TextColor;

void main()
{
	outputColor = texture(u_Texture[v_TexIndex], texCoord) * u_TextColor;
}