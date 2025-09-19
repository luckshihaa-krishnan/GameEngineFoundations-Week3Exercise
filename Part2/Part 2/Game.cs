﻿using System;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

namespace WindowEngine
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        uint[] indices = {
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };

        // Constructor
        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            // Set window size to 1280x768
            this.Size = new Vector2i(1280, 768);

            // Center the window on the screen
            this.CenterWindow(this.Size);
        }

        // Called automatically whenever the window is resized
        protected override void OnResize(ResizeEventArgs e)
        {
            // Update the OpenGL viewport to match the new window dimensions
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        // Called once when the game starts, ideal for loading resources
        protected override void OnLoad()
        {
            base.OnLoad();

            // Set the background color (RGBA)
            GL.ClearColor(new Color4(0.5f, 0.7f, 0.8f, 1f));

            // Define a simple triangle in normalized device coordinates (NDC)
            float[] vertices = {
               -0.5f,  0.5f, 0.0f,  // Top-left
                0.5f,  0.5f, 0.0f,  // Top-right
                0.5f, -0.5f, 0.0f,  // Bottom-right
               -0.5f, -0.5f, 0.0f,  // Bottom-left
            };

            uint[] indices = {
                0, 1, 2,
                0, 2, 3
            };

            // Generate VBO and VAO
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Generate a Vertex Array Object (VAO) to store the VBO configuration
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);

            // Bind the VBO and define the layout of vertex data for shaders
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Create element buffer object
            int elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            // Vertex shader: positions each vertex
            string vertexShaderCode = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition; // Vertex position input

                void main()
                {
                    gl_Position = vec4(aPosition, 1.0); // Convert vec3 to vec4 for output
                }
            ";

            // Fragment shader: outputs a single color
            string fragmentShaderCode = @"
                #version 330 core
                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(0.6f, 0.2f, 0.8f, 1.0f); // Orange-red color
                }
            ";

            // Compile shaders
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckShaderCompile(vertexShaderHandle, "Vertex Shader");

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);
            CheckShaderCompile(fragmentShaderHandle, "Fragment Shader");

            // Create shader program and link shaders
            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            // Cleanup shaders after linking (no longer needed individually)
            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);

            Console.WriteLine($"Vertices: \n" +
                $"{vertices[0]},{vertices[1]}, {vertices[2]}\n" +
                $"{vertices[3]},{vertices[4]}, {vertices[5]}\n" +
                $"{vertices[6]},{vertices[7]}, {vertices[8]}\n" +
                $"{vertices[9]},{vertices[10]}, {vertices[11]}\n");
        }

        // Called every frame to update game logic
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            // Handle input, animations, physics, AI, etc.
        }

        // Called every frame to render graphics
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Clear the screen with background color
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Use our shader program
            GL.UseProgram(shaderProgramHandle);

            // Bind the VAO and draw the triangle
            GL.BindVertexArray(vertexArrayHandle);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // Display the rendered frame
            SwapBuffers();
        }

        // Called when the game is closing or resources need to be released
        protected override void OnUnload()
        {
            // Unbind and delete buffers and shader program
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);

            base.OnUnload();
        }

        // Helper function to check for shader compilation errors
        private void CheckShaderCompile(int shaderHandle, string shaderName)
        {
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderHandle);
                Console.WriteLine($"Error compiling {shaderName}: {infoLog}");
            }
        }
    }
}
