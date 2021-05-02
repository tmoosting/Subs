using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    

    void FixedUpdate()
    {
        PropelTorpedo();
    }

    void PropelTorpedo()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float maxMagnitude = GameController.Instance.torpedoMaxSpeed / GameController.Instance.knotsPerMagnitude;
        if (rb.velocity.magnitude < maxMagnitude)
        {
            rb.AddForce(transform.up * GameController.Instance.torpedoAccelerationRate * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // hits a ship
        if (collision.gameObject.GetComponent<Ship>() != null)
        {
            ExplodeOnShip(collision.gameObject.GetComponent<Ship>());
        }
    }

    void ExplodeOnShip(Ship ship)
    {
        ship.EatTorpedo();
        Destroy(gameObject);
        SoundController.Instance.PlayTorpedoHitSound();
        GameObject explosion = Instantiate(GameController.Instance.explosionPrefab);
        explosion.transform.position = transform.position;
        ParticleSystem explosionParticles = explosion.GetComponent<ParticleSystem>();
        explosionParticles.Play();
    }
}
