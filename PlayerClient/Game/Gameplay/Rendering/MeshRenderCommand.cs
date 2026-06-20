using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Content;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public class MeshRenderCommand : RenderCommand
    {
        public override RenderType Type => RenderType.Mesh;
        public MeshBuffer Mesh;
        public MeshRenderCommand(MeshBuffer mesh, Texture2D texture, Vector3 position, Quaternion rotation, Vector3 scale, GameplayContext context) : base(texture, position, rotation, scale, context)
        {
            Mesh = mesh;
        }

        public MeshRenderCommand(MeshBuffer mesh, string textureKey, Vector3 position, Quaternion rotation, Vector3 scale, GameplayContext context) : base(textureKey, position, rotation, scale, context)
        {
            Mesh = mesh;
        }
    }
}
