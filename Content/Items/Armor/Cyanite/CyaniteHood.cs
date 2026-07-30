using Terraria.Localization;

namespace ITD.Content.Items.Armor.Cyanite;

[AutoloadEquip(EquipType.Head)]
public class CyaniteHood : ModItem
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
        Item.defense = 17;
    }
    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
		return body.type == ModContent.ItemType<CyanitePlating>() && legs.type == ModContent.ItemType<CyaniteGreaves>();
    }
	
    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Magic) += 0.15f;
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = SetBonusText.Value;
		player.manaCost -= 0.70f;
		player.GetModPlayer<CyaniteHoodPlayer>().setBonus = true;
    }
}

internal class CyaniteHoodPlayer : ModPlayer
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
			Player.AddBuff(BuffID.Frostburn2, 480, false);
		}
    }
}