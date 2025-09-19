/*
 * Name: Luckshihaa Krishnan 
 * Student ID: 186418216
 * Section: GAM 531 NSA 
 */

using System;
using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Week3Lab
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        // Constructor for Game class
        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            //Set window size to 1280x768
            this.Size = new Vector2i(1280, 768);

            // Center the window on the screen
            this.CenterWindow(this.Size);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            // Update the OpenGL viewport to match the new window dimensions
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }


        // When the game is loading
        protected override void OnLoad()
        {
            base.OnLoad();

            // Setting background color
            GL.ClearColor(new Color4(0.4f, 0.2f, 0.5f, 1f));

            //Creating an array with coordinates (to draw triangle)
            float[] vertices = new float[]
            {
                -0.2f, 0.5f, 0.0f,    1.0f,0.0f,1.0f,    // Triangle #1 - top left vertex
                 0.2f, 0.5f, 0.0f,    0.0f,0.0f,1.0f,   // Triangle #1 - top right vertex
                -0.2f, -0.5f, 0.0f,   1.0f,1.0f,1.0f,  // Triangle #1 - bottom vertex

                0.2f, 0.5f, 0.0f,    0.0f,0.0f,1.0f,    // Triangle #2 - top vertex
                0.2f, -0.5f, 0.0f,   0.0f,1.0f, 0.0f,   // Triangle #2 - bottom right vertex
               -0.2f, -0.5f, 0.0f,   1.0f,1.0f,1.0f,   // Triangle #2 - bottom left vertex

            };

            // Generate a Vertex Buffer Object (VBO) to store vertex data on GPU
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Generate a Vertex Array Object (VAO) to store the VBO configuration
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);

            // Bind the VBO and define the layout of vertex data for shaders
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);

            // for position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // for color
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Vertex shader: Positions each vertex
            string vertexShaderCode =
                @"
                    #version 330 core
                    layout(location = 0) in vec3 aPosition;     // Vertex position input
                    layout (location = 1) in vec3 aColor;

                    out vec3 vColor;

                    void main()
                    {
                        gl_Position = vec4(aPosition, 1.0);    //Converting vec3 to vec4 for output
                        vColor = aColor;
                    }
            
                ";

            // Fragment shader: outputs a single color
            string fragmentShaderCode =
                @"
                    #version 330 core
                    out vec4 FragColor;
                    in vec3 vColor;
                    
                    void main()
                    {
                        FragColor = vec4(vColor, 1.0f);  //Orage-red color (color of triangle)
                    }
                    
                ";


            // Creating Compiler Shaders
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


        }


        // When the game is unloading
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


        // Game is updated
        // Called every frame to update game logic, physics or input handling
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }


        // Called when I need to update any game visuals
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            //Clear the screen with background color
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Use our shader program
            GL.UseProgram(shaderProgramHandle);

            // Bind the VAO and draw the triangle
            GL.BindVertexArray(vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);   //drawing the 6 vertices
            GL.BindVertexArray(0);

            // Display the rendered frame
            SwapBuffers();
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