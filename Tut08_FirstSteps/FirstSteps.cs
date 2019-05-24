using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{
    public class FirstSteps : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float _camAngle = 0;
        private TransformComponent _cubeTransform;
        private TransformComponent _cube2Transform;
        private TransformComponent _cube3Transform;
        
        // Init is called on startup. 
        public override void Init()
        {
            RC.ClearColor = new float4(0.3f, 0.2f, 0.8f, 1.0f);

            _scene = new SceneContainer();
            _scene.Children = new List<SceneNodeContainer>();

            var cubeNode = new SceneNodeContainer();
            _scene.Children.Add(cubeNode);

            var cube2Node = new SceneNodeContainer();
            _scene.Children.Add(cube2Node);

            var cube3Node = new SceneNodeContainer();
            _scene.Children.Add(cube3Node);


            //The three components: one XForm, one Shader and the Mesh
            _cubeTransform = new TransformComponent {
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 10),
                Rotation = new float3(0, 0, 0)
            };

            var cubeShader = new ShaderEffectComponent
            { 
                Effect = SimpleMeshes.MakeShaderEffect(new float3 (0, 1, 0), new float3 (1, 1, 1),  4)
            };

            var cubeMesh = SimpleMeshes.CreateCuboid(new float3(10, 10, 10));
            cubeNode.Components = new List<SceneComponentContainer>();
            cubeNode.Components.Add(_cubeTransform);
            cubeNode.Components.Add(cubeShader);
            cubeNode.Components.Add(cubeMesh);



            _cube2Transform = new TransformComponent {
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0),
                Rotation = new float3(0, 0, 0)
            };

            var cube2Shader = new ShaderEffectComponent {
                Effect = SimpleMeshes.MakeShaderEffect(new float3 (0, 0, 1), new float3 (1, 1, 1),  4)
            };

            var cube2Mesh = SimpleMeshes.CreateCuboid(new float3(5, 5, 5));
            cube2Node.Components = new List<SceneComponentContainer>();
            cube2Node.Components.Add(_cube2Transform);
            cube2Node.Components.Add(cube2Shader);
            cube2Node.Components.Add(cube2Mesh);



            _cube3Transform = new TransformComponent {
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0),
                Rotation = new float3(0, 0, 0)
            };

            var cube3Shader = new ShaderEffectComponent {
                Effect = SimpleMeshes.MakeShaderEffect(new float3 (1, 0, 0), new float3 (1, 1, 1),  4)
            };

            var cube3Mesh = SimpleMeshes.CreateCuboid(new float3(5, 5, 5));
            cube3Node.Components = new List<SceneComponentContainer>();
            cube3Node.Components.Add(_cube3Transform);
            cube3Node.Components.Add(cube3Shader);
            cube3Node.Components.Add(cube3Mesh);

            

            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Animation cube
            //_cubeTransform.Translation = new float3(0, 10.0f * M.Sin(5 * TimeSinceStart), 0);
            //_cubeTransform.Rotation = new float3(4.5f * Time.DeltaTime, 0, 0);
            _cubeTransform.Scale = new float3(1, 1 - (M.Sin(3.5f * TimeSinceStart)), 1);
            

            // Animation cube2
            _cube2Transform.Translation = new float3(10.0f * M.Sin(5 * TimeSinceStart),0 , 0);
            _cube2Transform.Rotation = new float3(0, 0, 1.5f * M.Sin(5 * TimeSinceStart));

            // Animation cube3
            _cube3Transform.Translation = new float3(-10.0f * M.Sin(5 * TimeSinceStart),0 , 20);
            _cube3Transform.Rotation = new float3(0, 0, -1.5f * M.Sin(5 * TimeSinceStart));

            // Animate the camera angle
            _camAngle = _camAngle + 12.0f * M.Pi/180.0f * DeltaTime;

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 50) * float4x4.CreateRotationY(_camAngle);

            _sceneRenderer.Render(RC);


            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}