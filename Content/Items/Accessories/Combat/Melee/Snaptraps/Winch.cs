﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ITD.Systems;

namespace ITD.Content.Items.Accessories.Combat.Melee.Snaptraps
{
    public class Winch : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SnaptrapPlayer>().LengthIncrease += 0.10f;
            player.GetModPlayer<SnaptrapPlayer>().RetractMultiplier += 0.15f;
        }
    }
}