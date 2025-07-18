﻿using Terraria.DataStructures;
using Terraria.GameContent;

namespace ITD.Content.NPCs.Bosses
{
    public class CosmicJellyfishMini : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            //is buggy
/*            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 4;*/
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            //
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }
        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 74;
            NPC.damage = 10;
            NPC.defense = 0;
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.3f;
            NPC.netAlways = true;
            NPC.aiStyle = -1;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            NPC.checkDead();
            NPC.life = 0;
        }
        float speed = 6;
        float inertia = 40;
        float distanceToIdlePosition;
        float distance;
        bool movementallow;
        public override void AI()
        {

            // WARNING: DO NOT FORGET THIS LINE(obvious but yes)
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            Vector2 idlePosition = player.Center + new Vector2(NPC.ai[1], NPC.ai[2]);
            Vector2 vectorToIdlePosition = idlePosition - NPC.Center;
            Vector2 vectorToIdlePositionNorm = vectorToIdlePosition.SafeNormalize(Vector2.UnitY);
            distanceToIdlePosition = vectorToIdlePosition.Length();
            float distance = 200;
            //speed is dependant on attack
            if (distanceToIdlePosition > 10f)
            {
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
            }
            if (movementallow)
            {
                NPC.velocity = (NPC.velocity * (inertia - 2) + vectorToIdlePosition) / inertia;
            }
            NPC.netUpdate = true;
            speed = 8;
            inertia = 40;
            if (!IsDashing)
            {
                for (int i = 0; i < 3; i++)
                {
                    float X = NPC.Center.X - NPC.velocity.X / 20f * (float)i;
                    float Y = NPC.Center.Y - NPC.velocity.Y / 20f * (float)i;
                    int dust2 = Dust.NewDust(new Vector2(X, Y), NPC.width, NPC.height, DustID.ShimmerTorch, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 160, default, 2);
                    Main.dust[dust2].noGravity = true;
                    Main.dust[dust2].velocity *= 0.1f;
                }
                if (NPC.localAI[2]++ >= 150)
                {
                    NPC.velocity *= 0.9f;
                    movementallow = false;
                    if (NPC.localAI[2]++ >= 200)//Have to stop first
                    {

                        IsDashing = true;
                    }
                }
                else
                {
                    movementallow = true;
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    float X = NPC.Center.X - NPC.velocity.X / 20f * (float)i;
                    float Y = NPC.Center.Y - NPC.velocity.Y / 20f * (float)i;
                    int dust2 = Dust.NewDust(new Vector2(X, Y), NPC.width, NPC.height, DustID.ShimmerTorch, Main.rand.NextFloat(-2,2), Main.rand.NextFloat(-2, 2), 160, Color.WhiteSmoke, 2);

                    Main.dust[dust2].noGravity = true;
                    Main.dust[dust2].velocity *= 0.1f;
                }
                Dash(1, 5, 10, 60);
            }
            NPC.netUpdate = true;
            NPC.rotation = NPC.velocity.X / 50;
            Vector2 velo = Vector2.Normalize(new Vector2(player.Center.X, player.Center.Y) - new Vector2(NPC.Center.X, NPC.Center.Y));
            NPC.rotation = velo.ToRotation();
        }
        Vector2 dashvel;
        public bool IsDashing;
        public void Dash(int time1, int time2, int time3, int reset)
        {
            IsDashing = true;
            Player player = Main.player[NPC.target];
            NPC.localAI[1]++;
            //very hard coded
                if (NPC.localAI[1] == time1)//set where to dash
                {
                    dashvel = Vector2.Normalize(new Vector2(player.Center.X, player.Center.Y) - new Vector2(NPC.Center.X, NPC.Center.Y));
                    NPC.velocity = dashvel * 2;
                    NPC.netUpdate = true;
                }

                if (NPC.localAI[1] > time1 && NPC.localAI[1] < time2)//xcel
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.netUpdate = true;

                        NPC.velocity *= 2f;
                        NPC.netUpdate = true;
                    }
                }
                if (NPC.localAI[1] > time3) //Decelerate 
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.netUpdate = true;

                        NPC.velocity *= 0.96f;
                        NPC.netUpdate = true;

                    }
                }
                if (NPC.localAI[1] >= reset)//
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                    IsDashing = false;
            }

        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)          //this make so when the npc has 0 life(dead) he will spawn this
            {
                NPC.checkDead();
                NPC.life = 0;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.ShimmerTorch, 0, 0f, 40, default, Main.rand.NextFloat(2f, 3f));
                dust.velocity *= 2f;
                Dust dust2 = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.ShimmerTorch, 0, 0f, 40, default, Main.rand.NextFloat(2f, 3f));
                dust2.velocity *= 1f;
                dust2.noGravity = true;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            NPC.checkDead();
            NPC.life = 0;
        }
        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }
        public override void OnKill()
        {
            Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        }
        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 4;

            int frameSpeed = 8;
            NPC.frameCounter += 1f;
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (IsDashing)
            {
                Texture2D texture = TextureAssets.Npc[Type].Value;
                Vector2 drawOrigin = texture.Size() / 2f;
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - screenPos + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f) + new Vector2(0f, NPC.gfxOffY + 4f);
                    Color color = drawColor * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    spriteBatch.Draw(texture, drawPos, null, color, 0f, drawOrigin, NPC.scale, effects, 0);
                }
            }
            return true;
        }
    }
}