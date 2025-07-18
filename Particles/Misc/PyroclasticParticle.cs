﻿using ITD.Utilities;
using ITD.Utilities.EntityAnim;

namespace ITD.Particles.Misc
{
    public class PyroclasticParticle : ParticleEmitter
    {
        public override void OnEmitParticle(ref ITDParticle particle)
        {
            particle.scale *= 0.5f + Main.rand.NextFloat();
        }
        public override void AI(ref ITDParticle particle)
        {
            particle.scale = particle.spawnParameters.Scale * particle.ProgressOneToZero;
        }
        public override Color GetAlpha(ITDParticle particle)
        {
            float prog = EasingFunctions.OutQuad(particle.ProgressZeroToOne);
            return MiscHelpers.LerpMany(prog, [Color.White, Color.Yellow, Color.Red]);
        }
    }
}
