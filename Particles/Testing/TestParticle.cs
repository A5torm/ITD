﻿using Microsoft.Xna.Framework;
using Terraria;

namespace ITD.Particles.Testing
{
    public class TestParticle : ITDParticle
    {
        public override void SetStaticDefaults()
        {
            ParticleSystem.particleFramesVertical[type] = 5;
        }
        public override void SetDefaults()
        {
            canvas = ParticleDrawCanvas.UI;
            timeLeft = 120;
        }
        public override void PreUpdate()
        {
            velocity += Vector2.UnitY * 0.1f;
            rotation = velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}