﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Projectiles.Hostile
{
    public class SandSpike : ModProjectile
    {
		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
			Projectile.height = 32;
			Projectile.aiStyle = 157;
			Projectile.hostile = true;
			Projectile.alpha = 255;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
        }
		
		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
			Microsoft.Xna.Framework.Rectangle rectangle11 = texture.Frame(1, 6, 0, Projectile.frame, 0, 0);
			Vector2 origin10 = new Vector2(16f, (float)(rectangle11.Height / 2));
			Microsoft.Xna.Framework.Color alpha4 = Projectile.GetAlpha(lightColor);
			Vector2 scale10 = new Vector2(Projectile.scale);
			float expr_A5D0 = 35f;
			float lerpValue4 = Utils.GetLerpValue(expr_A5D0, expr_A5D0 - 5f, Projectile.ai[0], true);
			scale10.Y *= lerpValue4;
			Vector4 value25 = lightColor.ToVector4();
			Vector4 vector35 = new Microsoft.Xna.Framework.Color(67, 17, 17).ToVector4();
			vector35 *= value25;
			//Main.EntitySpriteDraw(TextureAssets.Extra[98].get_Value(), Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) - Projectile.velocity * Projectile.scale * 0.5f, null, Projectile.GetAlpha(new Microsoft.Xna.Framework.Color(vector35.X, vector35.Y, vector35.Z, vector35.W)) * 1f, Projectile.rotation + 1.57079637f, TextureAssets.Extra[98].get_Value().Size() / 2f, Projectile.scale * 0.9f, spriteEffects, 0f);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle11), alpha4, Projectile.rotation, origin10, scale10, SpriteEffects.None, 0f);

            return false;
        }
    }
}
