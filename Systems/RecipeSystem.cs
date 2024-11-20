﻿using ITD.Content.Items.Weapons.Melee;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using ITD.Utilities;

namespace ITD.Systems
{
    public class RecipeSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                // spidermeal shimmering. we can't use ItemID.Sets.ShimmerTransformToItem since you can't add a condition to it
                if (recipe.HasResult(ItemID.Flymeal))
                {
                    recipe.AddDecraftCondition(Condition.Hardmode);
                    recipe.AddCustomShimmerResult(ModContent.ItemType<Spidermeal>());
                }
            }
        }
        public override void AddRecipeGroups()
        {
            RecipeGroup group = new(() => Language.GetTextValue("LegacyMisc.37") + " Iron Ore",
            [
                ItemID.IronOre,
                ItemID.LeadOre
            ]);
            RecipeGroup.RegisterGroup("IronOre", group);
        }
    }
}
