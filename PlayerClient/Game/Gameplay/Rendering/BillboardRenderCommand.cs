using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public class BillboardRenderCommand : RenderCommand
    {
        public override RenderType Type => RenderType.Billboard;
        public BillboardRenderCommand(Texture2D texture, Vector3 position, Quaternion rotation, Vector3 scale, GameplayContext context) : base(texture, position, rotation, scale, context)
        {
        }

        public BillboardRenderCommand(string textureKey, Vector3 position, Quaternion rotation, Vector3 scale, GameplayContext context) : base(textureKey, position, rotation, scale, context)
        {
        }
    }
}
