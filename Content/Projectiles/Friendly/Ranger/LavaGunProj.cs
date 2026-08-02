using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Particles;
using ITD.Particles.Misc;
using ITD.Particles.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Friendly.Ranger;

public class LavaGunProj : ModProjectile
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
        emitter = ParticleSystem.NewEmitter<PyroclasticParticle>(ParticleEmitterDrawCanvas.WorldUnderProjectiles);
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
            emitter?.Emit(Projectile.position, Projectile.velocity.RotatedByRandom(2 * MathHelper.Pi) * 0.5f);

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