﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ITD.Content.Buffs;
using ITD.Content.Items;
using Terraria.GameContent.ItemDropRules;

namespace ITD.Content.NPCs
{
    public class DesertBat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.damage = 20;
            NPC.aiStyle = NPCAIStyleID.Bat;
            NPC.width = 42;
            NPC.height = 24;
            NPC.defense = 7;
            NPC.lifeMax = 30;
            NPC.knockBackResist = 0.8f;
            AnimationType = NPCID.GiantBat;
            NPC.value = Item.buyPrice(silver: 4);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath4;
            AIType = NPCID.GiantBat;
        }
        public override bool PreAI()
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sand);
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<SandyBuff>(), 60 * 8);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.GetModPlayer<ITD.ITDPlayer>().ZoneDeepDesert)
            {
                return 0.25f;
            }
            return 0f;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SandBlock, 1, 2, 6));
            npcLoot.Add(ItemDropRule.Common(ItemID.BatBat, 250));
            npcLoot.Add(ItemDropRule.Common(ItemID.DepthMeter, 100));
        }
    }
}
