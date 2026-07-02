using ITD.Content.Items.Accessories.Master;
using ITD.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.NPCs.Friendly
{
    public class SlimeCoating : ModNPC
    {
        public bool Jumped = false;
        public bool Collided = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 56;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.knockBackResist = 0.3f;
            NPC.netAlways = true;
            NPC.friendly = true;
            NPC.scale = 1.3f;
            NPC.aiStyle = -1;
            NPC.gfxOffY = -10;
            NPC.alpha = 30;
            NPC.immortal = false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return false;
        }

        int iRetardedIframe;
        public override bool CanBeHitByNPC(NPC attacker)
        {
            return iRetardedIframe <= 0;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.hostile && !projectile.friendly)
                return iRetardedIframe <= 0;
            else return false;
        }

        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[0]];
            if (!CheckActive(player))
            {
                return;
            }

            if (!player.GetModPlayer<KSGlandPlayer>().ksMasterAcc)
            {
                NPC.immortal = false;
                NPC.life = 0;
                NPC.checkDead();
                return;
            }

            if (player.oldVelocity.Y < 0 && player.velocity.Y == 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.RedTorch, 0, 0, 80, new Color(0, 80, 255, 100), Main.rand.NextFloat(1f, 2f));
                    dust.velocity *= 2f;
                    Dust dust2 = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.t_Slime, 0, 0, 80, new Color(0, 80, 255, 100), Main.rand.NextFloat(1f, 2f));
                    dust2.velocity *= 1f;
                    dust2.noGravity = true;
                }
            }

            if (!Jumped)
            {
                if (player.velocity.Y < 0)
                {
                    Jumped = true;
                }
            }
            else
            {
                if (player.velocity.Y == 0)
                {
                    moveDust();
                    Jumped = false;
                }
            }
            if (player.velocity.X == 0 && Math.Abs(player.oldVelocity.X) > 0.01f)
            {
                if (!Collided)
                {
                    SoundEngine.PlaySound(SoundID.Item150, NPC.Center);
                    Collided = true;
                }
            }
            else if (Math.Abs(player.velocity.X) > 0f)
            {
                Collided = false;
            }

            void moveDust()
            {
                SoundEngine.PlaySound(SoundID.Item150, NPC.Center);
                for (int j = 0; j < 12; j++)
                {
                    Dust dust = Dust.NewDustDirect(player.Bottom + new Vector2(-20, 0), player.width * 2, 10, DustID.t_Slime, 0, 0, 80, new Color(0, 80, 255, 100));
                    dust.noGravity = true;
                    dust.scale = 1.5f * Main.rand.NextFloat(0.75f, 1.25f);
                    dust.velocity.X = 10 * (j % 2 == 0 ? 1 : -1) * Main.rand.NextFloat(0.25f, 1.25f);
                }
            }

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height
                    , DustID.t_Slime, 0, 0, 80, new Color(0, 80, 255, 100), Main.rand.NextFloat(0.9f, 1.1f));
                dust.noGravity = false;
            }

            NPC.spriteDirection = NPC.direction = -player.direction;
            iRetardedIframe = player.immuneTime;
            NPC.velocity = player.velocity * 0.5f;
            NPC.Center = player.Center;

            player.AddBuff(BuffID.Slimed, 5);
            NPC.netUpdate = true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Player player = Main.player[(int)NPC.ai[0]];
            int damageToPlayer = (int)(hit.SourceDamage);
            player.HurtCustom("KSGlandAftershock", damageToPlayer, 0);
            player.immune = true;
            player.immuneTime = 40;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage /= 2f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_Slime, 0, 0, 80, new Color(0, 80, 255, 100), Main.rand.NextFloat(1f, 2f));
                dust.velocity *= 2f;
                Dust dust2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_Slime, 0, 0, 80, new Color(0, 80, 255, 100), Main.rand.NextFloat(1f, 2f));
                dust2.velocity *= 1f;
                dust2.noGravity = true;
            }
        }

        public override void OnKill()
        {
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_Slime, 0, 0, 80, new Color(0, 80, 255, 100), Main.rand.NextFloat(1f, 2f));
                dust.velocity *= 2f;
                Dust dust2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_Slime, 0, 0, 80, new Color(0, 80, 255, 100), Main.rand.NextFloat(1f, 2f));
                dust2.velocity *= 1f;
                dust2.noGravity = true;
            }
            Player player = Main.player[(int)NPC.ai[0]];
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            NPC.immune[projectile.owner] = 120;
        }

        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[(int)NPC.ai[0]];
            if (player.velocity.Y < 0)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else if (player.velocity.Y > 0)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.GetModPlayer<KSGlandPlayer>().ksMasterAcc = false;
                NPC.immortal = false;
                NPC.life = 0;
                NPC.checkDead();
                return false;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[(int)NPC.ai[0]];
            Texture2D texture = TextureAssets.Npc[Type].Value;
            int vertSize = texture.Height / Main.npcFrameCount[NPC.type];
            Vector2 drawOrigin = new(texture.Width / 2f, texture.Height / 2f / Main.npcFrameCount[NPC.type]);
            Rectangle frameRect = new(0, NPC.frame.Y, texture.Width, vertSize);

            Vector2 stretch = new Vector2(1.3f, 1.3f + Math.Abs(player.velocity.Y * 0.025f));

            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - screenPos + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f) + new Vector2(0f, NPC.gfxOffY + 4f);
                Color color = drawColor * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, drawPos, frameRect, color, 0f, drawOrigin, stretch, effects, 0);
            }
            spriteBatch.Draw(texture,
                NPC.position - screenPos + new Vector2(NPC.width * 0.5f,
                NPC.height * 0.5f) + new Vector2(0f, NPC.gfxOffY + 4f), frameRect, drawColor, 0f, drawOrigin, stretch,
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return false;
        }
    }
}