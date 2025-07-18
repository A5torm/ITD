﻿using System.Collections.Generic;

namespace ITD.Content.Items.Other
{
    public class BottomlessSandBucket : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.width = 20;
			Item.height = 20;
			Item.autoReuse = true;
			Item.value = Item.sellPrice(gold: 1);
			Item.master = true;

			Item.tileBoost += 2;
			
			Item.ammo = AmmoID.Sand;
			Item.notAmmo = true;
			
			Item.createTile = TileID.Sand;
        }
		
		public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
			{
				if (line.Mod == "Terraria" && line.Name == "Placeable")
				{
					line.Text = "";
                }
            }
		}
    }
}