class Entity():
    def __init__ (self,p : None):
        print ("Constructing Method")
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Entity')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Entity')

class World():
    def __init__ (self):
        print ("Constructing Method")
    def getEnemies(self) -> None:
        print('called getEnemies on World')
    def getOreDeposits(self) -> None:
        print('called getOreDeposits on World')
    def getEnemyTower(self) -> Entity:
        print('called getEnemyTower on World')
    def getAllEntities(self) -> None:
        print('called getAllEntities on World')
    def getAllEntitiesOfType(self,typeName : str) -> None:
        print('called getAllEntitiesOfType on World')

class Soldier():
    def __init__ (self,p : None):
        print ("Constructing Method")
    def isDead(self) -> bool:
        print('called isDead on Soldier')
    attackRange = float()
    def MovePlayer(self,move : None) -> None:
        print('called MovePlayer on Soldier')
    def SetPath(self,path : None) -> None:
        print('called SetPath on Soldier')
    def PathCompleted(self) -> bool:
        print('called PathCompleted on Soldier')
    def MoveOnPathNext(self) -> None:
        print('called MoveOnPathNext on Soldier')
    def IsInRange(self,entity : Entity) -> bool:
        print('called IsInRange on Soldier')
    def Attack(self,entity : Entity) -> None:
        print('called Attack on Soldier')
    def CollectOre(self,ore : OreDeposit) -> None:
        print('called CollectOre on Soldier')
    def MoveToCharacter(self,character : Character) -> None:
        print('called MoveToCharacter on Soldier')
    def MoveToPos(self,pos : None) -> None:
        print('called MoveToPos on Soldier')
    def MoveToEntity(self,entity : Entity) -> None:
        print('called MoveToEntity on Soldier')
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Soldier')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Soldier')

class Trap():
    def __init__ (self,p : None):
        print ("Constructing Method")
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Trap')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Trap')

class Entity():
    def __init__ (self,p : None):
        print ("Constructing Method")
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Entity')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Entity')

class Character():
    def __init__ (self,p : None):
        print ("Constructing Method")
    def isDead(self) -> bool:
        print('called isDead on Character')
    attackRange = float()
    def MovePlayer(self,move : None) -> None:
        print('called MovePlayer on Character')
    def SetPath(self,path : None) -> None:
        print('called SetPath on Character')
    def PathCompleted(self) -> bool:
        print('called PathCompleted on Character')
    def MoveOnPathNext(self) -> None:
        print('called MoveOnPathNext on Character')
    def IsInRange(self,entity : Entity) -> bool:
        print('called IsInRange on Character')
    def Attack(self,entity : Entity) -> None:
        print('called Attack on Character')
    def CollectOre(self,ore : OreDeposit) -> None:
        print('called CollectOre on Character')
    def MoveToCharacter(self,character : Character) -> None:
        print('called MoveToCharacter on Character')
    def MoveToPos(self,pos : None) -> None:
        print('called MoveToPos on Character')
    def MoveToEntity(self,entity : Entity) -> None:
        print('called MoveToEntity on Character')
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Character')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Character')

class Giant():
    def __init__ (self,p : None):
        print ("Constructing Method")
    def DeployShield(self,raised : bool) -> None:
        print('called DeployShield on Giant')
    def isDead(self) -> bool:
        print('called isDead on Giant')
    attackRange = float()
    def MovePlayer(self,move : None) -> None:
        print('called MovePlayer on Giant')
    def SetPath(self,path : None) -> None:
        print('called SetPath on Giant')
    def PathCompleted(self) -> bool:
        print('called PathCompleted on Giant')
    def MoveOnPathNext(self) -> None:
        print('called MoveOnPathNext on Giant')
    def IsInRange(self,entity : Entity) -> bool:
        print('called IsInRange on Giant')
    def Attack(self,entity : Entity) -> None:
        print('called Attack on Giant')
    def CollectOre(self,ore : OreDeposit) -> None:
        print('called CollectOre on Giant')
    def MoveToCharacter(self,character : Character) -> None:
        print('called MoveToCharacter on Giant')
    def MoveToPos(self,pos : None) -> None:
        print('called MoveToPos on Giant')
    def MoveToEntity(self,entity : Entity) -> None:
        print('called MoveToEntity on Giant')
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Giant')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Giant')

class Healer():
    def __init__ (self,p : None):
        print ("Constructing Method")
    def heal(self) -> None:
        print('called heal on Healer')
    def emp(self) -> None:
        print('called emp on Healer')
    def isDead(self) -> bool:
        print('called isDead on Healer')
    attackRange = float()
    def MovePlayer(self,move : None) -> None:
        print('called MovePlayer on Healer')
    def SetPath(self,path : None) -> None:
        print('called SetPath on Healer')
    def PathCompleted(self) -> bool:
        print('called PathCompleted on Healer')
    def MoveOnPathNext(self) -> None:
        print('called MoveOnPathNext on Healer')
    def IsInRange(self,entity : Entity) -> bool:
        print('called IsInRange on Healer')
    def Attack(self,entity : Entity) -> None:
        print('called Attack on Healer')
    def CollectOre(self,ore : OreDeposit) -> None:
        print('called CollectOre on Healer')
    def MoveToCharacter(self,character : Character) -> None:
        print('called MoveToCharacter on Healer')
    def MoveToPos(self,pos : None) -> None:
        print('called MoveToPos on Healer')
    def MoveToEntity(self,entity : Entity) -> None:
        print('called MoveToEntity on Healer')
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Healer')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Healer')

class Turret():
    def __init__ (self,p : None):
        print ("Constructing Method")
    def targetCharacter(self,enemy : Character) -> None:
        print('called targetCharacter on Turret')
    def shootCharacter(self) -> None:
        print('called shootCharacter on Turret')
    def lookAt(self,pos : vector2) -> None:
        print('called lookAt on Turret')
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on Turret')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on Turret')

class OreDeposit():
    def __init__ (self,p : None):
        print ("Constructing Method")
    position = None()
    owner = int()
    id = str()
    health = int()
    def pos(self) -> vector2:
        print('called pos on OreDeposit')
    def findClosestEntityOfType(self,type : str) -> Entity:
        print('called findClosestEntityOfType on OreDeposit')

class vector2():
    def __init__ (self,_x : float,_y : float):
        print ("Constructing Method")
    def fromVec2(self,v : None) -> vector2:
        print('called fromVec2 on vector2')
    def getVect2(self) -> None:
        print('called getVect2 on vector2')

