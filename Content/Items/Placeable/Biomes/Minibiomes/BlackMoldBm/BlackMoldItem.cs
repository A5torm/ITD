﻿using ITD.Content.Tiles.Misc;
using ITD.Utilities;

namespace ITD.Content.Items.Placeable.Biomes.Minibiomes.BlackMoldBm
{
    public class BlackMoldItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToSeeds();
        }
        public override bool? UseItem(Player player) => Helpers.UseItem_PlaceSeeds(player, ModContent.TileType<BlackMold>(), TileID.Stone);
    }
}
