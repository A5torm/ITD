﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Buffs
{
    public class SandyBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            Dust.NewDust(player.position, player.width, player.height, DustID.Sand);
            player.velocity.X += Main.rand.NextFloat(-1.5f, 1.5f);
            if (Main.rand.NextBool(50) && player.velocity.Y <= float.Epsilon && player.velocity.Y > -float.Epsilon && player.jump == 0)
            {
                player.velocity.Y -= 5f;
                player.velocity.X += Main.rand.NextFloat(-2f, 2f);
            }
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Sand);
            npc.velocity.X += Main.rand.NextFloat(-1.5f, 1.5f);
            if (Main.rand.NextBool(50) && npc.velocity.Y <= float.Epsilon && npc.velocity.Y > -float.Epsilon)
            {
                if (npc.aiStyle == NPCAIStyleID.Fighter)
                {
                    npc.velocity.Y -= 5f;
                    npc.velocity.X += Main.rand.NextFloat(-2f, 2f);
                }
            }
        }
    }
}