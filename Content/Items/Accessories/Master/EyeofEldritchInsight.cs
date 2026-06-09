using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Items.Accessories.Master;

public class EyeofEldritchInsight : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemNoGravity[Type] = true;
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        Item.Size = new Vector2(30);
        Item.master = true;
        Item.accessory = true;
    }

    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.Turquoise.ToVector3() * 0.5f);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<InsightedPlayer>().CorporateInsight = true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}

public class InsightedPlayer : ModPlayer
{
    public bool CorporateInsight;

    public override void ResetEffects()
    {
        CorporateInsight = false;
    }
}

public class InsightedProjectiles : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public bool isGoingToHit = false;

    public override void PostAI(Projectile projectile)
    {
        if (Main.netMode == NetmodeID.Server) return;

        Player player = Main.LocalPlayer;

        if (player.active && !player.dead && player.GetModPlayer<InsightedPlayer>().CorporateInsight)
        {
            Rectangle playerHitBox = player.Hitbox;
            Rectangle projHitbox = projectile.Hitbox;
            float num1 = 0f;

            if (projHitbox.Intersects(playerHitBox) ||
                Collision.CheckAABBvLineCollision(playerHitBox.TopLeft(), playerHitBox.Size(),
                projectile.Center, projectile.Center + projectile.velocity * 60,
                projectile.width * projectile.scale, ref num1) ||
                Collision.CheckAABBvLineCollision(projHitbox.TopLeft(), projHitbox.Size(),
                player.Center, player.Center + player.velocity * 10,
                player.width, ref num1) ||
                Collision.CheckAABBvLineCollision(playerHitBox.TopLeft() + player.velocity * 10, playerHitBox.Size(),
                projectile.Center, projectile.Center + projectile.velocity * 60,
                projectile.width * projectile.scale, ref num1))
            {
                isGoingToHit = true;
            }
            else
            {
                isGoingToHit = false;
            }
        }
        else
        {
            isGoingToHit = false;
        }
    }
    public override Color? GetAlpha(Projectile projectile, Color lightColor)
    {
        if (!isGoingToHit || !projectile.hostile || projectile.damage <= 0 || projectile.alpha >= 255)
            return null;

        Player player = Main.LocalPlayer;
        if (player.active && player.GetModPlayer<InsightedPlayer>().CorporateInsight)
        {
            if (projectile.ModProjectile is null || (projectile.ModProjectile != null && projectile.ModProjectile.CanHitPlayer(player) && (projectile.ModProjectile.CanDamage() ?? true)))
            {
                return Color.Red;
            }
        }
        return null;
    }
}

public class InsightedNPCs : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public bool isGoingToHit = false;

    public override void PostAI(NPC npc)
    {
        if (Main.netMode == NetmodeID.Server) return;

        Player player = Main.LocalPlayer;

        if (player.active && !player.dead && player.GetModPlayer<InsightedPlayer>().CorporateInsight)
        {
            Rectangle playerHitBox = player.Hitbox;
            Rectangle npcHitbox = npc.Hitbox;
            float num1 = 0f;

            if (npcHitbox.Intersects(playerHitBox) ||
                Collision.CheckAABBvLineCollision(playerHitBox.TopLeft(), playerHitBox.Size(),
                npc.Center, npc.Center + npc.velocity * 30,
                npcHitbox.Width * npc.scale, ref num1) ||
                Collision.CheckAABBvLineCollision(npcHitbox.TopLeft(), npcHitbox.Size(),
                player.Center, player.Center + player.velocity * 10,
                player.width, ref num1) ||
                Collision.CheckAABBvLineCollision(playerHitBox.TopLeft() + player.velocity * 10, playerHitBox.Size(),
                npc.Center, npc.Center + npc.velocity * 30,
                npcHitbox.Width * npc.scale, ref num1))
            {
                isGoingToHit = true;
            }
            else
            {
                isGoingToHit = false;
            }
        }
        else
        {
            isGoingToHit = false;
        }
    }

    public override Color? GetAlpha(NPC npc, Color drawColor)
    {
        if (!isGoingToHit || npc.friendly || npc.damage <= 0 || npc.Opacity <= 0f)
            return base.GetAlpha(npc, drawColor);

        Player player = Main.LocalPlayer;
        if (player.active && player.GetModPlayer<InsightedPlayer>().CorporateInsight)
        {
            if (npc.ModNPC is null || (npc.ModNPC != null && npc.ModNPC.CanHitPlayer(player, ref player.immuneTime)))
            {
                return Color.Red;
            }
        }

        return base.GetAlpha(npc, drawColor);
    }
}