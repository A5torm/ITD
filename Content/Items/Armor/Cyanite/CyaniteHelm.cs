using Terraria.Localization;
using ITD.Content.Projectiles.Friendly.Misc;

namespace ITD.Content.Items.Armor.Cyanite;

[AutoloadEquip(EquipType.Head)]
public class CyaniteHelm : ModItem
{
    public static LocalizedText SetBonusText { get; private set; }
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        SetBonusText = this.GetLocalization("SetBonus");
    }
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 26;
        Item.value = Item.sellPrice(silver: 10);
        Item.rare = ItemRarityID.Green;
        Item.defense = 22;
    }
    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<CyanitePlating>() && legs.type == ModContent.ItemType<CyaniteGreaves>();
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Melee) += 0.15f;
        player.GetCritChance(DamageClass.Generic) += 0.15f;
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = SetBonusText.Value;
		player.GetModPlayer<CyaniteHelmPlayer>().setBonus = true;
    }
}

internal class CyaniteHelmPlayer : ModPlayer
{
    public bool setBonus;

    public override void ResetEffects()
    {
        setBonus = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (setBonus)
        {
			for (int i = 0; i < 8; i++)
			{
				Vector2 direction = Vector2.UnitX.RotatedBy(MathHelper.PiOver4 * i);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + direction * (32f), direction, ModContent.ProjectileType<CyaniteSpike>(), info.SourceDamage * 5, 0.25f, Player.whoAmI, 0f, 1f, 0f);
			}
		}
    }
}