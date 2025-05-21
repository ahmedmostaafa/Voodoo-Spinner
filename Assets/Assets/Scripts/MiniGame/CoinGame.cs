using System.Collections.Generic;
using System.Linq;
using KabreetGames.SceneReferences;
using Unity.VisualScripting;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Assets.Scripts.MiniGame
{
    public class CoinGame : ValidatedMonoBehaviour
    {
        private readonly HashSet<Gate> gates = new();
        private Rigidbody2D rb;
        private Collider2D coll;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
        }

        public bool CanGetThrowGate(Gate gate)
        {
            return gates.Add(gate);
        }

        private void FixedUpdate()
        {
            if (coll.enabled || rb.linearVelocity.y > 0) return;
            coll.enabled = true;
            rb.gravityScale = 1;
            ClearGate();
        }
        
        public void SetVelocity(Vector2 rbLinearVelocity)
        {
            rb.linearVelocity = rbLinearVelocity;
        }

        private void ClearGate()
        {
            var removeGates = gates.Where(g => g is not NumberGate).ToList();
            gates.Clear();
            gates.AddRange(removeGates);
        }

        public void CopyGate(CoinGame component)
        {
            gates.AddRange(component.gates);
        }

        public void Bounce()
        {
            coll.enabled = false;
            var velocity = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 2;
            rb.AddForce(new Vector2(velocity.x + Random.Range( -1f, 1f), -velocity.y).normalized * 15f, ForceMode2D.Impulse);

        }

        public void ResetData()
        {
            coll.enabled = true;
            rb.gravityScale = 1;
            gates.Clear();
        }
    }
}