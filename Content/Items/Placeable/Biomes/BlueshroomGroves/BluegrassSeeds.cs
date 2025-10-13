﻿using ITD.Content.Tiles.BlueshroomGroves;
using ITD.Utilities;

namespace ITD.Content.Items.Placeable.Biomes.BlueshroomGroves;

public class BluegrassSeeds : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }
    public override void SetDefaults()
    {
        Item.DefaultToSeeds();
    }
    public override bool? UseItem(Player player) => Helpers.UseItemPlaceSeeds(player, ModContent.TileType<Bluegrass>(), TileID.SnowBlock);
}
