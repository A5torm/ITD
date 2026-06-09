using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Particles;
using ITD.Particles.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Friendly.Ranger;

public class LavaGunProj : ModProjectile
{
    public override string Texture => ITD.BlankTexture;

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.hostile = true;
        Projectile.alpha = 255;
        Projectile.penetrate = 1;
        Projectile.MaxUpdates = 3;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 15f)
        {
            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }
        if (Projectile.ai[0] > 6f)
        {
            int dustDensity = 2;

            for (int j = 0; j < dustDensity; j++)
            {
                Vector2 lerpedPosition = Vector2.Lerp(Projectile.oldPosition, Projectile.position, j / (float)dustDensity);
                lerpedPosition += new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f));
                int dustIndex = Dust.NewDust(lerpedPosition, 1, 1, DustID.Lava, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, Color.White, 1.4f);

                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity *= 0.1f;

                if (Main.rand.NextBool(8))
                {
                    int dust2 = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y,
                        100, Color.White, 1.4f);
                    Main.dust[dust2].noGravity = false;
                }
            }
        }
    }
    public override bool CanHitPlayer(Player target)
    {
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Lava, 0, 0, 100, default, 1.5f);
            Main.dust[dust].noGravity = true;

            Main.dust[dust].velocity *= Main.rand.NextFloat(2f,3f);
            Main.dust[dust].velocity = Main.dust[dust].velocity.RotatedByRandom(2 * MathHelper.Pi);

        }
        for (int i = 0; i < 32; i++)
        {
            int splash = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 2f);
            Main.dust[splash].noGravity = true;
            Main.dust[splash].velocity *= Main.rand.NextFloat(1f, 3f);
        }
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.isLikeATownNPC)
        {
            if (target.life > target.lifeMax)
            {
                target.life = target.lifeMax;
            }
            modifiers.FinalDamage.Base = target.lifeMax / 2;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 60 * 10);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return true;
    }
}