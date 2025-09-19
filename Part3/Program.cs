using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

class Program : GameWindow
{
    int _vao, _vbo, _ebo, _shaderProgram;
    int _transformLocation;

    float[] vertices = {
        // Position (x,y)   // Color (r,g,b)
        -0.5f, -0.5f,       1f, 0f, 0f,  // bottom left
         0.5f, -0.5f,       0f, 1f, 0f,  // bottom right
         0.5f,  0.5f,       0f, 0f, 1f,  // top right
        -0.5f,  0.5f,       1f, 1f, 0f   // top left
    };

    uint[] indices = {
        0, 1, 2,
        0, 2, 3
    };

    string vertexShaderSource = @"
        #version 330 core
        layout(location = 0) in vec2 aPos;
        layout(location = 1) in vec3 aColor;
        out vec3 ourColor;

        uniform mat4 transform;

        void main()
        {
            gl_Position = transform * vec4(aPos, 0.0, 1.0);
            ourColor = aColor;
        }
    ";

    string fragmentShaderSource = @"
        #version 330 core
        in vec3 ourColor;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(ourColor, 1.0);
        }
    ";

    public Program(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0f, 0f, 0f, 1f);

        // --- Compile Shaders ---
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // --- Setup Buffers ---
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Get uniform location
        _transformLocation = GL.GetUniformLocation(_shaderProgram, "transform");
    }

    protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vao);

        // --- Transformation: rotation over time ---
        float time = (float)GLFW.GetTime();
        Matrix4 rotation = Matrix4.CreateRotationZ(time);   // rotate around Z-axis
        Matrix4 scale = Matrix4.CreateScale((float)Math.Sin(time) * 0.5f + 1.0f); 
        Matrix4 transform = rotation * scale;

        GL.UniformMatrix4(_transformLocation, false, ref transform);

        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    static void Main()
    {
        var gws = GameWindowSettings.Default;
        var nws = new NativeWindowSettings() { Size = new Vector2i(800, 600), Title = "Square Transform Shader" };
        using var window = new Program(gws, nws);
        window.Run();
    }
}
