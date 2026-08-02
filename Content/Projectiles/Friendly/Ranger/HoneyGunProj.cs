using ITD.Content.Buffs.Debuffs;
using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Particles;
using ITD.Particles.Misc;
using ITD.Particles.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Friendly.Ranger;

public class HoneyGunProj : ModProjectile
{
    public override string Texture => ITD.BlankTexture;

    public ParticleEmitter emitter;
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
        emitter = ParticleSystem.NewEmitter<HoneyParticle>(ParticleEmitterDrawCanvas.WorldUnderProjectiles);
        emitter.tag = Projectile;
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
        if (emitter != null)
            emitter.keptAlive = true;
        if (Projectile.ai[0] > 1f)
        {
            int dustDensity = 2;

            for (int j = 0; j < dustDensity; j++)
            {
                Vector2 lerpedPosition = Vector2.Lerp(Projectile.oldPosition, Projectile.position, j / (float)dustDensity);
                lerpedPosition += new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f));
                emitter?.Emit(lerpedPosition, Projectile.velocity * 0.2f);
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
            emitter?.Emit(Projectile.position, Projectile.velocity.RotatedByRandom(2 * MathHelper.Pi) * 0.2f);

        }
        for (int i = 0; i < 32; i++)
        {
            emitter?.Emit(Projectile.position, Projectile.velocity);
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