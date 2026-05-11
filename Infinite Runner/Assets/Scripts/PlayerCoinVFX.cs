using UnityEngine;

public class PlayerCoinVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] coinCollectParticles;

    public void PlayCoinCollectVFX()
    {
        if (coinCollectParticles == null || coinCollectParticles.Length == 0)
        {
            return;
        }

        foreach (ParticleSystem particle in coinCollectParticles)
        {
            if (particle == null) continue;

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play();
        }
    }
}