#shader vertex
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aColor;
layout (location = 3) in float aTexIndex; //new
out vec2 texCoord;
out vec4 color;
flat out int v_TexIndex; //new

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;


void main() 
{
	color = vec4(aColor.rgb, 1.0);
	texCoord = aTexCoord;
	v_TexIndex = int(aTexIndex); //new
	gl_Position = vec4(aPosition.xyz, 1.0) * model * view * projection;
	//gl_Position = projection * view * model * vec4(aPosition.xyz, 1.0);
}

#shader fragment
#version 330 core
out vec4 outputColor;

in vec2 texCoord;
in vec4 color;

flat in int v_TexIndex; //new

uniform sampler2D u_Texture[6];

void main() 
{
	outputColor = texture(u_Texture[v_TexIndex], texCoord) * color;
}