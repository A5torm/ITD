﻿using ITD.Content.Projectiles.Friendly;
using ITD.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace ITD.Content.Items.Weapons.Mage
{
    public class ThunderStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ProjectileID.None, 30, 0f, true);
            Item.damage = 300;
            Item.staff[Type] = true;
            Item.useTurn = true;
			Item.shoot = ModContent.ProjectileType<LightningStaffStrike>();
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 startPos = new(Main.MouseWorld.X + Main.rand.NextFloat(-256f, 256f), Main.screenPosition.Y);
            Vector2 theoreticalEndPos = Helpers.QuickRaycast(new Vector2 (Main.MouseWorld.X, Main.screenPosition.Y), Vector2.UnitY);
            Vector2 endPos = Helpers.QuickRaycast(startPos, (theoreticalEndPos - startPos).SafeNormalize(Vector2.Zero), true);
            MiscHelpers.CreateLightningEffects(startPos, endPos);
            Projectile.NewProjectile(source, endPos, Vector2.Zero, type, damage, knockback, player.whoAmI);
            SoundEngine.PlaySound(SoundID.Item89, endPos);
            Collision.HitTiles(endPos - new Vector2(32, 0), Vector2.UnitY, 64, 16);
            for (int i = 0; i < 5; i++)
            {
                Gore.NewGore(Item.GetSource_FromThis(), endPos, new Vector2(Main.rand.NextFloat(-2, 2), -1), Main.rand.Next(61, 64));
            }
            return false;
        }
    }
}
