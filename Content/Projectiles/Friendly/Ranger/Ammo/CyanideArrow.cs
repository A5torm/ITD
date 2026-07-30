using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Content.Items.Armor.Cyanite;
using ITD.Particles;
using ITD.Particles.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Friendly.Ranger.Ammo;

public class CyanideArrow : ModProjectile
{
    public ParticleEmitter emitter;

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(1); // assume wooden arrow identity

        emitter = ParticleSystem.NewEmitter<CyaniteFlash>(ParticleEmitterDrawCanvas.WorldOverProjectiles);
        emitter.tag = Projectile;
    }

    public override void AI()
    {
        if (emitter != null)
            emitter.keptAlive = true;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        emitter?.Emit(Projectile.Center, new Vector2(), 2f, 20);
    }

	private void Impact(int target = -1)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Projectile spike0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<CyaniteSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Main.rand.NextFloat(0.8f, 1f), 0f);
            if (target != -1)
			{
				spike0.localNPCImmunity[target] = -1; // no double hitsies
			}
			Player player = Main.player[Projectile.owner];
			if (player.active && player.GetModPlayer<CyaniteMaskPlayer>().setBonus)
			{
				Projectile spike1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.RotatedBy(0.5f), Projectile.velocity.RotatedBy(0.5f), ModContent.ProjectileType<CyaniteSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Main.rand.NextFloat(0.7f, 0.8f), 0f);
				Projectile spike2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.RotatedBy(-0.5f), Projectile.velocity.RotatedBy(-0.5f), ModContent.ProjectileType<CyaniteSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Main.rand.NextFloat(0.7f, 0.8f), 0f);
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

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 position = Projectile.Center - Main.screenPosition;

        Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
        Rectangle sourceRectangle = texture.Frame(1, 1);
        Vector2 origin = sourceRectangle.Size() / 2f;

        Main.EntitySpriteDraw(texture, position, sourceRectangle, new Color(120, 184, 255, 50), Main.GlobalTimeWrappedHourly * 2f, origin, 1f * Main.essScale, SpriteEffects.None, 0f);
        Main.EntitySpriteDraw(texture, position, sourceRectangle, new Color(120, 184, 255, 50), Main.GlobalTimeWrappedHourly * 2f + MathHelper.PiOver2, origin, 1f * Main.essScale, SpriteEffects.None, 0f);

        return true;
    }
}