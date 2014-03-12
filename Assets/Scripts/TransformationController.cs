using UnityEngine;
using System.Collections;

public class TransformationController : MonoBehaviour {

	public Object shipModel;
	public Object transformationAnimation;
	public Object characterModel;

	private enum State {
		Ship,
		Transforming,
		Biped,
		Sentinel
	};

	private string modelName = "Model";

	private State state = State.Ship;
	
		// Use this for initialization
	void Start () {
		// Which prefab should we have in the next state?
		Object prefab = GetPrefabForState(state);

		// Use the prefab as the "model"
		SetModelPrefab(prefab);
	}

	bool CanGotoNextState()
	{
		return true;
	}

	// Given the current state, what is the next state we should be in?
	State GetNextState()
	{
		State nextState = state + 1;

		if(state == State.Sentinel)
			state = State.Ship;

		return nextState;
	}

	Object GetPrefabForState(State state)
	{
		switch(state)
		{
			case State.Ship:
				return shipModel;
				break;

			case State.Transforming:
				return transformationAnimation;
				break;

			case State.Biped:
				return characterModel;
				break;

			default:
				DebugUtils.Assert(false);
				return null;
				break;
		}
	}

	// Instanciates and connects the prefab, then returns it
	GameObject SetModelPrefab(Object prefab)
	{
		// Get rid of any existing model prefabs
		Transform child = transform.Find(modelName);
		if(child)
		{
			DestroyObject(child.gameObject);
		}

		// Create a new instance from the given prefab and attach it as a child
		GameObject instance = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
		instance.name = modelName;
		instance.transform.parent = transform;

		return instance;
	}

	public IEnumerator PlayAnimation(Animation animation, string paramName)
	{
		animation.Play("ToRobot");
		while(animation.IsPlaying("ToRobot"))
			yield return null;
	}

	// Update is called once per frame
	void Update () {
		bool stateChangeRequest = false;

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(CanGotoNextState())
				stateChangeRequest = true;
		}

		if(stateChangeRequest)
		{
			// What's the next state we should be in?
			State nextState = GetNextState();

			// Which prefab should we have in the next state?
			Object prefab = GetPrefabForState(nextState);

			// Use the prefab as the "model"
			GameObject child = SetModelPrefab(prefab);

			if(child.animation != null)
				StartCoroutine(PlayAnimation(child.animation, "ToRobot"));

			state = nextState;
		}
	}
}
