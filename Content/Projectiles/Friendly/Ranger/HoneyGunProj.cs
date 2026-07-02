using ITD.Content.Buffs.Debuffs;
using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Particles;
using ITD.Particles.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Friendly.Ranger;

public class HoneyGunProj : ModProjectile
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
        Projectile.tileCollide = true;
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
        if (Projectile.ai[0] > 1f)
        {
            int dustDensity = 2;

            for (int j = 0; j < dustDensity; j++)
            {
                Vector2 lerpedPosition = Vector2.Lerp(Projectile.oldPosition, Projectile.position, j / (float)dustDensity);
                lerpedPosition += new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f));
                int dustIndex = Dust.NewDust(lerpedPosition, 1, 1, DustID.Honey, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, Color.White, 1.4f);

                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity *= 0.1f;

                if (Main.rand.NextBool(8))
                {
                    int dust2 = Dust.NewDust(Projectile.position, 1, 1, DustID.Honey2, Projectile.velocity.X, Projectile.velocity.Y,
                        100, Color.White, 1.4f);
                    Main.dust[dust2].noGravity = false;
                    Main.dust[dust2].velocity *= 0.9f;
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
            int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Honey, 0, 0, 100, default, 1.5f);
            Main.dust[dust].noGravity = true;

            Main.dust[dust].velocity *= Main.rand.NextFloat(2f, 3f);
            Main.dust[dust].velocity = Main.dust[dust].velocity.RotatedByRandom(2 * MathHelper.Pi);

        }
        for (int i = 0; i < 32; i++)
        {
            int splash = Dust.NewDust(Projectile.position, 1, 1, DustID.Honey2, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 2f);
            Main.dust[splash].noGravity = true;
            Main.dust[splash].velocity *= Main.rand.NextFloat(1f, 3f);
            Main.dust[splash].velocity *= 0.9f;

        }
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.isLikeATownNPC)
        {
            if (target.life < target.lifeMax - 1)
            {
                target.HealEffect(target.lifeMax - target.life);
                target.life = target.lifeMax;
            }
            else
            {
                target.HealEffect(target.lifeMax / 2);
                target.life += target.lifeMax/2;
                target.AddBuff(BuffID.Honey, 60 * 300);
            }
            modifiers.FinalDamage.Flat *= 0;
        }
        
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!target.isLikeATownNPC)
        {
            target.AddBuff(ModContent.BuffType<RoyalJellyDebuff>(), 60 * 3);
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity *= 0.4f;
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return true;
    }
}