using UnityEngine;
using Combat.Scene;
using GameServices.Settings;
using Zenject;

public class TestCamera1 : MonoBehaviour
{

			[SerializeField] GameObject Background;
			[SerializeField] float MinSize = 5;
            [SerializeField] float MaxSize = 25;
            //[SerializeField] float Border = 5;
			[SerializeField] float ZoomSpeed = 5;
			[SerializeField] float LinearSpeed = 5;
            [SerializeField] private float ZoomOverride = -1;

		    [Inject]
		    private void Initialize(IScene scene)
		    {
				Debug.Log((scene == null) + "111111111111111111111");
		        _scene = scene;

		    }

            private void Start()
			{
                _zoom = ZoomOverride >= 0 ? ZoomOverride : 25;
			}
			
			private void LateUpdate()
			{
				var camera = gameObject.GetComponent<UnityEngine.Camera>();
				var viewRect = _scene.ViewRect;
				var orthographicSize = Mathf.Clamp(0.5f*Mathf.Max(viewRect.width/camera.aspect, viewRect.height), MinSize + _zoom*(MaxSize - MinSize), MaxSize);
				var delta = Mathf.Min(ZoomSpeed*Time.unscaledDeltaTime, 1);
				camera.orthographicSize += (orthographicSize - camera.orthographicSize)*delta;
				var position = Vector2.Lerp(transform.position, viewRect.center, LinearSpeed*Time.unscaledDeltaTime);
				gameObject.Move(position);

				if (Background != null)
					Background.Move(position);
			}

			private float _zoom;
            private IScene _scene;
}
