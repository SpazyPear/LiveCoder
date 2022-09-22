using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python3DMath;

namespace PythonProxies
{

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class PythonClass : System.Attribute
    {
        public string className;
        public bool pythonAcceptedMethodsOnly;
        public PythonClass(string className, bool onlyPythonAcceptedMethods = false)
        {
            this.className = className;
            this.pythonAcceptedMethodsOnly = onlyPythonAcceptedMethods;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class PythonMethod : System.Attribute { }

    [PythonClass("Soldier")]
    public class SoldierProxy : CharacterHandlerProxy
    {
        Soldier target;

        public SoldierProxy(Soldier p) : base(p)
        {
            this.target = p;
        }
    }


    [PythonClass("Trap")]
    public class TrapProxy : EntityProxy
    {
        Trap target;

        public TrapProxy(Trap p) : base(p)
        {
            this.target = p;
        }

    }

    [PythonClass("Entity")]
    public class EntityProxy : PythonProxyObject
    {
        public Entity target;

        public EntityProxy(Entity p)
        {
            this.pythonClassName = "Entity";
            this.target = p;
        }

        public Vector2Int position
        {
            get
            {
                return target.gridPos;
            }
        }

        public string owner => target.ownerPlayer.playerID;

        public string id => target.ID.ToString();
        public int health => target.currentHealth;

        public vector2 pos()
        {
            return vector2.fromVec2(new Vector2(target.transform.position.x, target.transform.position.z));
        }

        public EntityProxy findClosestEntityOfType(string type)
        {
            Entity closest = target.findClosestEntityOfType(target, type);

            if (closest != null)
            {
                return closest.CreateProxy() as EntityProxy;
            }
            else
            {
                return null;
            }
        }

        // Override Equality Check to see if objects are the same
        public override bool Equals(object obj)
        {
            if (obj is EntityProxy)
            {
                if (((EntityProxy)obj).target == target){
                    return true;
                }
            }

            return false;
        }
    }

    [PythonClass("Character")]
    public class CharacterHandlerProxy : EntityProxy
    {
        public Character target;

        public CharacterHandlerProxy(Character p) : base(p)
        {
            this.target = p;
        }

        public bool isDead() { return (target == null || target.currentHealth <= 0); }

        public float attackRange
        {
            get
            {
                return target.characterData.range;
            }
        }
        
        public void AttackInDirection(int x, int y) { target.attack(x, y); }

        public void MoveToEntity(EntityProxy entity) { target.MoveTo(entity.target.gridPos); }

        public void CheckForInRangeEntities(string typeName, bool friendlies, bool enemies) { target.checkForInRangeEntities(typeName, friendlies, enemies); }

        
        public void MovePlayer(vector2 move) { target.replicatedMove(Mathf.RoundToInt(move.x), Mathf.RoundToInt(move.y)); }
        //public void SetPath(List<vector2> path) { target.SetPath(path); }
        public bool PathCompleted() { return target.PathCompleted(); }
        public void MoveOnPathNext() { target.MoveOnPathNext(); }

        public bool IsInRange(EntityProxy entity) { return target.checkForInRangeEntities("Entity", true, true).Contains(entity.target); } // make good

        public void Attack(int x, int y) { target.attack(x,y); }

      
        public void MoveToCharacter(CharacterHandlerProxy character) { target.MoveToCharacter(character.target); }
        public void MoveToPos(vector2 pos) { target.MoveTo(new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y))); }

        


    }

    [PythonClass("Giant")]
    public class GiantHandlerProxy : CharacterHandlerProxy
    {
        Giant target;

        public GiantHandlerProxy(Giant p) : base(p)
        {
            this.target = p;
        }

        public void DeployShield(bool raised) { target.replicatedPositionShield(raised); }
    }

    [PythonClass("Healer")]
    public class HealerHandlerProxy : CharacterHandlerProxy
    {
        Healer target;

        public HealerHandlerProxy(Healer p) : base(p)
        {
            this.target = p;
        }

        public void heal() { target.heal(); }

        public void emp() { target.OnEMP(); }
    }

    [PythonClass("Turret")]
    public class TurretProxy : EntityProxy
    {
        Turret target;

        public TurretProxy(Turret p) : base(p)
        {
            this.pythonClassName = "Turret";
            this.target = p;
        }

        public void targetCharacter(CharacterHandlerProxy enemy) => target.target(enemy.target);
        public void shoot()
        {
            Debug.Log("Turret shooting through proxy");
            target.shoot();
        }

        public void lookAt(vector2 pos) => target.lookAt(pos);

    }

    [PythonClass("OreDeposit")]
    public class OreDepositProxy : EntityProxy
    {
        public OreDeposit target;

        public OreDepositProxy(OreDeposit p) : base(p)
        {
            this.target = p;
        }
    }
}