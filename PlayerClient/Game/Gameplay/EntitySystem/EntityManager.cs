/*
 * File: EntityManager.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Content;
using PlayerClient.Game.Gameplay.InputSubsystem;
using PlayerClient.Game.Gameplay.Rendering;
using Shared.Game.InputSystem;
using Shared.Network.Package;

namespace PlayerClient.Game.Gameplay.EntitySystem
{
    public class EntityManager : ManagerClass
    {
        List<Entity> entities;
        Entity thisPlayer;
        public EntityManager() : base()
        {
            entities = new List<Entity>();
        }

        public override void Initialize(GameplayContext context)
        {
            thisPlayer = new Entity(Vector3.Zero, "player_char2");

            entities.Add(thisPlayer);
            entities.Add(new Entity(new Vector3(2, 0, 0), "player_char1"));
        }

        public override void LoadContent(GameplayContext context, EngineContentManager contentManager)
        {
            contentManager.AddContent<Texture2D>("Texture/sprite_player", "player_char1_texture", ContentType.Texture);
            contentManager.AddContent<Texture2D>("Texture/sprite_player2", "player_char2_texture", ContentType.Texture);
        }

        public override void StartMatch(GameplayContext context)
        {
        }
        public override void Update(GameplayContext context, GameTime gameTime)
        {
            Camera camera = context.Camera;
            InputPackage inputPackage = new InputPackage();

            inputPackage.RawMovement = new Vector3
            {
                X = Input.GetAxisRaw("Horizontal"),
                Z = -Input.GetAxisRaw("Vertical")
            };

            Vector2 mouseMovement = Input.MousePositionDelta;
            inputPackage.AimAngle = MathF.Atan2(mouseMovement.Y, mouseMovement.X);

            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Fire1, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Fire2, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Reload, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Use, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Jump, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Crouch, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Ability1, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Ability2, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Ability3, Input.IsActionDown, () => { });
            inputPackage.Buttons |= GameAction.IsInputDown(InputActionFlags.Ability4, Input.IsActionDown, () => { });

            context.NetworkManager.QueueSendPackage(inputPackage);

            // sim


            if (inputPackage.RawMovement.Length() > 0)
            {
                Vector3 movement = Vector3.Normalize(inputPackage.RawMovement);
                camera.Position += movement;
                thisPlayer.Position += movement;
                Console.WriteLine($"MOVEMENT {movement}");
            }
            if (mouseMovement.Length() > 0)
            {
                camera.AddYaw(MathHelper.ToRadians(mouseMovement.X) / 2);
                camera.AddPitch(MathHelper.ToRadians(mouseMovement.Y) / 2);
                Console.WriteLine($"MOUSE MOVEMENT {mouseMovement}");
            }
        }
        public override void PostPhysicsUpdate(GameplayContext context, GameTime gameTime)
        {
        }
        public override void Draw(GameplayContext context, GameTime gameTime)
        {
            foreach (Entity entity in entities)
            {
                context.RenderCommands.Add(new RenderCommand(RenderType.Billboard, entity.BaseSkin.TextureId, entity.Position, 1, 1, context));
            }
        }
        public override void Unload(GameplayContext context)
        {
        }
    }
}
