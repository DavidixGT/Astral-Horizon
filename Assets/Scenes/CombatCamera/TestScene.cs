using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using UnityEngine;
using Combat.Scene;
using CommonComponents.Signals;

public class TestScene : IScene
{
           public void AddUnit(IUnit unit)
           {

           }

        public IUnitList<IShip> Ships { get; }
        public IUnitList<IUnit> Units { get; }

        public IShip PlayerShip { get; }
        public IShip EnemyShip { get; }

        public Vector2 FindFreePlace(float minDistance, UnitSide unitSide)
        {return Vector2.zero;}
        public void Shake(float amplitude){}

        public Vector2 ViewPoint { get; }
        public Rect ViewRect { get; }

        public SceneSettings Settings { get; }
	
		public void Clear(){}
}
