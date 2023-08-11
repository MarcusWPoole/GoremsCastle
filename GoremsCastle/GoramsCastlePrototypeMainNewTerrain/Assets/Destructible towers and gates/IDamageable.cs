using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
   void TakesDamage(int Damage);
   Transform GetTransform();
}
