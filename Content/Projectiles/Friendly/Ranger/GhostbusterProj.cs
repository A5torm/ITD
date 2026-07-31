using ITD.Systems;
using ITD.Utilities;
using ITD.Particles;
using ITD.Particles.Misc;

namespace ITD.Content.Projectiles.Friendly.Ranger;

public class GhostbusterProj : ModProjectile
{
    public NPC TargetLock;
    public Vector2 VacuumCleaner;
	public ParticleEmitter emitter;
	
    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.width = 8; Projectile.height = 8;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 30;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
		
		emitter = ParticleSystem.NewEmitter<EctoCloud>(ParticleEmitterDrawCanvas.WorldOverProjectiles);
        emitter.tag = Projectile;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (target != TargetLock)
            return false;
        return base.CanHitNPC(target);
    }

    public override void AI()
    {
		if (emitter != null)
            emitter.keptAlive = true;
        Player player = Main.player[Projectile.owner];
        ITDPlayer modPlayer = player.GetITDPlayer();
        Vector2 mouse = modPlayer.MousePosition;

        Projectile.timeLeft = 60;

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

        if (!player.channel)
            Projectile.Kill();

        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);

        Vector2 position = new Vector2((int)(player.position.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(player.position.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2) - new Vector2(player.direction * 5f, 0f);

        Vector2 value = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height];
        value.Y -= 2f;
        position += value * player.gravDir;

        float holdoutDistance = 36f;
        Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(mouse - playerCenter).RotatedBy(-0.075f * player.direction);
        position += holdoutOffset;

        VacuumCleaner = position;

        NPC closestNPC = null;

        float MaxDistance = 2560000;

        foreach (var target in Main.ActiveNPCs)
        {
            float idktbh = 0f;
            Rectangle targetHitbox = target.Hitbox;
            if (!target.friendly && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), playerCenter, playerCenter + Vector2.Normalize(mouse - playerCenter) * 320f, 100f, ref idktbh))
            {
                float MouseToTarget = Vector2.DistanceSquared(target.Center, mouse);

                if (MouseToTarget < MaxDistance)
                {
                    MaxDistance = MouseToTarget;
                    closestNPC = target;
                }
            }
        }
        TargetLock = closestNPC;

        if (TargetLock != null)
		{
            Projectile.Center = TargetLock.Center;
			for (int j = 0; j < 3; j++)
            {
				Vector2 particlePosition = Projectile.Center + Main.rand.NextVector2Circular(32f, 32f);
				emitter?.Emit(particlePosition, new Vector2(), 0f, 20);
			}
		}
        else
            Projectile.Center = VacuumCleaner;
	
		if (emitter != null)
		{
			for (int i = emitter.particles.Count - 1; i >= 0; i--)
			{
				ITDParticle particle = emitter.particles[i];
				particle.velocity = Vector2.Normalize(VacuumCleaner - particle.position) * 12f;
				emitter.particles[i] = particle;
			}
		}
		
        modPlayer.recoilFront = modPlayer.recoilBack = Main.rand.NextFloat(0.15f);
    }
	
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
