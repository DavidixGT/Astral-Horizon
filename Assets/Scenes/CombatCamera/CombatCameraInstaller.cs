using System.Collections;
using System.Collections.Generic;
using Combat.Scene;
using UnityEngine;
using Zenject;
using GameServices.Settings;
using Combat.Camera;
using System.Threading;

public class CombatCameraInstaller : MonoInstaller<CombatCameraInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TestScene>().AsSingle();

        //Container.BindInterfacesAndSelfTo<GameSettings>().AsSingle().NonLazy();
        //Container.BindInterfacesTo<CombatCamera>().AsSingle();
    }
}
