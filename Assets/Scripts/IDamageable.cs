using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IDamageable
{
    public abstract void die(object sender = null);
    
    public abstract void takeDamage(int damage, object sender = null);
}
