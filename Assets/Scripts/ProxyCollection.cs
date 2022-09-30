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
        public string instanceVariableName;

        public PythonClass(string className, bool onlyPythonAcceptedMethods = false, string instanceVariableName = "")
        {
            this.className = className;
            this.pythonAcceptedMethodsOnly = onlyPythonAcceptedMethods;
            this.instanceVariableName = instanceVariableName;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class PythonMethod : System.Attribute { }


    [PythonClass("Unit")]
    public class UnitProxy : PythonProxyObject
    {
        public Unit target;

        public UnitProxy(Unit p)
        {
            this.pythonClassName = "Unit";
            this.target = p;
        }

        public vector2 position
        {
            get
            {
                return new vector2(target.gridPos.x, target.gridPos.y);
            }
        }

        public string owner => target.ownerPlayer.playerID;

        public vector2 pos()
        {
            return vector2.fromVec2(new Vector2(target.transform.position.x, target.transform.position.z));
        }
    }

    [PythonClass("Entity")]
    public class EntityProxy : PythonProxyObject
    {
        public Entity target;

        public EntityProxy(Entity p)
        {
            this.pythonClassName = "Unit";
            this.target = p;
        }
    }

    [PythonClass("PlaceableObject")]
    public class PlaceableObjectProxy : PythonProxyObject
    {
        public PlaceableObject target;

        public PlaceableObjectProxy(PlaceableObject p)
        {
            this.pythonClassName = "PlaceableObject";
            this.target = p;
        }
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

    // Modules

    [PythonClass("Module")]
    public class ModuleProxy
    {
        public Module target;
        public ModuleProxy (Module p)
        {
            this.target = p;
        }
    }

    [PythonClass("MoveModule",false,"moveModule")]
    public class MoveModuleProxy : ModuleProxy
    {
        public MoveModule target;
        public MoveModuleProxy(MoveModule p) : base(p)
        {
            this.target = p;
        }

        public void move (vector2 dir)
        {
            this.target.moveUnit(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));
        }

        public void MoveToEntity(EntityProxy entity) { target.MoveTo(entity.target.gridPos); }

        public void MovePlayer(vector2 move) { target.replicatedMove(Mathf.RoundToInt(move.x), Mathf.RoundToInt(move.y)); }
        //public void SetPath(List<vector2> path) { target.SetPath(path); }
        public bool PathCompleted() { return target.PathCompleted(); }
        public void MoveOnPathNext() { target.MoveOnPathNext(); }
        public void MoveToObject(PlaceableObject character) { target.MoveToObject(character); }
        public void MoveToPos(vector2 pos) { target.MoveTo(new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y))); }

        }

    [PythonClass("MeleeModule", false, "meleeModule")]
    public class MeleeModuleProxy : ModuleProxy
    {
        public MeleeModule target;
        public MeleeModuleProxy(MeleeModule p) : base(p)
        {
            this.target = p;
        }

        public void attack(vector2 dir)
        {
            this.target.attack(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));
        }
    }

    [PythonClass("ShieldModule", false, "shieldModule")]
    public class ShieldModuleProxy : ModuleProxy
    {
        public ShieldModule target;
        public ShieldModuleProxy(ShieldModule p) : base(p)
        {
            this.target = p;
        }
        
        public void positionShield(bool raised)
        {
            this.target.positionShield(raised);
        }

        public void lowerShield()
        {
            this.target.lowerShield();
        }
    }

    [PythonClass("HealerModule", false, "healerModule")]
    public class HealerModuleProxy : ModuleProxy
    {
        public HealerModule target;
        public HealerModuleProxy(HealerModule p) : base(p)
        {
            this.target = p;
        }

        public void heal()
        {
            this.target.heal();
        }
    }

    [PythonClass("HackerModule", false, "hackerModule")]
    public class HackerModuleProxy : ModuleProxy
    {
        public HackerModule target;
        public HackerModuleProxy(HackerModule p) : base(p)
        {
            this.target = p;
        }

        public void hack(vector2 direction)
        {
            this.target.hack(direction);
        }
    }

    [PythonClass("EMPModule", false, "empModule")]
    public class EMPModuleProxy : ModuleProxy
    {
        public EMPModule target;
        public EMPModuleProxy(EMPModule p) : base(p)
        {
            this.target = p;
        }

        public void EMP()
        {
            this.target.EMP();
        }
    }

    [PythonClass("TurretModule", false, "turretModule")]
    public class TurretModuleProxy : ModuleProxy
    {
        public TurretModule target;
        public TurretModuleProxy(TurretModule p) : base(p)
        {
            this.target = p;
        }

        public void shoot()
        {
            this.target.shoot();
        }
        
        public void targetEntity(PlaceableObject entity)
        {
            this.target.targetEntity(entity);
        }
        
        public void lookAt(vector2 pos) => target.lookAt(pos);
    }

}