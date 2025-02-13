﻿using ITD.Content.Tiles.LayersRework;

namespace ITD.Content.Items.Placeable
{
    public class Emberstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EmberstoneTile>());
        }
    }
}
