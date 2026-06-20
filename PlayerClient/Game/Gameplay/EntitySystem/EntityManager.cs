/*
 * File: EntityManager.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 20 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Maths;
using PlayerClient.Game.Content;
using PlayerClient.Game.Gameplay.InputSubsystem;
using PlayerClient.Game.Gameplay.Rendering;
using Shared.Core.Game;
using Shared.Game.BVH;
using Shared.Game.InputSystem;
using Shared.Game.Simulator;
using Shared.Network.Package;

namespace PlayerClient.Game.Gameplay.EntitySystem
{
    public class EntityManager : ManagerClass
    {
        List<Entity> entities;
        Entity thisPlayer;
        InputBuffer inputBuffer;
        PlayerState predictedState;
        SpriteFont spriteFont;
        public EntityManager() : base()
        {
            inputBuffer = new InputBuffer();
            entities = new List<Entity>();
        }

        public override void Initialize(GameplayContext context)
        {
            thisPlayer = new Entity(Vector3.Up * 50, "player_char2");

            // entities.Add(thisPlayer);
            // entities.Add(new Entity(new Vector3(2, 0, 0), "player_char1"));
        }

        public override void LoadContent(GameplayContext context, EngineContentManager contentManager)
        {
            spriteFont = contentManager._contentManager.Load<SpriteFont>("Fonts/Arial");
        }

        public override void StartMatch(GameplayContext context)
        {
            EngineContentManager.Instance.AddContent<Texture2D>("player_char1_texture", "Texture/sprite_player", ContentType.Texture);
            EngineContentManager.Instance.AddContent<Texture2D>("player_char2_texture", "Texture/sprite_player2", ContentType.Texture);
            predictedState = new PlayerState();
            predictedState.GravityScale = 1f;
            predictedState.Position = Vector3.Up * 5;
        }
        public override void Update(GameplayContext context, GameTime gameTime)
        {
            Camera camera = context.Camera;

            Vector3 rawMovement = new Vector3();

            Vector2 mouseMovement = -Input.MousePositionDelta;

            InputActionFlags inputAction = 0;
            inputAction |= GameAction.IsInputDown(InputActionFlags.Right, Input.IsActionDown, () => { rawMovement -= camera.Right; });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Left, Input.IsActionDown, () => { rawMovement += camera.Right; });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Forward, Input.IsActionDown, () => { rawMovement += camera.Forward; });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Backward, Input.IsActionDown, () => { rawMovement -= camera.Forward; });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Fire1, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Fire2, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Reload, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Use, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Jump, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Crouch, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Ability1, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Ability2, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Ability3, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Ability4, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Ability5, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Ability6, Input.IsActionDown, () => { });
            inputAction |= GameAction.IsInputDown(InputActionFlags.Ability7, Input.IsActionDown, () => { });


            InputPackage inputPackage = inputBuffer.Record(rawMovement, inputAction, mouseMovement);
            BVHTree tree = context.MapManager.GetLoadedMap().BVH;
            if (tree != null)
            {
                predictedState = MovementSimulator.Simulate(predictedState, inputPackage, tree, thisPlayer.RuntimeState.Base.MovementStats, (float)gameTime.ElapsedGameTime.TotalSeconds);
                context.NetworkManager.QueueSendPackage(inputPackage);
            }

            // server shit
        }
        public override void PostPhysicsUpdate(GameplayContext context, GameTime gameTime)
        {
            Camera camera = context.Camera;
            camera.Position = predictedState.Position;
            camera.AddYaw(predictedState.AimYaw);
            camera.AddPitch(predictedState.AimPitch);
        }
        public override void Draw(GameplayContext context, GameTime gameTime)
        {
            foreach (Entity entity in entities)
            {
                context.RenderCommands.Add(new BillboardRenderCommand(entity.BaseSkin.TextureId, predictedState.Position, Quaternion.Identity, Vector3.One, context));
            }
            context.SpriteBatch.DrawString(spriteFont, $"P:{predictedState.Position}", new Vector2(0), Color.Blue);
            context.SpriteBatch.DrawString(spriteFont, $"V:{predictedState.Velocity}", new Vector2(0, 20), Color.Blue);
            context.SpriteBatch.DrawString(spriteFont, $"G:{predictedState.IsGrounded}", new Vector2(0, 40), Color.Blue);
        }
        public override void Unload(GameplayContext context)
        {
        }
    }
}
