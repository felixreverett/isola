using FeloxGame.Core.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FeloxGame.Core.Rendering
{
    public class Shader
    {
        public int ProgramId { get; private set; } // Shader handle
        private ShaderProgramSource _shaderProgramSource { get; }
        public bool Compiled { get; private set; }
        private readonly IDictionary<string, int> _uniforms = new Dictionary<string, int>();

        public Shader(ShaderProgramSource shaderProgramSource, bool compile  = false)
        {
            _shaderProgramSource = shaderProgramSource;
            if(compile)
            {
                CompileShader();
            }
        }

        public bool CompileShader()
        {
            if (_shaderProgramSource == null)
            {
                Console.WriteLine("Shader program source is Null");
                return false;
            }
            if (Compiled)
            {
                Console.WriteLine("Shader is already compiled");
                return false;
            }
            int vertexShaderId = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderId, _shaderProgramSource.VertexShaderSource); // Give it the source
            GL.CompileShader(vertexShaderId); // Compile the shader
            GL.GetShader(vertexShaderId, ShaderParameter.CompileStatus, out var vertexShaderCompilationCode);
            if (vertexShaderCompilationCode != (int)All.True)
            {
                Console.WriteLine(GL.GetShaderInfoLog(vertexShaderId));
                return false;
            }

            int fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderId, _shaderProgramSource.FragmentShaderSource); // Specify source
            GL.CompileShader(fragmentShaderId); // Compile the shader
            GL.GetShader(fragmentShaderId, ShaderParameter.CompileStatus, out var fragmentShaderCompilationCode);
            if (fragmentShaderCompilationCode != (int)All.True)
            {
                Console.WriteLine(GL.GetShaderInfoLog(fragmentShaderId));
                return false;
            }

            // Create actual shader and link them to pass information
            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, vertexShaderId);
            GL.AttachShader(ProgramId, fragmentShaderId);
            GL.LinkProgram(ProgramId);

            // Cleanup
            GL.DetachShader(ProgramId, vertexShaderId);
            GL.DetachShader(ProgramId, fragmentShaderId);
            GL.DeleteShader(vertexShaderId);
            GL.DeleteShader(fragmentShaderId);

            GL.GetProgram(ProgramId, GetProgramParameterName.ActiveUniforms, out var totalUniforms);
            for (int i = 0; i < totalUniforms; i++)
            {
                string key = GL.GetActiveUniform(ProgramId, i, out _, out _);
                int location = GL.GetUniformLocation(ProgramId, key);
                _uniforms.Add(key, location);
            }

            Compiled = true;
            return true;
        }

        public int GetUniformLocation(string uniformName) => _uniforms[uniformName];

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(ProgramId);
            GL.UniformMatrix4(_uniforms[name], true, ref data);
        }

        public void SetInt(string uniformName, int value)
        {
            GL.UseProgram(ProgramId);
            int location = GetUniformLocation(uniformName);
            GL.Uniform1(location, value);
        }

        public void Debug()
        {
            foreach (KeyValuePair<string, int> s in _uniforms)
            {
                Console.WriteLine($"Uniform: {s.Key}. Value: {s.Value}");
            }
        }

        public void Use()
        {
            if (Compiled)
            {
                GL.UseProgram(ProgramId);
            }
            else
            {
                throw new Exception("Shader has not been compiled!");
            }
        }

        public static ShaderProgramSource ParseShader(string filePath)
        {
            string[] shaderSource = new string[2];
            eShaderType shaderType = eShaderType.NONE;
            var allLines = File.ReadAllLines(filePath);
            for(int i = 0; i < allLines.Length; i++)
            {
                string current = allLines[i];
                if (current.ToLower().Contains("#shader"))
                {
                    if (current.ToLower().Contains("vertex"))
                    {
                        shaderType = eShaderType.VERTEX;
                    }
                    else if (current.ToLower().Contains("fragment"))
                    {
                        shaderType = eShaderType.FRAGMENT;
                    }
                    else
                    {
                        Console.WriteLine("Error. No shader type has been supplied");
                    }
                }
                else
                {
                    shaderSource[(int)shaderType] += current + Environment.NewLine;
                }
            }
            return new ShaderProgramSource(shaderSource[(int)eShaderType.VERTEX], shaderSource[(int)eShaderType.FRAGMENT]);
        }
    }
}
