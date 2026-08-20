using ITD.Content.Buffs.PetBuffs;
using ITD.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Projectiles.Friendly.Pets
{
    public class SplendidPaintingPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.LightPet[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 34;
            Projectile.width = 34;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.dead && player.HasBuff(ModContent.BuffType<SplendidPaintingBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            float timePercent = (float)(Main.time / (Main.dayTime ? Main.dayLength : Main.nightLength));
            float distanceFromCenter = Math.Abs(timePercent - 0.5f) * 2f;

            float brilliance;
            if (Main.dayTime)
            {
                brilliance = MathHelper.Lerp(2f, 1f, distanceFromCenter);//just the number in the code dumbo
            }
            else
            {
                brilliance = MathHelper.Lerp(0.3f, 1f, distanceFromCenter);
            }

            float essScale = MiscHelpers.BetterEssScale(2, 0.2f);
            brilliance *= MiscHelpers.BetterEssScale(5, 0.1f);
            Projectile.rotation += 0.0025f;
            //please hold
            Projectile.Center = Vector2.Lerp(Projectile.Center, player.Top + new Vector2(0 * player.direction + player.velocity.X * 15, -40 * essScale), 0.1f);

            if (!Main.dedServ)
            {
                Lighting.AddLight(Projectile.Center, brilliance * 0.9f, brilliance * 0.6f, brilliance * 0.1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 stretch = new(1f, 1f);
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 center = Projectile.Size / 2f;
            Vector2 miragePos = Projectile.position - Main.screenPosition + center;
            Vector2 origin = new(tex.Width * 0.5f, tex.Height / Main.projFrames[Type] * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                if (Projectile.oldPos[k] == Vector2.Zero) continue;
                Vector2 trailPos = Projectile.oldPos[k] - Main.screenPosition + center;
                float fade = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;
                Color trailColor = new Color(255, 255, 255, 0) * fade * Projectile.Opacity;

                Main.EntitySpriteDraw(tex, trailPos, frame, trailColor, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            float time = Main.GlobalTimeWrappedHourly;
            float timer = (float)Main.time / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.35f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(tex, miragePos + new Vector2(0f, 2).RotatedBy(radians) * time, frame, new Color(255, 255, 255, 0) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.5f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(tex, miragePos + new Vector2(0f, 5).RotatedBy(radians) * time, frame, new Color(255, 0, 0, 0) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            // Draw the main projectile sprite
            Main.EntitySpriteDraw(tex, miragePos, frame, Color.White * Projectile.Opacity * 0.8f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}