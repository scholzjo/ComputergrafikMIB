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
    public class AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private ScenePicker _scenePicker;
        private TransformComponent _baseTransform;
        private TransformComponent _rechtsVorneTransform;
        private TransformComponent _linksVorneTransform;
        private TransformComponent _rechtsHintenTransform;
        private TransformComponent _linksHintenTransform;
        private TransformComponent _Arm1Transform;
        private TransformComponent _Arm2Transform;
        private TransformComponent _Arm3Transform;
        private PickResult _currentPick;
        private float3 _oldColor;
        private float _camAngle = 0;
        private Boolean _Arm1;
        private Boolean _Arm2;
        private Boolean _Arm3;


        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            // TRANSFROM COMPONENT
                            _baseTransform,

                            // SHADER EFFECT COMPONENT
                            new ShaderEffectComponent
                            {
                                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.7f, 0.7f, 0.7f), new float3(1, 1, 1), 5)
                            },

                            // MESH COMPONENT
                            SimpleMeshes.CreateCuboid(new float3(10, 10, 10))
                        }
                    },
                }
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = AssetStorage.Get<SceneContainer>("Auto.fus");

            _rechtsVorneTransform = _scene.Children.FindNodes(node => node.Name == "rechtsVorne")?.FirstOrDefault()?.GetTransform();
            _linksVorneTransform = _scene.Children.FindNodes(node => node.Name == "linksVorne")?.FirstOrDefault()?.GetTransform();
            _rechtsHintenTransform = _scene.Children.FindNodes(node => node.Name == "rechtsHinten")?.FirstOrDefault()?.GetTransform();
            _linksHintenTransform = _scene.Children.FindNodes(node => node.Name == "linksHinten")?.FirstOrDefault()?.GetTransform();
            _Arm1Transform = _scene.Children.FindNodes(node => node.Name == "Arm1")?.FirstOrDefault()?.GetTransform();
            _Arm2Transform = _scene.Children.FindNodes(node => node.Name == "Arm2")?.FirstOrDefault()?.GetTransform();
            _Arm3Transform = _scene.Children.FindNodes(node => node.Name == "Arm3")?.FirstOrDefault()?.GetTransform();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //_rechtsVorneTransform.Rotation = new float3(M.MinAngle(TimeSinceStart), 0, 0);

                float rechts_vorne_rotation_z = _rechtsVorneTransform.Rotation.z;
                rechts_vorne_rotation_z += 2 * -Keyboard.UpDownAxis * DeltaTime;

                float rechts_vorne_rotation_y = _rechtsVorneTransform.Rotation.y;
                rechts_vorne_rotation_y += Keyboard.LeftRightAxis * DeltaTime;

                if(rechts_vorne_rotation_y < 0.25 && rechts_vorne_rotation_y > -0.25)
                {
                    _rechtsVorneTransform.Rotation = new float3(0, rechts_vorne_rotation_y, rechts_vorne_rotation_z);
                }
                else
                {
                    _rechtsVorneTransform.Rotation = new float3(0, _rechtsVorneTransform.Rotation.y, rechts_vorne_rotation_z);
                }

                

                //Diagnostics.Log(rechts_vorne_rotation_y); 

                float links_vorne_rotation_z = _linksVorneTransform.Rotation.z;
                links_vorne_rotation_z += 2 * -Keyboard.UpDownAxis * DeltaTime;

                float links_vorne_rotation_y = _linksVorneTransform.Rotation.y;
                links_vorne_rotation_y += Keyboard.LeftRightAxis * DeltaTime;

                if(links_vorne_rotation_y < 0.25 && links_vorne_rotation_y > -0.25)
                {
                    _linksVorneTransform.Rotation = new float3(0, links_vorne_rotation_y, links_vorne_rotation_z);
                }
                else
                {
                    _linksVorneTransform.Rotation = new float3(0, _linksVorneTransform.Rotation.y, links_vorne_rotation_z);
                }


                float links_hinten_rotation_z = _linksHintenTransform.Rotation.z;
                links_hinten_rotation_z += 2 * -Keyboard.UpDownAxis * DeltaTime;
                _linksHintenTransform.Rotation = new float3(0, 0, links_hinten_rotation_z);

                float rechts_hinten_rotation_z = _rechtsHintenTransform.Rotation.z;
                rechts_hinten_rotation_z += 2 * -Keyboard.UpDownAxis * DeltaTime;
                _rechtsHintenTransform.Rotation = new float3(0, 0, rechts_hinten_rotation_z);



            if(_Arm1)
            {
                float arm1_rotation_y = _Arm1Transform.Rotation.y;
                arm1_rotation_y += 2 * Keyboard.ADAxis * DeltaTime;
                _Arm1Transform.Rotation = new float3(0, arm1_rotation_y, 0);
            }

            if(_Arm2)
            {
                float arm2_rotation_z = _Arm2Transform.Rotation.z;
                arm2_rotation_z += 2 * Keyboard.WSAxis * DeltaTime;
                if(arm2_rotation_z <= 1.5 && arm2_rotation_z >= -1.5)
                {
                    _Arm2Transform.Rotation = new float3(0, 0, arm2_rotation_z);
                }
            }

            if(_Arm3)
            {
                float arm3_rotation_z = _Arm3Transform.Rotation.z;
                arm3_rotation_z += 2 * Keyboard.WSAxis * DeltaTime;
                if(arm3_rotation_z <= 1 && arm3_rotation_z >= -1)
                {
                    _Arm3Transform.Rotation = new float3(0, 0, arm3_rotation_z);
                }
               //Diagnostics.Log(arm3_rotation_z); 
            }

            

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            _camAngle += 12.0f * M.Pi/180.0f * DeltaTime;
            RC.View = float4x4.CreateTranslation(0, 0, 40) * float4x4.CreateRotationY(_camAngle);

            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);
                _scenePicker.View = RC.View;
                _scenePicker.Projection = RC.Projection;

                List<PickResult> pickResults = _scenePicker.Pick(pickPosClip).ToList();
                PickResult newPick = null;
                if (pickResults.Count > 0)
                {
                    pickResults.Sort((a, b) => Sign(a.ClipPos.z - b.ClipPos.z));
                    newPick = pickResults[0];
                }

                if (newPick?.Node != _currentPick?.Node)
                {
                    if (_currentPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = _currentPick.Node.GetComponent<ShaderEffectComponent>();
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", _oldColor);
                    }
                    if (newPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = newPick.Node.GetComponent<ShaderEffectComponent>();
                        _oldColor = (float3)shaderEffectComponent.Effect.GetEffectParam("DiffuseColor");
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", new float3(1, 0.4f, 0.4f));

                        if(newPick.Node.Name == "Arm1")
                        {
                            _Arm1 = true;
                            _Arm2 = false;
                            _Arm3 = false;
                        }
                        else if(newPick.Node.Name == "Arm2")
                        {
                            _Arm1 = false;
                            _Arm2 = true;
                            _Arm3 = false;
                        }
                        else if(newPick.Node.Name == "Arm3")
                        {
                            _Arm1 = false;
                            _Arm2 = false;
                            _Arm3 = true;
                        }
                        else
                        {
                            _Arm1 = false;
                            _Arm2 = false;
                            _Arm3 = false;
                        }
                    }
                    _currentPick = newPick;
                }
            }

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered farame) on the front buffer.
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
