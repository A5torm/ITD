using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Content.Items.Armor.Cyanite;
using ITD.Particles;
using ITD.Particles.Projectiles;
using Terraria.Audio;

namespace ITD.Content.Projectiles.Friendly.Ranger.Ammo;

public class CyanideBullet : ModProjectile
{
    public ParticleEmitter emitter;

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.aiStyle = 1;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.timeLeft = 600;
        Projectile.alpha = 255;
        Projectile.light = 0.5f;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 1;

        AIType = ProjectileID.Bullet;

        emitter = ParticleSystem.NewEmitter<CyaniteFlash>(ParticleEmitterDrawCanvas.WorldOverProjectiles);
        emitter.tag = Projectile;
    }

    public override void AI()
    {
        if (emitter != null)
            emitter.keptAlive = true;
    }

	private void Impact(int target = -1)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Projectile spike0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<CyaniteSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Main.rand.NextFloat(0.7f, 0.8f), 0f);
            if (target != -1)
			{
				spike0.localNPCImmunity[target] = -1; // no double hitsies
			}
			Player player = Main.player[Projectile.owner];
			if (player.active && player.GetModPlayer<CyaniteMaskPlayer>().setBonus)
			{
				Projectile spike1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.RotatedBy(0.5f), Projectile.velocity.RotatedBy(0.5f), ModContent.ProjectileType<CyaniteSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Main.rand.NextFloat(0.5f, 0.6f), 0f);
				Projectile spike2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.RotatedBy(-0.5f), Projectile.velocity.RotatedBy(-0.5f), ModContent.ProjectileType<CyaniteSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Main.rand.NextFloat(0.5f, 0.6f), 0f);
				if (target != -1)
				{
					spike1.localNPCImmunity[target] = -1; // no double hitsies
					spike2.localNPCImmunity[target] = -1; // no double hitsies
				}
			}
        }
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
		Player player = Main.player[Projectile.owner]; // boosted damage for cyanite mask set bonus
        if (player.active && player.GetModPlayer<CyaniteMaskPlayer>().setBonus)
        {
            modifiers.SourceDamage *= 1.2f;
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        emitter?.Emit(Projectile.Center, new Vector2(), 1.6f, 20);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn2, 600);
        Impact(target.whoAmI);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Impact();
        return true;
    }
}