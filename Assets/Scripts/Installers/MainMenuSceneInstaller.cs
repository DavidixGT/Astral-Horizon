﻿using Services.Messenger;
using GameServices.SceneManager;
using Zenject;

namespace Installers
{
    public class MainMenuSceneInstaller : MonoInstaller<MainMenuSceneInstaller>
    {
        public override void InstallBindings()
        {
			Container.BindInterfacesTo<Messenger>().AsSingle().WithArguments(GameScene.MainMenu);
		}
	}
}
